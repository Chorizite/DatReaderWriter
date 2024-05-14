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

        /// <inheritdoc/>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                _blockAllocator?.Dispose();
            }
        }
    }
}