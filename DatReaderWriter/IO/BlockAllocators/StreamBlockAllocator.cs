using ACClientLib.DatReaderWriter.Options;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;

namespace ACClientLib.DatReaderWriter.IO.BlockAllocators {
    /// <summary>
    /// A block allocator that uses file streams for reading and writing.
    /// </summary>
    public class StreamBlockAllocator : BaseBlockAllocator {
        private readonly FileStream _datStream;
        private readonly object _streamLock = new();

        /// <summary>
        /// Create a new stream block allocator from an existing dat file.
        /// </summary>
        /// <param name="options">The options to use</param>
        public StreamBlockAllocator(DatDatabaseOptions options) : base(options) {
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

            TryReadHeader();
        }

        /// <inheritdoc/>
        public override void WriteBytes(byte[] buffer, int byteOffset, int numBytes) {
            lock (_streamLock) {
                _datStream.Position = byteOffset;
                _datStream.Write(buffer, 0, numBytes);
                _datStream.Flush();
            }
        }

        /// <inheritdoc/>
        public override int WriteBlock(byte[] buffer, int numBytes, int startingBlock = 0) {
            startingBlock = startingBlock > 0 ? startingBlock : ReserveBlock();

            var nextBlockBuffer = new byte[4];
            var currentBlock = startingBlock;
            var bufferIndex = 0;

            lock (_streamLock) {
                while (bufferIndex < numBytes) {
                    var size = Math.Min(Header.BlockSize - 4, numBytes - bufferIndex);
                    
                    // Write the block data
                    _datStream.Position = currentBlock + 4;
                    _datStream.Write(buffer, bufferIndex, size);
                    
                    bufferIndex += size;
                    
                    // Prepare next block pointer
                    var oldOffset = currentBlock;
                    if (bufferIndex < numBytes) {
                        _datStream.Position = currentBlock;
                        _datStream.Read(nextBlockBuffer, 0, 4);
                        var nextBlock = BinaryPrimitives.ReadInt32LittleEndian(nextBlockBuffer);
                        
                        if (nextBlock <= 0) {
                            currentBlock = ReserveBlock();
                        }
                        else {
                            currentBlock = nextBlock;
                        }
                    }
                    else {
                        currentBlock = 0;
                    }

                    // Write pointer to next block
                    BinaryPrimitives.WriteInt32LittleEndian(nextBlockBuffer, currentBlock);
                    _datStream.Position = oldOffset;
                    _datStream.Write(nextBlockBuffer, 0, 4);
                }

                _datStream.Flush();
            }

            WriteHeader();
            return startingBlock;
        }

        /// <inheritdoc/>
        public override void ReadBytes(byte[] buffer, int bufferOffset, int byteOffset, int numBytes) {
            lock (_streamLock) {
                _datStream.Position = byteOffset;
                _datStream.Read(buffer, bufferOffset, numBytes);
            }
        }

        /// <inheritdoc/>
        public override void ReadBlock(byte[] buffer, int startingBlock) {
            var nextBlockBuffer = new byte[4];
            var currentBlock = startingBlock;
            var totalRead = 0;

            lock (_streamLock) {
                while (currentBlock != 0 && totalRead < buffer.Length) {
                    var bytesToRead = Math.Min(Header.BlockSize - 4, buffer.Length - totalRead);
                    
                    // Read block data
                    _datStream.Position = currentBlock + 4;
                    var bytesRead = _datStream.Read(buffer, totalRead, bytesToRead);
                    totalRead += bytesRead;

                    if (totalRead >= buffer.Length) {
                        return;
                    }

                    // Get next block pointer
                    _datStream.Position = currentBlock;
                    _datStream.Read(nextBlockBuffer, 0, 4);
                    currentBlock = BinaryPrimitives.ReadInt32LittleEndian(nextBlockBuffer);
                }
            }
        }

        /// <inheritdoc/>
        public override bool TryGetBlockOffsets(int startingBlock, out List<int> fileBlocks) {
            fileBlocks = new List<int>();
            var nextBlockBuffer = new byte[4];
            var currentBlock = startingBlock;

            lock (_streamLock) {
                while (currentBlock != 0) {
                    fileBlocks.Add(currentBlock);
                    
                    _datStream.Position = currentBlock;
                    _datStream.Read(nextBlockBuffer, 0, 4);
                    currentBlock = BinaryPrimitives.ReadInt32LittleEndian(nextBlockBuffer);
                }
            }

            return true;
        }

        /// <inheritdoc/>
        protected override void Expand(int newSizeInBytes) {
            if (newSizeInBytes <= _datStream.Length) {
                throw new Exception($"Tried to shrink the database with Expand({newSizeInBytes:N0} bytes) when dat file is already {_datStream.Length:N0} bytes. Can't shrink a database without rewriting!");
            }

            lock (_streamLock) {
                _datStream.SetLength(newSizeInBytes);
                Header.FileSize = newSizeInBytes;
            }
        }

        /// <inheritdoc/>
        public override void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose pattern implementation
        /// </summary>
        /// <param name="disposing">Whether this is being called from Dispose() or finalizer</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
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