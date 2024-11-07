using DatReaderWriter.Options;
using DatReaderWriter.Lib.IO;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace DatReaderWriter.Lib.IO.BlockAllocators {
    /// <summary>
    /// A block allocator that uses a Memory Mapped file.
    /// </summary>
    unsafe public class MemoryMappedBlockAllocator : BaseBlockAllocator {
        private readonly FileStream _datStream;
        private MemoryMappedFile _mappedFile;
        private MemoryMappedViewAccessor _view;
        private byte* _viewPtr;

        /// <summary>
        /// Create a new memory mapped block allocator from an existing dat file.
        /// </summary>
        /// <param name="options">The options to use</param>
        public MemoryMappedBlockAllocator(DatDatabaseOptions options) : base(options) {
            if (CanWrite) {
                _datStream = File.Open(options.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                // make sure we at least have room for a header, this is a new dat
                if (_datStream.Length < DatHeader.SIZE) {
                    _datStream.SetLength(DatHeader.SIZE);
                }
            }
            else {
                _datStream = File.Open(options.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }

            UpdateViewPtr(out _mappedFile, out _view, out _viewPtr);

            TryReadHeader();
        }

        /// <inheritdoc/>
        public override void WriteBytes(byte[] buffer, int byteOffset, int numBytes) {
            fixed (byte* bufferPtr = &buffer[0]) {
                Buffer.MemoryCopy(bufferPtr, _viewPtr + byteOffset, numBytes, numBytes);
            }
        }

        /// <inheritdoc/>
        public override int WriteBlock(byte[] buffer, int numBytes, int startingBlock = 0) {
            startingBlock = startingBlock > 0 ? startingBlock : ReserveBlock();

            var currentBlockBuffer = stackalloc byte[4];
            var currentBlockSpan = new Span<int>(currentBlockBuffer, 1);
            BinaryPrimitives.WriteInt32LittleEndian(new Span<byte>(currentBlockBuffer, 4), startingBlock);

            var bufferIndex = 0;
            while (bufferIndex < numBytes) {
                fixed (byte* dataPtr = &buffer[bufferIndex]) {
                    var size = Math.Min(Header.BlockSize - 4, numBytes - bufferIndex);
                    Buffer.MemoryCopy(dataPtr, _viewPtr + currentBlockSpan[0] + 4, size, size);

                    bufferIndex += size;

                    var oldOffset = currentBlockSpan[0];
                    if (bufferIndex < numBytes) {
                        Buffer.MemoryCopy(_viewPtr + currentBlockSpan[0], currentBlockBuffer, 4, 4);
                        var nextBlock = BinaryPrimitives.ReadInt32LittleEndian(new Span<byte>(currentBlockBuffer, 4));
                        if (nextBlock <= 0) {
                            currentBlockSpan[0] = ReserveBlock();
                        }
                        else {
                            currentBlockSpan[0] = nextBlock;
                        }
                    }
                    else {
                        currentBlockSpan[0] = 0;
                    }

                    // write pointer to next block, 0 if none
                    Buffer.MemoryCopy(currentBlockBuffer, _viewPtr + oldOffset, 4, 4);
                }
            }

            WriteHeader();

            return startingBlock;
        }

        /// <inheritdoc/>
        public override void ReadBytes(byte[] buffer, int bufferOffset, int byteOffset, int numBytes) {
            new ReadOnlySpan<byte>(_viewPtr + byteOffset, numBytes).CopyTo(buffer.AsSpan().Slice(bufferOffset, numBytes));
        }

        /// <inheritdoc/>
        public override void ReadBlock(byte[] buffer, int startingBlock) {
            var nextBlockBuffer = stackalloc byte[4];
            Span<int> bufferStatsSpan = [0, 0];

            while (startingBlock != 0 && bufferStatsSpan[0] < buffer.Length) {
                bufferStatsSpan[1] = Math.Min(Header.BlockSize - 4, buffer.Length - bufferStatsSpan[0]);
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


        /// <inheritdoc/>
        public override bool TryGetBlockOffsets(int startingBlock, out List<int> fileBlocks) {
            fileBlocks = [];
            var nextBlockBuffer = stackalloc byte[4];

            while (startingBlock != 0) {
                Buffer.MemoryCopy(_viewPtr + startingBlock, nextBlockBuffer, 4, 4);
                startingBlock = BinaryPrimitives.ReadInt32LittleEndian(new ReadOnlySpan<byte>(nextBlockBuffer, 4));
            }

            return true;
        }

        /// <inheritdoc/>
        protected override void Expand(int newSizeInBytes) {
            if (newSizeInBytes <= _datStream.Length) {
                throw new Exception($"Tried to shrink the database with Expand({newSizeInBytes:N0} bytes) when dat file is already {_datStream.Length:N0} bytes. Can't shrink a database without rewriting!");
            }

            _datStream.SetLength(newSizeInBytes);
            Header.FileSize = newSizeInBytes;

            // must recreate the memory mapped file after resizing...
            DestroyMappedFile();
            UpdateViewPtr(out _mappedFile, out _view, out _viewPtr);
        }

        private void DestroyMappedFile() {
            if (_view is not null) {
                if (CanWrite) {
                    _view.Flush();
                }
                _view.SafeMemoryMappedViewHandle.ReleasePointer();
                _viewPtr = (byte*)0;
                _view.Dispose();
                _view = null;
            }

            _mappedFile?.Dispose();
        }

        private void UpdateViewPtr(out MemoryMappedFile mappedFile, out MemoryMappedViewAccessor view, out byte* viewPtr) {
            if (CanWrite) {
                mappedFile = MemoryMappedFile.CreateFromFile(_datStream, null, 0, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, true);
                view = mappedFile.CreateViewAccessor(0, _datStream.Length, MemoryMappedFileAccess.ReadWrite);
            }
            else {
                mappedFile = MemoryMappedFile.CreateFromFile(_datStream, null, 0, MemoryMappedFileAccess.Read, HandleInheritability.None, true);
                view = mappedFile.CreateViewAccessor(0, _datStream.Length, MemoryMappedFileAccess.Read);
            }

            viewPtr = (byte*)0;
            view.SafeMemoryMappedViewHandle.AcquirePointer(ref viewPtr);
        }

        /// <inheritdoc/>
        public override void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                DestroyMappedFile();

                if (CanWrite) {
                    try {
                        _datStream.Flush(true);
                    }
                    catch { }
                }
                _datStream.Dispose();
            }
        }
    }
}
