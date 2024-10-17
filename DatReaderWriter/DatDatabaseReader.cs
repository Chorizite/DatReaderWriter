using ACClientLib.DatReaderWriter.IO;
using ACClientLib.DatReaderWriter.IO.BlockAllocators;
using ACClientLib.DatReaderWriter.IO.DatBTree;
using ACClientLib.DatReaderWriter.Options;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ACClientLib.DatReaderWriter {
    /// <summary>
    /// Provides read access to a dat database
    /// </summary>
    public class DatDatabaseReader : IDisposable {
        public readonly IDatBlockAllocator BlockAllocator;
        public readonly DatBTreeReaderWriter Tree;

        /// <summary>
        /// Database Options
        /// </summary>
        public DatDatabaseOptions Options { get; }

        /// <summary>
        /// Dat header
        /// </summary>
        public DatHeader Header => BlockAllocator.Header;

        /// <summary>
        /// Create a new DatDatabase
        /// </summary>
        /// <param name="options">Options configuration action</param>
        /// <param name="blockAllocator">Block allocator instance to use</param>
        public DatDatabaseReader(Action<DatDatabaseOptions>? options = null, IDatBlockAllocator? blockAllocator = null) {
            Options = new DatDatabaseOptions();
            options?.Invoke(Options);

            BlockAllocator = blockAllocator ?? new MemoryMappedBlockAllocator(Options);
            Tree = new DatBTreeReaderWriter(BlockAllocator);
        }

        /// <summary>
        /// Get the raw bytes of a file entry
        /// </summary>
        /// <param name="fileId">The id of the file to get</param>
        /// <param name="bytes">The raw bytes</param>
        /// <returns>True if the file was found, false otherwise</returns>
#if (NET8_0_OR_GREATER)
            public bool TryGetFileBytes(uint fileId, [MaybeNullWhen(false)] out byte[] bytes) {
#else
        public bool TryGetFileBytes(uint fileId, out byte[] bytes) {
#endif
            if (Tree.TryGetFile(fileId, out var fileEntry)) {
                bytes = new byte[fileEntry.Size];
                BlockAllocator.ReadBlock(bytes, fileEntry.Offset);
                return true;
            }

            bytes = null!;
            return false;
        }

        /// <summary>
        /// Read a dat file
        /// </summary>
        /// <typeparam name="T">The dat file type</typeparam>
        /// <param name="fileId">The id of the file to get</param>
        /// <param name="value">The unpacked file</param>
        /// <returns></returns>
#if (NET8_0_OR_GREATER)
            public bool TryReadFile<T>(uint fileId, [MaybeNullWhen(false)] out T value) where T : IUnpackable {
#else
        public bool TryReadFile<T>(uint fileId, out T value) where T : IDBObj {
#endif
            if (!TryGetFileBytes(fileId, out var bytes)) {
                value = default!;
                return false;
            }

            value = (T)Activator.CreateInstance(typeof(T));

            if (!value.Unpack(new DatFileReader(bytes))) {
                value = default!;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Try and write a <see cref="IDBObj"/> to the dat.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value to write</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool TryWriteFile<T>(T value) where T : IDBObj {
            int startingBlockId = 0;
            if (Tree.TryGetFile(value.Id, out var existingFile)) {
                startingBlockId = existingFile.Offset;
            }

            // TODO: fix this static 5mb buffer...
            var buffer = BaseBlockAllocator.SharedBytes.Rent(1024 * 1024 * 5);
            var writer = new DatFileWriter(buffer);

            value.Pack(writer);

            startingBlockId = Tree.BlockAllocator.WriteBlock(buffer, writer.Offset);
            Tree.Insert(new DatBTreeFile() {
                Flags = 0,
                Id = value.Id,
                Size = (uint)buffer.Length,
                Offset = startingBlockId
            });

            BaseBlockAllocator.SharedBytes.Return(buffer);

            return true;
        }

        /// <inheritdoc/>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                Tree?.Dispose();
            }
        }
    }
}