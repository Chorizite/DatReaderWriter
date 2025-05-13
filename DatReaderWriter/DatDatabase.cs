using DatReaderWriter.DBObjs;
using DatReaderWriter.Enums;
using DatReaderWriter.Lib;
using DatReaderWriter.Options;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Lib.IO.BlockAllocators;
using DatReaderWriter.Lib.IO.DatBTree;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DatReaderWriter {
    /// <summary>
    /// Provides read access to a dat database
    /// </summary>
    public partial class DatDatabase : IDisposable {
        private Dictionary<uint, IDBObj> _fileCache = [];

        /// <summary>
        /// Block allocator
        /// </summary>
        public readonly IDatBlockAllocator BlockAllocator;

        /// <summary>
        /// Binary Tree
        /// </summary>
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
        /// Iteration data
        /// </summary>
        public Iteration Iteration { get; }

        /// <summary>
        /// The dat collection this dat database belongs to, if any
        /// </summary>
        public DatCollection? DatCollection { get; internal set; }

        /// <summary>
        /// Create a new DatDatabase
        /// </summary>
        /// <param name="options">Options configuration action</param>
        /// <param name="blockAllocator">Block allocator instance to use</param>
        public DatDatabase(Action<DatDatabaseOptions>? options = null, IDatBlockAllocator? blockAllocator = null) {
            Options = new DatDatabaseOptions();
            options?.Invoke(Options);

            // use StreamBlockAllocator on x86, to avoid memory mapping using all the address space
            if (IntPtr.Size == 4) {
                BlockAllocator = blockAllocator ?? new StreamBlockAllocator(Options);
            }
            else {
                BlockAllocator = blockAllocator ?? new MemoryMappedBlockAllocator(Options);
            }

            Tree = new DatBTreeReaderWriter(BlockAllocator);

            if (TryReadFile<Iteration>(0xFFFF0001, out var iteration)) {
                Iteration = iteration;

                if (BlockAllocator.CanWrite && Iteration.Iterations.Count > 1) {
                    throw new NotSupportedException("Can't write to a database with multiple iterations");
                }
            }
            else {
                Iteration = new Iteration();
                Iteration.CurrentIteration = 1;
                Iteration.Iterations.Add(1, -1);
            }
        }

        /// <summary>
        /// Clear the file cache, only applicable if <see cref="DatCollectionOptions.FileCachingStrategy"/>
        /// is <see cref="FileCachingStrategy.OnDemand"/>
        /// </summary>
        public void ClearCache() {
            _fileCache.Clear();
        }

        /// <summary>
        /// Get an enumerable of all existing ids of a specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public IEnumerable<uint> GetAllIdsOfType<T>() where T : IDBObj {
            if (DBObjAttributeCache.TypeCache.TryGetValue(typeof(T), out var typeAttr)) {
                if (typeAttr.IsSingular) {
                    return [ typeAttr.FirstId ];
                }
                return Tree.GetFilesInRange(typeAttr.FirstId, typeAttr.LastId).Select(f => f.Id);
            }

            return Enumerable.Empty<uint>();
        }

        /// <summary>
        /// Get the type of a DBObj based on its id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DBObjType TypeFromId(uint id) {
            return DBObjAttributeCache.DBObjTypeFromId(Header.Type, id);
        }

        /// <summary>
        /// Get the raw bytes of a file entry, this is never cached.
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
        /// Read a dat file, and caches it regardless of <see cref="DatCollectionOptions.FileCachingStrategy"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public T? GetCached<T>(uint fileId) where T : IDBObj {
            if (_fileCache.TryGetValue(fileId, out var cached) && cached is T t) {
                return t;
            }

            TryReadFile<T>(fileId, out var value);
            
            if (value is not null && !_fileCache.ContainsKey(fileId)) {
                _fileCache.Add(fileId, value);
            }
            
            return value;
        }

        /// <summary>
        /// Read a dat file, returns null if the file is not found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public T? Get<T>(uint fileId) where T : IDBObj {
            TryReadFile<T>(fileId, out var value);
            return value;
        }

        /// <summary>
        /// Read a dat file. This will be cached according to the <see cref="DatCollectionOptions.FileCachingStrategy"/> in use
        /// </summary>
        /// <typeparam name="T">The dat file type</typeparam>
        /// <param name="fileId">The id of the file to get</param>
        /// <param name="value">The unpacked file</param>
        /// <returns></returns>
#if (NET8_0_OR_GREATER)
        public bool TryReadFile<T>(uint fileId, [MaybeNullWhen(false)] out T value) where T : IDBObj {
#else
        public bool TryReadFile<T>(uint fileId, out T value) where T : IDBObj {
#endif
            if (Options.FileCachingStrategy == FileCachingStrategy.OnDemand) {
                if (_fileCache.TryGetValue(fileId, out var cached) && cached is T t) {
                    value = t;
                    return true;
                }
            }

            if (!TryGetFileBytes(fileId, out var bytes)) {
                value = default!;
                return false;
            }

            value = (T)Activator.CreateInstance(typeof(T));

            if (!value.Unpack(new DatBinReader(bytes, this))) {
                value = default!;
                return false;
            }

            if (Options.FileCachingStrategy == FileCachingStrategy.OnDemand) {
                _fileCache.Add(fileId, value);
            }

            return true;
        }

        /// <summary>
        /// Try and write a <see cref="IDBObj"/> to the dat.
        /// </summary>
        /// <param name="value">The value to write</param>
        /// <param name="iteration">The iteration to use. If none is passed, it will use the current files iteration if available, otherwise it will use the current dat iteration.</param>
        public Result<T, string> TryWriteFile<T>(T value, int? iteration = null) where T : IDBObj {
            if (!BlockAllocator.CanWrite) {
                return "Block allocator was opened as read only.";
            }

            int startingBlockId = 0;
            if (Tree.TryGetFile(value.Id, out var existingFile)) {
                startingBlockId = existingFile.Offset;
            }

            // TODO: fix this static 5mb buffer...?
            // we dont know how big the file will be, so we need to make sure we have enough space
            var buffer = BaseBlockAllocator.SharedBytes.Rent(1024 * 1024 * 5);
            var writer = new DatBinWriter(buffer, this);

            value.Pack(writer);
            startingBlockId = Tree.BlockAllocator.WriteBlock(buffer, writer.Offset, startingBlockId);

            var newIteration = iteration.HasValue ? iteration.Value : (existingFile?.Iteration ?? 0);
            var newEntry = new DatBTreeFile() {
                Flags = existingFile?.Flags ?? 0u,
                Id = value.Id,
                Size = (uint)writer.Offset,
                Offset = startingBlockId,
                Date = DateTime.UtcNow,
                Iteration = newIteration
            };
            var oldEntry = Tree.Insert(newEntry);

            // update dat iteration if needed
            if (newIteration > Iteration.CurrentIteration) {
                Iteration.CurrentIteration = newIteration;
                var iterationUpdateRes = TryWriteFile(Iteration);
                if (!iterationUpdateRes) {
                    return iterationUpdateRes.Error ?? "Failed to update dat iteration.";
                }
            }

            BaseBlockAllocator.SharedBytes.Return(buffer);

            if (_fileCache.ContainsKey(value.Id)) {
                _fileCache[value.Id] = value;
            }

            return value;
        }

        /// <inheritdoc/>
        public void Dispose() {
            ClearCache();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                Tree?.Dispose();
                BlockAllocator?.Dispose();
            }
        }
    }
}