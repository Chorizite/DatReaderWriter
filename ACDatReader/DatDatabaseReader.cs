using ACDatReader.IO;
using ACDatReader.IO.BlockAllocators;
using ACDatReader.Options;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ACDatReader {
    /// <summary>
    /// Provides read access to a dat database
    /// </summary>
    public class DatDatabaseReader : IDisposable {
        private readonly IDatBlockAllocator _blockAllocator;

        /// <summary>
        /// Database Options
        /// </summary>
        public DatDatabaseOptions Options { get; }

        /// <summary>
        /// Dat header
        /// </summary>
        public DatHeader Header => _blockAllocator.Header;

        /// <summary>
        /// Create a new DatDatabase
        /// </summary>
        /// <param name="options">Options configuration action</param>
        /// <param name="blockAllocator">Block allocator instance to use</param>
        public DatDatabaseReader(Action<DatDatabaseOptions>? options = null, IDatBlockAllocator? blockAllocator = null) {
            Options = new DatDatabaseOptions();
            options?.Invoke(Options);

            _blockAllocator = blockAllocator ?? new MemoryMappedBlockAllocator(Options);
        }

        /// <summary>
        /// Try to get the byte contents of a file
        /// </summary>
        /// <param name="fileId">The id of the file to get</param>
        /// <param name="bytes">The bytes that were found, if any.</param>
        /// <returns>True if the file bytes were found, false otherwise</returns>
        public bool TryGetFileBytes(uint fileId, out byte[]? bytes) {
            bytes = null;
            return false;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Dispose
        /// </summary>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                _blockAllocator?.Dispose();
            }
        }
    }
}