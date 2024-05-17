using ACClientLIb.DatReaderWriter.Options;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace ACClientLIb.DatReaderWriter.IO.BlockAllocators {
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
        public void InitNew(DatDatabaseType type, uint subset, int blockSize = 1024, int numBlocksToAllocate = 1024) {
            if (!CanWrite) {
                throw new Exception($"Attempted to write a new database, but MemoryMappedBlockAllocator.CanWrite is false");
            }

            Header.Type = type;
            Header.SubSet = subset;
            Header.BlockSize = blockSize;
            Header.RootBlock = 0;
            Header.FileSize = DatHeader.SIZE;
            Header.Magic = DatHeader.RETAIL_MAGIC;

            if (numBlocksToAllocate > 0) {
                AllocateEmptyBlocks(numBlocksToAllocate);
            }

            Header.WriteEmptyTransaction();
            WriteHeader();

            HasHeaderData = true;
        }

        /// <inheritdoc/>
        public void SetVersion(string version, int engineVersion, int gameVersion, Guid majorVersion, uint minorVersion) {
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
            Header.Unpack(new DatFileReader(buffer));
            SharedBytes.Return(buffer);

            if (Header.Magic == DatHeader.RETAIL_MAGIC) {
                HasHeaderData = true;
            }
        }

        /// <inheritdoc/>
        public void AllocateEmptyBlocks(int numBlocksToAllocate) {
            if (Header.FirstFreeBlock == 0 && Header.LastFreeBlock == 0) {
                // new dat. start creating blocks directly after the dat header
                // but aligned to Header.BlockSize
                var firstBlockOffset = (int)Math.Ceiling((double)DatHeader.SIZE / Header.BlockSize) * Header.BlockSize;

                Header.FreeBlockCount = 0;
                Header.FirstFreeBlock = firstBlockOffset;
                Header.LastFreeBlock = firstBlockOffset;
                Header.FileSize = firstBlockOffset;
            }

            var offset = Header.FileSize;
            Expand(Header.FileSize + (numBlocksToAllocate * Header.BlockSize));
            
            if (Header.FreeBlockCount == 0) {
                Header.FirstFreeBlock = offset;
            }
            Header.LastFreeBlock = Header.FileSize - Header.BlockSize;
            Header.FreeBlockCount += numBlocksToAllocate;

            WriteHeader();
        }

        /// <inheritdoc/>
        public int ReserveBlock() {
            if (Header.FreeBlockCount > 0) {
                var freeBlockOffset = Header.FirstFreeBlock;
                Header.FirstFreeBlock += Header.BlockSize;
                Header.FreeBlockCount--;

                WriteHeader();

                return freeBlockOffset;
            }
            else {
                // todo: we should maybe expand by num bytes or something instead
                // of block size?
                AllocateEmptyBlocks(2048);
                return ReserveBlock();
            }
        }

        /// <summary>
        /// Write the header to the dat
        /// </summary>
        protected void WriteHeader() {
            var headerBuffer = SharedBytes.Rent(DatHeader.SIZE);
            Header.Pack(new DatFileWriter(headerBuffer));
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

        /// <summary>
        /// Expand the dat file size.
        /// </summary>
        /// <param name="newSizeInBytes">The new filesize in bytes</param>
        protected abstract void Expand(int newSizeInBytes);

        /// <inheritdoc/>
        public abstract void Dispose();
    }
}
