using DatReaderWriter.Options;
using DatReaderWriter.Lib.IO;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DatReaderWriter.Lib.IO.BlockAllocators {
    /// <summary>
    /// A block allocator that uses file streams for reading and writing.
    /// </summary>
    public class StreamBlockAllocator : BaseBlockAllocator {
        private readonly FileStream _datStream;
        private readonly SemaphoreSlim _streamLock = new(1, 1);

        /// <summary>
        /// Create a new stream block allocator from an existing dat file.
        /// </summary>
        /// <param name="options">The options to use</param>
        public StreamBlockAllocator(DatDatabaseOptions options) : base(options) {
            if (CanWrite) {
                _datStream = File.Open(options.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                    FileShare.ReadWrite);

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
        public override int ReserveBlock() {
            _streamLock.Wait();
            try {
                return base.ReserveBlock();
            }
            finally {
                _streamLock.Release();
            }
        }

        /// <inheritdoc/>
        public override void WriteBytes(byte[] buffer, int byteOffset, int numBytes) {
            _streamLock.Wait();
            try {
                WriteBytesInternal(buffer, 0, numBytes);
                _datStream.Flush();
            }
            finally {
                _streamLock.Release();
            }
        }

        private void WriteBytesInternal(byte[] buffer, int offset, int count) {
            _datStream.Position = offset;
            _datStream.Write(buffer, 0, count);
        }

        /// <inheritdoc/>
        protected override void WriteHeader() {
            var headerBuffer = SharedBytes.Rent(DatHeader.SIZE);
            Header.WriteEmptyTransaction();
            Header.Pack(new DatBinWriter(headerBuffer));
            WriteBytesInternal(headerBuffer, 0, DatHeader.SIZE);
            SharedBytes.Return(headerBuffer);
        }

        /// <inheritdoc/>
        public override int WriteBlock(byte[] buffer, int numBytes, int startingBlock = 0) {
            // startingBlock = startingBlock > 0 ? startingBlock : ReserveBlockCore(); // Moved inside lock

            var nextBlockBuffer = new byte[4];
            var currentBlock = startingBlock;
            var bufferIndex = 0;

            _streamLock.Wait();
            try {
                if (startingBlock <= 0) {
                    startingBlock = ReserveBlockCore();
                    currentBlock = startingBlock;
                }

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
                            // Release lock before reserving new block to avoid deadlock if ReserveBlock needs lock (e.g. for Expand)
                            // We use ReserveBlockCore which assumes lock is held (or safe to call)
                            // But ReserveBlockCore calls AllocateEmptyBlocks -> Expand. 
                            // Expand in StreamBlockAllocator no longer locks.
                            // So we DO NOT need to release lock IF we use ReserveBlockCore and Expand is lock-free!
                            currentBlock = ReserveBlockCore();
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
            finally {
                _streamLock.Release();
            }

            WriteHeader();
            return startingBlock;
        }

        /// <inheritdoc/>
        public override async ValueTask<int> WriteBlockAsync(byte[] buffer, int numBytes, int startingBlock = 0,
            CancellationToken ct = default) {
            // startingBlock = startingBlock > 0 ? startingBlock : ReserveBlockCore(); // Moved inside lock

            var nextBlockBuffer = new byte[4];
            var currentBlock = startingBlock;
            var bufferIndex = 0;

            await _streamLock.WaitAsync(ct);
            try {
                if (startingBlock <= 0) {
                    startingBlock = ReserveBlockCore();
                    currentBlock = startingBlock;
                }

                while (bufferIndex < numBytes) {
                    var size = Math.Min(Header.BlockSize - 4, numBytes - bufferIndex);

                    // Write the block data
                    _datStream.Position = currentBlock + 4;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
                    await _datStream.WriteAsync(buffer.AsMemory(bufferIndex, size), ct);
#else
                    await _datStream.WriteAsync(buffer, bufferIndex, size, ct);
#endif

                    bufferIndex += size;

                    // Prepare next block pointer
                    var oldOffset = currentBlock;
                    if (bufferIndex < numBytes) {
                        _datStream.Position = currentBlock;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
                        await _datStream.ReadAsync(nextBlockBuffer.AsMemory(0, 4), ct);
#else
                         await _datStream.ReadAsync(nextBlockBuffer, 0, 4, ct);
#endif
                        var nextBlock = BinaryPrimitives.ReadInt32LittleEndian(nextBlockBuffer);

                        if (nextBlock <= 0) {
                            currentBlock = ReserveBlockCore();
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
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
                    await _datStream.WriteAsync(nextBlockBuffer.AsMemory(0, 4), ct);
#else
                    await _datStream.WriteAsync(nextBlockBuffer, 0, 4, ct);
#endif
                }

                await _datStream.FlushAsync(ct);
            }
            finally {
                _streamLock.Release();
            }

            WriteHeader();
            return startingBlock;
        }

        /// <inheritdoc/>
        public override void ReadBytes(byte[] buffer, int bufferOffset, int byteOffset, int numBytes) {
            _streamLock.Wait();
            try {
                _datStream.Position = byteOffset;
#if NET7_0_OR_GREATER
                _datStream.ReadExactly(buffer, bufferOffset, numBytes);
#else
                int totalRead = 0;
                while (totalRead < numBytes) {
                    var read = _datStream.Read(buffer, bufferOffset + totalRead, numBytes - totalRead);
                    if (read == 0) throw new EndOfStreamException();
                    totalRead += read;
                }
#endif
            }
            finally {
                _streamLock.Release();
            }
        }

        /// <inheritdoc/>
        public override void ReadBlock(byte[] buffer, int startingBlock) {
            var nextBlockBuffer = new byte[4];
            var currentBlock = startingBlock;
            var totalRead = 0;

            _streamLock.Wait();
            try {
                while (currentBlock != 0 && totalRead < buffer.Length) {
                    var bytesToRead = Math.Min(Header.BlockSize - 4, buffer.Length - totalRead);

                    // Read block data
                    _datStream.Position = currentBlock + 4;
#if NET7_0_OR_GREATER
                    _datStream.ReadExactly(buffer, totalRead, bytesToRead);
#else
                    int blockRead = 0;
                    while (blockRead < bytesToRead) {
                        var read = _datStream.Read(buffer, totalRead + blockRead, bytesToRead - blockRead);
                        if (read == 0) throw new EndOfStreamException();
                        blockRead += read;
                    }
#endif
                    totalRead += bytesToRead;

                    if (totalRead >= buffer.Length) {
                        return;
                    }

                    // Get next block pointer
                    _datStream.Position = currentBlock;
#if NET7_0_OR_GREATER
                    _datStream.ReadExactly(nextBlockBuffer, 0, 4);
#else
                    int ptrRead = 0;
                    while (ptrRead < 4) {
                        var read = _datStream.Read(nextBlockBuffer, ptrRead, 4 - ptrRead);
                        if (read == 0) throw new EndOfStreamException();
                        ptrRead += read;
                    }
#endif
                    currentBlock = BinaryPrimitives.ReadInt32LittleEndian(nextBlockBuffer);
                }
            }
            finally {
                _streamLock.Release();
            }
        }

        /// <inheritdoc/>
        public override async ValueTask
            ReadBlockAsync(byte[] buffer, int startingBlock, CancellationToken ct = default) {
            var nextBlockBuffer = new byte[4];
            var currentBlock = startingBlock;
            var totalRead = 0;

            await _streamLock.WaitAsync(ct);
            try {
                while (currentBlock != 0 && totalRead < buffer.Length) {
                    var bytesToRead = Math.Min(Header.BlockSize - 4, buffer.Length - totalRead);

                    // Read block data
                    _datStream.Position = currentBlock + 4;

#if NET7_0_OR_GREATER
                    await _datStream.ReadExactlyAsync(buffer.AsMemory(totalRead, bytesToRead), ct);
#else
                    int blockRead = 0;
                    while (blockRead < bytesToRead) {
                        var read =
 await _datStream.ReadAsync(buffer, totalRead + blockRead, bytesToRead - blockRead, ct);
                        if (read == 0) throw new EndOfStreamException();
                        blockRead += read;
                    }
#endif
                    totalRead += bytesToRead;

                    if (totalRead >= buffer.Length) {
                        return;
                    }

                    // Get next block pointer
                    _datStream.Position = currentBlock;
#if NET7_0_OR_GREATER
                    await _datStream.ReadExactlyAsync(nextBlockBuffer.AsMemory(0, 4), ct);
#else
                    int ptrRead = 0;
                    while (ptrRead < 4) {
                        var read = await _datStream.ReadAsync(nextBlockBuffer, ptrRead, 4 - ptrRead, ct);
                        if (read == 0) throw new EndOfStreamException();
                        ptrRead += read;
                    }
#endif
                    currentBlock = BinaryPrimitives.ReadInt32LittleEndian(nextBlockBuffer);
                }
            }
            finally {
                _streamLock.Release();
            }
        }

        /// <inheritdoc/>
        public override bool TryGetBlockOffsets(int startingBlock, out List<int> fileBlocks) {
            fileBlocks = new List<int>();
            var nextBlockBuffer = new byte[4];
            var currentBlock = startingBlock;

            _streamLock.Wait();
            try {
                while (currentBlock != 0) {
                    fileBlocks.Add(currentBlock);

                    _datStream.Position = currentBlock;
#if NET7_0_OR_GREATER
                    _datStream.ReadExactly(nextBlockBuffer, 0, 4);
#else
                    int ptrRead = 0;
                    while (ptrRead < 4) {
                        var read = _datStream.Read(nextBlockBuffer, ptrRead, 4 - ptrRead);
                        if (read == 0) throw new EndOfStreamException();
                        ptrRead += read;
                    }
#endif
                    currentBlock = BinaryPrimitives.ReadInt32LittleEndian(nextBlockBuffer);
                }
            }
            finally {
                _streamLock.Release();
            }

            return true;
        }

        /// <inheritdoc/>
        protected override void Expand(int newSizeInBytes) {
            if (newSizeInBytes <= _datStream.Length) {
                throw new Exception(
                    $"Tried to shrink the database with Expand({newSizeInBytes:N0} bytes) when dat file is already {_datStream.Length:N0} bytes. Can't shrink a database without rewriting!");
            }

            // Lock is expected to be held by caller (ReserveBlock -> AllocateEmptyBlocks)
            _datStream.SetLength(newSizeInBytes);
            Header.FileSize = newSizeInBytes;
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
                _streamLock.Dispose();
            }
        }
    }
}