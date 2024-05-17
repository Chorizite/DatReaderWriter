using ACClientLIb.DatReaderWriter.IO;
using ACClientLIb.DatReaderWriter.IO.BlockAllocators;
using ACClientLIb.DatReaderWriter.IO.DatBTree;
using ACClientLIb.DatReaderWriter.Options;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ACClientLIb.DatReaderWriter {
    /// <summary>
    /// Provides read access to a dat database
    /// </summary>
    public class DatDatabaseReader : IDisposable {
        private readonly IDatBlockAllocator _blockAllocator;
        private readonly DatBTreeReaderWriter _tree;

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
            _tree = new DatBTreeReaderWriter(_blockAllocator);
        }

        /// <summary>
        /// Get the raw bytes of a file entry
        /// </summary>
        /// <param name="fileId">The id of the file to get</param>
        /// <param name="bytes">The raw bytes</param>
        /// <returns>True if the file was found, false otherwise</returns>
        public bool TryGetFileBytes(uint fileId, [MaybeNullWhen(false)] out byte[] bytes) {
            if (_tree.TryGetFile(fileId, out var fileEntry)) {
                bytes = new byte[fileEntry.Size];
                _blockAllocator.ReadBlock(bytes, fileEntry.Offset);
                return true;
            }

            bytes = null;
            return false;
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