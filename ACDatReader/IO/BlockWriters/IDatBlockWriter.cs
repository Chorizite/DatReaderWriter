using System;

namespace ACDatReader.IO.BlockWriters {
    /// <summary>
    /// A dat writer that can write a byte buffer directly to a contiguous chunk in the dat,
    /// or by filling in blocks
    /// </summary>
    public interface IDatBlockWriter : IDisposable {
        /// <summary>
        /// Writes a contiguous chunk of data into a dat file.
        /// This does not follow blocks.
        /// </summary>
        /// <param name="buffer">The buffer data to write</param>
        /// <param name="blockOffset">The offset in the dat file</param>
        void WriteBytes(byte[] buffer, uint blockOffset);

        /// <summary>
        /// Write block contents starting at <paramref name="startingBlock"/>, until it writes
        /// the contents of <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">The buffer data to write</param>
        /// <param name="startingBlock">The offset of the first block to write to in the dat file</param>
        /// <param name="blockSize">The size of a block, defined in <see cref="DatHeader.BlockSize"/></param>
        /// <param name="blockProvider"></param>
        void WriteBlocks(byte[] buffer, uint startingBlock, int blockSize, IDatBlockAllocator blockProvider);
    }
}