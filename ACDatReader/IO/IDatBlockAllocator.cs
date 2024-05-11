namespace ACDatReader.IO {
    /// <summary>
    /// Block allocator for a dat. Used to request / return blocks from/to the dat.
    /// </summary>
    public interface IDatBlockAllocator {
        /// <summary>
        /// Request an unused block offset from the pool, and mark it as used.
        /// </summary>
        /// <returns>The offset of an unused block</returns>
        public uint Request();

        /// <summary>
        /// Return a block offset back to the pool, and mark it as unused.
        /// </summary>
        /// <param name="blockOffset">The block offset to return</param>
        public void Return(uint blockOffset);
    }
}