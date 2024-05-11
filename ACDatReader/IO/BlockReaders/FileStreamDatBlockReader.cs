using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ACDatReader.IO.BlockReaders {
    /// <summary>
    /// Dat block reader that uses a FileStream.
    /// </summary>
    public class FileStreamDatBlockReader : IDatBlockReader {
        private readonly static ArrayPool<byte> sharedBytes = ArrayPool<byte>.Shared;
        private readonly FileStream _datStream;

        /// <summary>
        /// Create a new reader
        /// </summary>
        /// <param name="datFilePath">The path to the dat file to read</param>
        public FileStreamDatBlockReader(string datFilePath) {
            _datStream = new FileStream(datFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1024, FileOptions.RandomAccess);
        }

        /// <inheritdoc cref="IDatBlockReader.ReadBytes(byte[], int, int)"/>
        public void ReadBytes(byte[] buffer, int offset, int numBytes) {
            _datStream.Seek(offset, SeekOrigin.Begin);
            _datStream.ReadExactly(buffer, 0, numBytes);
        }

        /// <inheritdoc cref="IDatBlockReader.ReadBlocks(byte[], int, int)"/>
        unsafe public void ReadBlocks(byte[] buffer, int startingBlock, int blockSize) {
            var nextBlockBuffer = stackalloc byte[4];
            Span<int> bufferStatsSpan = [0, 0];

            while (startingBlock != 0 && bufferStatsSpan[0] < buffer.Length) {
                _datStream.Seek(startingBlock, SeekOrigin.Begin);

                var nextBlockSpan = new Span<byte>(nextBlockBuffer, 4);
                _datStream.ReadExactly(nextBlockSpan);
                startingBlock = BinaryPrimitives.ReadInt32LittleEndian(nextBlockSpan);

                bufferStatsSpan[1] = Math.Min(blockSize - 4, buffer.Length - bufferStatsSpan[0] - 4);
                _datStream.ReadExactly(buffer, bufferStatsSpan[0], bufferStatsSpan[1]);

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
                _datStream?.Dispose();
            }
        }
    }
}