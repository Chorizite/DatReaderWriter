using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ACDatReader.IO.BlockReaders {
    /// <summary>
    /// Dat block reader that uses a MemoryMappedFile.
    /// </summary>
    unsafe public class MemoryMappedDatBlockReader : IDatBlockReader {
        private readonly static ArrayPool<byte> sharedBytes = ArrayPool<byte>.Shared;

        private readonly FileStream _datStream;
        private readonly MemoryMappedFile _mappedFile;
        private readonly MemoryMappedViewAccessor _view;
        private readonly byte* _viewPtr;

        /// <summary>
        /// Create a new reader
        /// </summary>
        /// <param name="datFilePath">The path to the dat file to read</param>
        public MemoryMappedDatBlockReader(string datFilePath) {
            _datStream = File.OpenRead(datFilePath);
            _mappedFile = MemoryMappedFile.CreateFromFile(_datStream, null, 0, MemoryMappedFileAccess.Read, HandleInheritability.None, true);

            _view = _mappedFile.CreateViewAccessor(0, _datStream.Length, MemoryMappedFileAccess.Read);
            _view.SafeMemoryMappedViewHandle.AcquirePointer(ref _viewPtr);
        }

        /// <inheritdoc cref="IDatBlockReader.ReadBytes(byte[], int, int)"/>
        public void ReadBytes(byte[] buffer, int blockOffset, int numBytes) {
            new ReadOnlySpan<byte>(_viewPtr + blockOffset, numBytes).CopyTo(buffer);
        }

        /// <inheritdoc cref="IDatBlockReader.ReadBlocks(byte[], int, int)"/>
        public void ReadBlocks(byte[] buffer, int startingBlock, int blockSize) {
            var nextBlockBuffer = stackalloc byte[4];
            Span<int> bufferStatsSpan = [0, 0];

            while (startingBlock != 0 && bufferStatsSpan[0] < buffer.Length) {
                bufferStatsSpan[1] = Math.Min(blockSize - 4, buffer.Length - bufferStatsSpan[0] - 4);
                fixed (byte* dataPtr = &buffer[bufferStatsSpan[0]]) {
                    Buffer.MemoryCopy(_viewPtr + startingBlock + 4, dataPtr, bufferStatsSpan[1], bufferStatsSpan[1]);
                }

                if (bufferStatsSpan[0] + bufferStatsSpan[1] >= buffer.Length) {
                    return;
                }

                Buffer.MemoryCopy(_viewPtr + startingBlock, nextBlockBuffer, 4, 4);
                startingBlock = BinaryPrimitives.ReadInt32LittleEndian(new ReadOnlySpan<byte>(nextBlockBuffer, 4));

                bufferStatsSpan[0] += bufferStatsSpan[1];
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Dispose
        /// </summary>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                _view.SafeMemoryMappedViewHandle.ReleasePointer();
                _view.Dispose();
                _mappedFile.Dispose();
                _datStream.Dispose();
            }
        }
    }
}
