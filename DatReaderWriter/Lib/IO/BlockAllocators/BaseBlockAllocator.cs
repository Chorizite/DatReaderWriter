using DatReaderWriter.Enums;
using DatReaderWriter.Options;
using DatReaderWriter.Lib.IO;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DatReaderWriter.Lib.IO.BlockAllocators {
    /// <summary>
    /// Base block allocator class.
    /// </summary>
    public abstract class BaseBlockAllocator : IDatBlockAllocator {
        internal readonly static ArrayPool<byte> SharedBytes = ArrayPool<byte>.Shared;

        /// <summary>
        /// The options being used for this allocator
        /// </summary>
        protected DatDatabaseOptions Options;

        /// <inheritdoc/>
        public DatHeader Header { get; } = new();

        /// <inheritdoc/>
        public bool CanWrite => Options.AccessType == DatAccessType.ReadWrite;

        /// <inheritdoc/>
        public bool HasHeaderData { get; protected set; } = false;

        /// <summary>
        /// Create a new allocator and setup options.
        /// </summary>
        /// <param name="options">The options to use</param>
        protected BaseBlockAllocator(DatDatabaseOptions options) {
            Options = options;
        }

        /// <inheritdoc/>
        public void InitNew(DatFileType type, uint subset, int blockSize = 1024, int numBlocksToAllocate = 1024) {
            if (!CanWrite) {
                throw new Exception(
                    $"Attempted to write a new database, but MemoryMappedBlockAllocator.CanWrite is false");
            }

            var headerBlockCount = (int)Math.Ceiling((double)DatHeader.SIZE / blockSize);

            Header.Type = type;
            Header.SubSet = subset;
            Header.BlockSize = blockSize;
            Header.RootBlock = 0;
            Header.FileSize = headerBlockCount * blockSize;
            Header.Magic = DatHeader.RETAIL_MAGIC;

            if (numBlocksToAllocate > 0) {
                AllocateEmptyBlocks(numBlocksToAllocate);
            }
            else {
                Expand(headerBlockCount * blockSize);
            }

            WriteHeader();

            HasHeaderData = true;
        }

        /// <inheritdoc/>
        public void SetVersion(string version, int engineVersion, int gameVersion, Guid majorVersion,
            uint minorVersion) {
            if (version.Length > 255) {
                throw new Exception($"Version string can only be 255 characters max (was {version.Length} characters)");
            }

            Header.Version = version;
            Header.EngineVersion = engineVersion;
            Header.GameVersion = gameVersion;
            Header.MajorVersion = majorVersion;
            Header.MinorVersion = minorVersion;

            WriteHeader();
        }

        /// <summary>
        /// Try and read header data from existing file
        /// </summary>
        protected void TryReadHeader() {
            var buffer = SharedBytes.Rent(DatHeader.SIZE);
            ReadBytes(buffer, 0, 0, DatHeader.SIZE);
            Header.Unpack(new DatBinReader(buffer));
            SharedBytes.Return(buffer);

            if (Header.Magic == DatHeader.RETAIL_MAGIC) {
                HasHeaderData = true;
            }
        }

        /// <inheritdoc/>
        public void AllocateEmptyBlocks(int numBlocksToAllocate) {
            if (numBlocksToAllocate <= 0) return;

            if (Header.FirstFreeBlock == 0 && Header.LastFreeBlock == 0) {
                // new dat. start creating blocks directly after the dat header
                // but aligned to Header.BlockSize
                var firstBlockOffset = (int)Math.Ceiling((double)DatHeader.SIZE / Header.BlockSize) * Header.BlockSize;

                Header.FreeBlockCount = 0;
                Header.FirstFreeBlock = firstBlockOffset;
                Header.LastFreeBlock = firstBlockOffset;
                Header.FileSize = firstBlockOffset;
            }

            var hadFreeBlocks = Header.FreeBlockCount > 0;
            var oldLastFreeBlock = Header.LastFreeBlock;
            var chainHead = Header.FileSize;

            Expand(Header.FileSize + numBlocksToAllocate * Header.BlockSize);

            // Write in-band next-pointers for each newly appended block.
            // Format: (nextOffset | 0x80000000); the last block uses 0x80000000 (next=0, free marker).
            var ptrBuf = SharedBytes.Rent(4);
            for (int i = 0; i < numBlocksToAllocate; i++) {
                var blockOffset = chainHead + i * Header.BlockSize;
                int nextPtr;
                if (i < numBlocksToAllocate - 1) {
                    nextPtr = (chainHead + (i + 1) * Header.BlockSize) | unchecked((int)0x80000000);
                }
                else {
                    nextPtr = unchecked((int)0x80000000); // end-of-chain: next=0, free marker
                }
                BinaryPrimitives.WriteInt32LittleEndian(ptrBuf, nextPtr);
                WriteBytes(ptrBuf, blockOffset, 4);
            }

            if (hadFreeBlocks) {
                // Link the old tail of the free list to the new chain's head.
                int linkPtr = chainHead | unchecked((int)0x80000000);
                BinaryPrimitives.WriteInt32LittleEndian(ptrBuf, linkPtr);
                WriteBytes(ptrBuf, oldLastFreeBlock, 4);
            }
            else {
                Header.FirstFreeBlock = chainHead;
            }

            SharedBytes.Return(ptrBuf);

            Header.LastFreeBlock = chainHead + (numBlocksToAllocate - 1) * Header.BlockSize;
            Header.FreeBlockCount += numBlocksToAllocate;

            WriteHeader();
        }

        /// <inheritdoc/>
        /// <inheritdoc/>
        public virtual int ReserveBlock() {
            return ReserveBlockCore();
        }

        /// <summary>
        /// Protected internal implementation of ReserveBlock without locking (or assumes lock held).
        /// </summary>
        protected int ReserveBlockCore() {
            if (Header.FreeBlockCount > 0) {
                var freeBlockOffset = Header.FirstFreeBlock;

                // Read the in-band next-pointer from the first 4 bytes of the free block.
                // Format: (nextOffset | 0x80000000); strip the high free-marker bit.
                var ptrBuf = SharedBytes.Rent(4);
                ReadBytes(ptrBuf, 0, freeBlockOffset, 4);
                var next = BinaryPrimitives.ReadInt32LittleEndian(ptrBuf) & 0x7FFFFFFF;
                SharedBytes.Return(ptrBuf);

                Header.FirstFreeBlock = next;
                Header.FreeBlockCount--;

                WriteHeader();

                return freeBlockOffset;
            }
            else {
                AllocateEmptyBlocks(50);
                return ReserveBlockCore();
            }
        }

        /// <summary>
        /// Write the header to the dat
        /// </summary>
        protected virtual void WriteHeader() {
            var headerBuffer = SharedBytes.Rent(DatHeader.SIZE);
            Header.WriteEmptyTransaction();
            Header.Pack(new DatBinWriter(headerBuffer));
            WriteBytes(headerBuffer, 0, DatHeader.SIZE);
            SharedBytes.Return(headerBuffer);
        }

        /// <inheritdoc/>
        public void SetRootBlock(int offset) {
            Header.RootBlock = offset;
            WriteHeader();
        }

        /// <inheritdoc/>
        public abstract void WriteBytes(byte[] buffer, int byteOffset, int numBytes);

        /// <inheritdoc/>
        public abstract int WriteBlock(byte[] buffer, int numBytes, int startingBlock = -1);


        /// <inheritdoc/>
        public abstract void ReadBytes(byte[] buffer, int bufferOffset, int byteOffset, int numBytes);

        /// <inheritdoc/>
        public abstract void ReadBlock(byte[] buffer, int startingBlock);

        /// <inheritdoc/>
        public abstract ValueTask<int> WriteBlockAsync(byte[] buffer, int numBytes, int startingBlock = 0,
            CancellationToken ct = default);

        /// <inheritdoc/>
        public abstract ValueTask ReadBlockAsync(byte[] buffer, int startingBlock, CancellationToken ct = default);

        /// <inheritdoc/>
        public abstract bool TryGetBlockOffsets(int startingBlock, out List<int> fileBlocks);

        /// <summary>
        /// Expand the dat file size.
        /// </summary>
        /// <param name="newSizeInBytes">The new filesize in bytes</param>
        protected abstract void Expand(int newSizeInBytes);

        /// <inheritdoc/>
        public abstract void Dispose();
    }
}
