using System;

namespace ACDatReader.IO {
    /// <summary>
    /// A dat reader that can read into buffers, either in contiguous chunks or by
    /// following blocks.
    /// </summary>
    public interface IDatBlockReader : IDisposable {
        /// <summary>
        /// Read a contiguous chunk of data into a buffer.
        /// This does not follow blocks.
        /// </summary>
        /// <param name="buffer">The buffer to read into</param>
        /// <param name="blockOffset">The offset in the dat file</param>
        /// <param name="numBytes">The number of bytes to read</param>
        void ReadBytes(ref byte[] buffer, uint blockOffset, int numBytes);

        /// <summary>
        /// Reads block contents starting at <paramref name="startingBlock"/>, until
        /// either runs out of valid blocks to read, or files the contents of <paramref name="buffer"/>
        /// </summary>
        /// <param name="buffer">The buffer to read into</param>
        /// <param name="startingBlock">The offset of the block in the dat file</param>
        /// <param name="blockSize">The size of a block, defined in <see cref="DatHeader.BlockSize"/></param>
        void ReadBlocks(ref byte[] buffer, uint startingBlock, int blockSize);
    }
}