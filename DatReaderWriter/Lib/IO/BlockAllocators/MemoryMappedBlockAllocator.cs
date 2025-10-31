using DatReaderWriter.Options;
using DatReaderWriter.Lib.IO;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteBytes(byte[] buffer, int byteOffset, int numBytes) {
            fixed (byte* bufferPtr = buffer) {
                Buffer.MemoryCopy(bufferPtr, _viewPtr + byteOffset, numBytes, numBytes);
            }
        }

        /// <inheritdoc/>
        public override int WriteBlock(byte[] buffer, int numBytes, int startingBlock = 0) {
            startingBlock = startingBlock > 0 ? startingBlock : ReserveBlock();

            int currentBlock = startingBlock;
            int bufferIndex = 0;
            int blockDataSize = Header.BlockSize - 4;

            while (bufferIndex < numBytes) {
                int size = Math.Min(blockDataSize, numBytes - bufferIndex);

                fixed (byte* dataPtr = &buffer[bufferIndex]) {
                    // Write data to current block (skip first 4 bytes for next pointer)
                    Buffer.MemoryCopy(dataPtr, _viewPtr + currentBlock + 4, size, size);
                }

                bufferIndex += size;

                // Determine next block
                int nextBlock;
                if (bufferIndex < numBytes) {
                    // Read existing next block pointer
                    nextBlock = *(int*)(_viewPtr + currentBlock);

                    if (nextBlock <= 0) {
                        nextBlock = ReserveBlock();
                    }
                }
                else {
                    nextBlock = 0; // End of chain
                }

                // Write next block pointer
                *(int*)(_viewPtr + currentBlock) = nextBlock;
                currentBlock = nextBlock;
            }

            WriteHeader();

            return startingBlock;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void ReadBytes(byte[] buffer, int bufferOffset, int byteOffset, int numBytes) {
            fixed (byte* bufferPtr = &buffer[bufferOffset]) {
                Buffer.MemoryCopy(_viewPtr + byteOffset, bufferPtr, numBytes, numBytes);
            }
        }

        /// <inheritdoc/>
        public override void ReadBlock(byte[] buffer, int startingBlock) {
            int bufferOffset = 0;
            int blockDataSize = Header.BlockSize - 4;
            int bufferLength = buffer.Length;

            fixed (byte* bufferPtr = buffer) {
                while (startingBlock != 0 && bufferOffset < bufferLength) {
                    int bytesToRead = Math.Min(blockDataSize, bufferLength - bufferOffset);

                    // Copy data from block (skip first 4 bytes)
                    Buffer.MemoryCopy(_viewPtr + startingBlock + 4, bufferPtr + bufferOffset, bytesToRead, bytesToRead);

                    bufferOffset += bytesToRead;

                    if (bufferOffset >= bufferLength) {
                        return;
                    }

                    // Read next block pointer directly
                    startingBlock = *(int*)(_viewPtr + startingBlock);
                }
            }
        }

        /// <inheritdoc/>
        public override bool TryGetBlockOffsets(int startingBlock, out List<int> fileBlocks) {
            fileBlocks = new List<int>();

            while (startingBlock != 0) {
                fileBlocks.Add(startingBlock);
                startingBlock = *(int*)(_viewPtr + startingBlock);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DestroyMappedFile() {
            if (_view is not null) {
                if (CanWrite) {
                    _view.Flush();
                }
                _view.SafeMemoryMappedViewHandle.ReleasePointer();
                _viewPtr = null;
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

            viewPtr = null;
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