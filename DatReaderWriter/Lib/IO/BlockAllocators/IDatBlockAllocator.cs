using DatReaderWriter.Enums;
using DatReaderWriter.Lib.IO;
using System;
using System.Collections.Generic;

namespace DatReaderWriter.Lib.IO.BlockAllocators {
    /// <summary>
    /// A dat block allocator. Responsible for managing dat blocks and keeping
    /// the backing dat header up to date as things change. Used for reading and 
    /// writing.
    /// </summary>
    public interface IDatBlockAllocator : IDisposable {
        /// <summary>
        /// True if this was created with write access.
        /// </summary>
        bool CanWrite { get; }

        /// <summary>
        /// Has header data, either from reading from an existing data or from using
        /// <see cref="InitNew(DatFileType, uint, int, int)"/>
        /// </summary>
        bool HasHeaderData { get; }

        /// <summary>
        /// The header data. Only valid if <see cref="HasHeaderData"/> is <see langword="true"/>
        /// </summary>
        DatHeader Header { get; }

        /// <summary>
        /// Initialize a database by writing the base header data and allocating some unused blocks
        /// </summary>
        /// <param name="type">The type of database</param>
        /// <param name="subset">The database sub type</param>
        /// <param name="blockSize">block size to use</param>
        /// <param name="numBlocksToAllocate">number of empty blocks to allocate</param>
        void InitNew(DatFileType type, uint subset, int blockSize = 1024, int numBlocksToAllocate = 1024);

        /// <summary>
        /// Set the version headers
        /// </summary>
        /// <param name="version">A version string, less than 256 characters in length. ASCII only. Does not appear to be used in the retail dats</param>
        /// <param name="engineVersion">Engine version</param>
        /// <param name="gameVersion">Game version</param>
        /// <param name="majorVersion">Major version</param>
        /// <param name="minorVersion">Minor version</param>
        void SetVersion(string version, int engineVersion, int gameVersion, Guid majorVersion, uint minorVersion);

        /// <summary>
        /// Write bytes from the buffer directly to the dat, without following blocks.
        /// <see cref="CanWrite"/> must be true.
        /// </summary>
        /// <param name="buffer">The buffer to write from</param>
        /// <param name="byteOffset">The byte offset in the dat to write to</param>
        /// <param name="numBytes">The number of bytes to write</param>
        void WriteBytes(byte[] buffer, int byteOffset, int numBytes);

        /// <summary>
        /// Write a buffers contents to unused block space, optionally
        /// providing a starting block. If the provided starting block is
        /// &lt;= 0 (default) a new block will be found or allocated automatically.
        /// </summary>
        /// <param name="buffer">The buffer to write</param>
        /// <param name="numBytes">The number of bytes to write</param>
        /// <param name="startingBlock">The starting block to use, or -1 to find an empty block</param>
        /// <returns>The offset of the newly written starting block</returns>
        int WriteBlock(byte[] buffer, int numBytes, int startingBlock = 0);

        /// <summary>
        /// Fills a buffer with contiguous data from a dat file.
        /// </summary>
        /// <param name="buffer">The buffer to read into</param>
        /// <param name="bufferOffset">The offset into the buffer to start writing</param>
        /// <param name="datOffset">The starting byte offset in the dat file to read from</param>
        /// <param name="numBytes">The number of bytes to read</param>
        void ReadBytes(byte[] buffer, int bufferOffset, int datOffset, int numBytes);

        /// <summary>
        /// Read a block's data into a buffer
        /// </summary>
        /// <param name="buffer">The buffer to read into</param>
        /// <param name="startingBlock">The starting block offset</param>
        void ReadBlock(byte[] buffer, int startingBlock);

        /// <summary>
        /// Get a list of all offsets that a block is using
        /// </summary>
        /// <param name="startingBlock">The block offset to start with</param>
        /// <param name="fileBlocks">The block offsets contained in this block, including <paramref name="startingBlock"/></param>
        /// <returns>True if the starting block was found, false otherwise (out of bounds, probably)</returns>
        bool TryGetBlockOffsets(int startingBlock, out List<int> fileBlocks);

        /// <summary>
        /// Allocate empty blocks in the dat file.
        /// <see cref="CanWrite"/> must be true.
        /// </summary>
        /// <param name="numBlocksToAllocate"></param>
        void AllocateEmptyBlocks(int numBlocksToAllocate);

        /// <summary>
        /// Reserve a block from the empty block pool. This will resize the dat
        /// as neccesary to allocate new blocks.
        /// </summary>
        /// <returns>The offset of the newly reserved block</returns>
        public int ReserveBlock();

        /// <summary>
        /// Set a new root node for this dat
        /// </summary>
        /// <param name="offset">The offset in the dat of the new root node</param>
        void SetRootBlock(int offset);
    }
}