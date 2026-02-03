using DatReaderWriter.DBObjs;
using DatReaderWriter.Enums;
using DatReaderWriter.Lib;
using DatReaderWriter.Options;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Lib.IO.BlockAllocators;
using DatReaderWriter.Lib.IO.DatBTree;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#if NET8_0_OR_GREATER
using System.IO.Compression;
#endif

namespace DatReaderWriter {
    /// <summary>
    /// Provides read access to a dat database
    /// </summary>
    public class DatDatabase : IDisposable {
        /// <summary>
        /// A delegate used to configure a <see cref="DatBTreeFile"/> entry before it is inserted into the tree.
        /// </summary>
        /// <param name="entry"></param>
        public delegate void ConfigureEntryDelegate(ref DatBTreeFile entry);

        public ConcurrentDictionary<uint, IDBObj> _fileCache = [];

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

            if (TryGet<Iteration>(0xFFFF0001, out var iteration)) {
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
                    return [typeAttr.FirstId];
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
        /// <param name="autoDecompress">Automatically decompress file data as needed, defaults to true</param>
        /// <returns>True if the file was found, false otherwise</returns>
#if (NET8_0_OR_GREATER)
        public bool TryGetFileBytes(uint fileId, [MaybeNullWhen(false)] out byte[] bytes, bool autoDecompress = true) {
#else
        public bool TryGetFileBytes(uint fileId, out byte[] bytes, bool autoDecompress = true) {
#endif
            if (Tree.TryGetFile(fileId, out var fileEntry)) {
                bytes = new byte[fileEntry.Size];
                BlockAllocator.ReadBlock(bytes, fileEntry.Offset);

                if (autoDecompress && fileEntry.Flags.HasFlag(DatBTreeFileFlags.IsCompressed)) {
                    bytes = Decompress(bytes);
                }

                return true;
            }

            bytes = null!;
            return false;
        }

#if (NET8_0_OR_GREATER)
        public async ValueTask<(bool Success, byte[]? Bytes)> TryGetFileBytesAsync(uint fileId,
            CancellationToken ct = default) {
#else
        public async Task<(bool Success, byte[]? Bytes)> TryGetFileBytesAsync(uint fileId, CancellationToken ct =
         default) {
#endif
            var (success, fileEntry) = await Tree.TryGetFileAsync(fileId, ct);
            if (success) {
                var size = (int)fileEntry.Size;
                var buffer = BaseBlockAllocator.SharedBytes.Rent(size);
                try {
                    await BlockAllocator.ReadBlockAsync(buffer, fileEntry.Offset, ct);

                    if (fileEntry.Flags.HasFlag(DatBTreeFileFlags.IsCompressed)) {
                        var uncompressedSize = BitConverter.ToUInt32(buffer, 0);
                        var decompressedBytes = new byte[uncompressedSize];
                        Decompress(buffer.AsSpan(0, size), decompressedBytes);
                        return (true, decompressedBytes);
                    }
                    else {
                        var bytes = new byte[size];
                        buffer.AsSpan(0, size).CopyTo(bytes);
                        return (true, bytes);
                    }
                }
                finally {
                    BaseBlockAllocator.SharedBytes.Return(buffer);
                }
            }

            return (false, null);
        }

        /// <summary>
        /// Get the raw bytes of a file entry, this is never cached.
        /// </summary>
        /// <param name="fileId">The id of the file to get</param>
        /// <param name="bytes">The raw bytes (ref: potentially resized or updated)</param>
        /// <param name="bytesRead">The number of bytes read</param>
        /// <returns>True if the file was found, false otherwise</returns>
        public bool TryGetFileBytes(uint fileId, Span<byte> bytes, out int bytesRead) {
            if (Tree.TryGetFile(fileId, out var fileEntry)) {
                bytesRead = (int)fileEntry.Size;

                // If the caller provided buffer is too small, we can't do much but fail or read what fits?
                // The pattern (out bytesRead) usually implies we read up to fit.
                // But DatBinReader expects full file. 
                // Let's assume we try to read full file.

                var size = (int)fileEntry.Size;
                var buffer = BaseBlockAllocator.SharedBytes.Rent(size);
                try {
                    BlockAllocator.ReadBlock(buffer, fileEntry.Offset);

                    if (fileEntry.Flags.HasFlag(DatBTreeFileFlags.IsCompressed)) {
                        // We need uncompressed size to know if 'bytes' is big enough?
                        // Decompress will throw or fail if dest is too small?
                        // Decompress now takes Span, so it will fill what it can.
                        bytesRead = Decompress(buffer.AsSpan(0, size), bytes);
                    }
                    else {
                        var toRead = Math.Min(bytes.Length, size);
                        buffer.AsSpan(0, toRead).CopyTo(bytes);
                        bytesRead = toRead;
                    }

                    return true;
                }
                finally {
                    BaseBlockAllocator.SharedBytes.Return(buffer);
                }
            }

            bytesRead = 0;
            return false;
        }

        /// <summary>
        /// Get the raw bytes of a file entry, this is never cached.
        /// </summary>
        /// <param name="fileId">The id of the file to get</param>
        /// <param name="bytes">The raw bytes (ref: potentially resized or updated)</param>
        /// <param name="bytesRead">The number of bytes read</param>
        /// <returns>True if the file was found, false otherwise</returns>
        public bool TryGetFileBytes(uint fileId, ref byte[] bytes, out int bytesRead) {
            if (Tree.TryGetFile(fileId, out var fileEntry)) {
                bytesRead = (int)fileEntry.Size;
                if (bytes.Length < bytesRead) {
                    Array.Resize(ref bytes, bytesRead);
                }

                var size = (int)fileEntry.Size;
                var buffer = BaseBlockAllocator.SharedBytes.Rent(size);
                try {
                    BlockAllocator.ReadBlock(buffer, fileEntry.Offset);

                    if (fileEntry.Flags.HasFlag(DatBTreeFileFlags.IsCompressed)) {
                        var uncompressedSize = BitConverter.ToUInt32(buffer, 0);
                        if (bytes.Length < uncompressedSize) {
                            Array.Resize(ref bytes, (int)uncompressedSize);
                        }

                        bytesRead = Decompress(buffer.AsSpan(0, size), bytes.AsSpan());
                    }
                    else {
                        buffer.AsSpan(0, size).CopyTo(bytes);
                    }

                    return true;
                }
                finally {
                    BaseBlockAllocator.SharedBytes.Return(buffer);
                }
            }

            bytesRead = 0;
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

            var value = Get<T>(fileId);
            if (value is not null && !_fileCache.ContainsKey(fileId)) {
                _fileCache.TryAdd(fileId, value);
            }

            return value;
        }

        /// <summary>
        /// Read a dat file asynchronously, and caches it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
#if (NET8_0_OR_GREATER)
        public async ValueTask<T?> GetCachedAsync<T>(uint fileId, CancellationToken ct = default) where T : IDBObj {
#else
        public async Task<T?> GetCachedAsync<T>(uint fileId, CancellationToken ct = default) where T : IDBObj {
#endif
            if (_fileCache.TryGetValue(fileId, out var cached) && cached is T t) {
                return t;
            }

            var value = await GetAsync<T>(fileId, ct);
            if (value is not null && !_fileCache.ContainsKey(fileId)) {
                // Not thread safe yet!
                _fileCache.TryAdd(fileId, value);
            }

            return value;
        }

        /// <summary>
        /// Read a dat file asynchronously
        /// </summary>
#if (NET8_0_OR_GREATER)
        public async ValueTask<T?> GetAsync<T>(uint fileId, CancellationToken ct = default) where T : IDBObj {
#else
        public async Task<T?> GetAsync<T>(uint fileId, CancellationToken ct = default) where T : IDBObj {
#endif
            var (success, value) = await TryGetAsync<T>(fileId, ct);
            return success ? value : default;
        }

        /// <summary>
        /// Read a dat file, returns null if the file is not found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public T? Get<T>(uint fileId) where T : IDBObj {
            TryGet<T>(fileId, out var value);
            return value;
        }

        /// <summary>
        /// Try and read a <see cref="IDBObj"/>. This will be cached according to the <see cref="DatCollectionOptions.FileCachingStrategy"/> in use
        /// </summary>
        /// <typeparam name="T">The dat file type</typeparam>
        /// <param name="fileId">The id of the file to get</param>
        /// <param name="value">The unpacked file</param>
        /// <returns></returns>
#if (NET8_0_OR_GREATER)
        public bool TryGet<T>(uint fileId, [MaybeNullWhen(false)] out T value) where T : IDBObj {
#else
        public bool TryGet<T>(uint fileId, out T value) where T : IDBObj {
#endif
            if (Options.FileCachingStrategy == FileCachingStrategy.OnDemand) {
                if (_fileCache.TryGetValue(fileId, out var cached) && cached is T t) {
                    value = t;
                    return true;
                }
            }

            if (!Tree.TryGetFile(fileId, out var fileEntry)) {
                value = default!;
                return false;
            }

            var size = (int)fileEntry.Size;
            var buffer = BaseBlockAllocator.SharedBytes.Rent(size);
            try {
                BlockAllocator.ReadBlock(buffer, fileEntry.Offset);

                var data = buffer.AsMemory(0, size);
                if (fileEntry.Flags.HasFlag(DatBTreeFileFlags.IsCompressed)) {
                    data = Decompress(data.ToArray());
                }

                value = Activator.CreateInstance<T>();

                // Slice the buffer to the exact size to avoid reading garbage data
                if (!value.Unpack(new DatBinReader(data, this))) {
                    value = default!;
                    return false;
                }

                if (Options.FileCachingStrategy == FileCachingStrategy.OnDemand) {
                    _fileCache[fileId] = value;
                }

                return true;
            }
            finally {
                BaseBlockAllocator.SharedBytes.Return(buffer);
            }
        }

#if (NET8_0_OR_GREATER)
        public async ValueTask<(bool Success, T? Value)> TryGetAsync<T>(uint fileId, CancellationToken ct = default)
            where T : IDBObj {
#else
        public async Task<(bool Success, T? Value)> TryGetAsync<T>(uint fileId, CancellationToken ct =
         default) where T : IDBObj {
#endif
            if (Options.FileCachingStrategy == FileCachingStrategy.OnDemand) {
                if (_fileCache.TryGetValue(fileId, out var cached) && cached is T t) {
                    return (true, t);
                }
            }

            var (success, fileEntry) = await Tree.TryGetFileAsync(fileId, ct);
            if (!success) {
                return (false, default);
            }

            var size = (int)fileEntry.Size;
            var buffer = BaseBlockAllocator.SharedBytes.Rent(size);
            byte[]? decompressedBuffer = null;
            try {
                await BlockAllocator.ReadBlockAsync(buffer, fileEntry.Offset, ct);

                Memory<byte> data = buffer.AsMemory(0, size);
                if (fileEntry.Flags.HasFlag(DatBTreeFileFlags.IsCompressed)) {
                    var uncompressedSize = BitConverter.ToUInt32(buffer, 0);
                    decompressedBuffer = BaseBlockAllocator.SharedBytes.Rent((int)uncompressedSize);
                    var decompressedSize = Decompress(buffer.AsSpan(0, size), decompressedBuffer);
                    data = decompressedBuffer.AsMemory(0, decompressedSize);
                }

                var value = Activator.CreateInstance<T>();

                // Slice the buffer to the exact size to avoid reading garbage data
                if (!value.Unpack(new DatBinReader(data, this))) {
                    return (false, default);
                }

                if (Options.FileCachingStrategy == FileCachingStrategy.OnDemand) {
                    _fileCache[fileId] = value;
                }

                return (true, value);
            }
            finally {
                BaseBlockAllocator.SharedBytes.Return(buffer);
                if (decompressedBuffer != null)
                    BaseBlockAllocator.SharedBytes.Return(decompressedBuffer);
            }
        }

        // Removed obsolete TryReadFile

        /// <summary>
        /// Try and write a <see cref="IDBObj"/> to the dat.
        /// </summary>
        /// <param name="value">The value to write</param>
        /// <param name="iteration">The iteration to use. If none is passed, it will use the current files iteration if available, otherwise it will use the current dat iteration.</param>
        public Result<T, string> TryWriteFile<T>(T value, int? iteration = null) where T : IDBObj {
            return TryWriteFileInternal(value, false, (ref DatBTreeFile entry) => {
                if (iteration.HasValue) entry.Iteration = iteration.Value;
            });
        }

        /// <summary>
        /// Try and write a <see cref="IDBObj"/> to the dat.
        /// </summary>
        /// <param name="value">The value to write</param>
        /// <param name="template">A template to use for the file entry metadata (Flags, Version, Iteration)</param>
        public Result<T, string> TryWriteFile<T>(T value, DatBTreeFile template) where T : IDBObj {
            return TryWriteFileInternal(value, false, (ref DatBTreeFile entry) => {
                entry.Flags = (template.Flags & ~DatBTreeFileFlags.IsCompressed) |
                              (entry.Flags & DatBTreeFileFlags.IsCompressed);
                entry.Version = template.Version;
                entry.Iteration = template.Iteration;
            });
        }

        /// <summary>
        /// Try and write a <see cref="IDBObj"/> to the dat, attempting compression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="iteration"></param>
        /// <returns></returns>
        public Result<T, string> TryWriteCompressed<T>(T value, int? iteration = null) where T : IDBObj {
            return TryWriteFileInternal(value, true, (ref DatBTreeFile entry) => {
                if (iteration.HasValue) entry.Iteration = iteration.Value;
            });
        }

        /// <summary>
        /// Try and write a <see cref="IDBObj"/> to the dat, attempting compression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="template">A template to use for the file entry metadata (Flags, Version, Iteration)</param>
        /// <returns></returns>
        public Result<T, string> TryWriteCompressed<T>(T value, DatBTreeFile template) where T : IDBObj {
            return TryWriteFileInternal(value, true, (ref DatBTreeFile entry) => {
                entry.Flags = (template.Flags & ~DatBTreeFileFlags.IsCompressed) |
                              (entry.Flags & DatBTreeFileFlags.IsCompressed);
                entry.Version = template.Version;
                entry.Iteration = template.Iteration;
            });
        }

        private Result<T, string> TryWriteFileInternal<T>(T value, bool compress,
            ConfigureEntryDelegate? configureEntry = null)
            where T : IDBObj {
            // TODO: fix this static 5mb buffer...?
            // we dont know how big the file will be, so we need to make sure we have enough space
            var buffer = BaseBlockAllocator.SharedBytes.Rent(1024 * 1024 * 5);
            try {
                var writer = new DatBinWriter(buffer, this);
                value.Pack(writer);

                var res = TryWriteBytesCore(value.Id, buffer, writer.Offset, compress, configureEntry);
                if (!res) {
                    return res.Error!;
                }

                if (_fileCache.ContainsKey(value.Id)) {
                    _fileCache[value.Id] = value;
                }

                return value;
            }
            finally {
                BaseBlockAllocator.SharedBytes.Return(buffer);
            }
        }

        /// <summary>
        /// Try and write a <see cref="IDBObj"/> to the dat asynchronously.
        /// </summary>
#if (NET8_0_OR_GREATER)
        public async ValueTask<Result<T, string>> TryWriteFileAsync<T>(T value, int? iteration = null,
            CancellationToken ct = default) where T : IDBObj {
#else
        public async Task<Result<T, string>> TryWriteFileAsync<T>(T value, int? iteration = null, CancellationToken ct =
         default) where T : IDBObj {
#endif
            return await TryWriteFileInternalAsync(value, false, (ref DatBTreeFile entry) => {
                if (iteration.HasValue) entry.Iteration = iteration.Value;
            }, ct);
        }

        /// <summary>
        /// Try and write a <see cref="IDBObj"/> to the dat asynchronously.
        /// </summary>
#if (NET8_0_OR_GREATER)
        public async ValueTask<Result<T, string>> TryWriteFileAsync<T>(T value, DatBTreeFile template,
            CancellationToken ct = default) where T : IDBObj {
#else
        public async Task<Result<T, string>> TryWriteFileAsync<T>(T value, DatBTreeFile template, CancellationToken ct =
         default) where T : IDBObj {
#endif
            return await TryWriteFileInternalAsync(value, false, (ref DatBTreeFile entry) => {
                entry.Flags = (template.Flags & ~DatBTreeFileFlags.IsCompressed) |
                              (entry.Flags & DatBTreeFileFlags.IsCompressed);
                entry.Version = template.Version;
                entry.Iteration = template.Iteration;
            }, ct);
        }

        /// <summary>
        /// Try and write a <see cref="IDBObj"/> to the dat asynchronously, attempting compression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="iteration"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
#if (NET8_0_OR_GREATER)
        public async ValueTask<Result<T, string>> TryWriteCompressedAsync<T>(T value, int? iteration = null,
            CancellationToken ct = default) where T : IDBObj {
#else
        public async Task<Result<T, string>> TryWriteCompressedAsync<T>(T value, int? iteration =
 null, CancellationToken ct =
         default) where T : IDBObj {
#endif
            return await TryWriteFileInternalAsync(value, true, (ref DatBTreeFile entry) => {
                if (iteration.HasValue) entry.Iteration = iteration.Value;
            }, ct);
        }

        /// <summary>
        /// Try and write a <see cref="IDBObj"/> to the dat asynchronously, attempting compression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="template"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
#if (NET8_0_OR_GREATER)
        public async ValueTask<Result<T, string>> TryWriteCompressedAsync<T>(T value, DatBTreeFile template,
            CancellationToken ct = default) where T : IDBObj {
#else
        public async Task<Result<T, string>> TryWriteCompressedAsync<T>(T value, DatBTreeFile template, CancellationToken ct
 =
         default) where T : IDBObj {
#endif
            return await TryWriteFileInternalAsync(value, true, (ref DatBTreeFile entry) => {
                entry.Flags = (template.Flags & ~DatBTreeFileFlags.IsCompressed) |
                              (entry.Flags & DatBTreeFileFlags.IsCompressed);
                entry.Version = template.Version;
                entry.Iteration = template.Iteration;
            }, ct);
        }

#if (NET8_0_OR_GREATER)
        private async ValueTask<Result<T, string>> TryWriteFileInternalAsync<T>(T value, bool compress,
            ConfigureEntryDelegate? configureEntry = null,
            CancellationToken ct = default) where T : IDBObj {
#else
        private async Task<Result<T, string>> TryWriteFileInternalAsync<T>(T value, bool compress, ConfigureEntryDelegate? configureEntry
 = null, CancellationToken ct =
         default) where T : IDBObj {
#endif
            // TODO: fix this static 5mb buffer...?
            var buffer = BaseBlockAllocator.SharedBytes.Rent(1024 * 1024 * 5);
            try {
                var writer = new DatBinWriter(buffer, this);
                value.Pack(writer);

                var res = await TryWriteBytesCoreAsync(value.Id, buffer, writer.Offset, compress, configureEntry, ct);
                if (!res) {
                    return res.Error!;
                }

                if (_fileCache.ContainsKey(value.Id)) {
                    _fileCache[value.Id] = value;
                }

                return value;
            }
            finally {
                BaseBlockAllocator.SharedBytes.Return(buffer);
            }
        }

        /// <summary>
        /// Try and write a <see cref="IDBObj"/> to the dat.
        /// </summary>
        /// <param name="id">The id of the file to write</param>
        /// <param name="buffer">The value to write</param>
        /// <param name="bytesToWrite">The number of bytes to write from the buffer.</param>
        /// <param name="iteration">The iteration to use.</param>
        public Result<bool, string> TryWriteFileBytes(uint id, byte[] buffer, int bytesToWrite, int iteration) {
            return TryWriteBytesCore(id, buffer, bytesToWrite, false, (ref DatBTreeFile entry) => {
                entry.Iteration = iteration;
            });
        }

        /// <summary>
        /// Try and write a <see cref="IDBObj"/> to the dat.
        /// </summary>
        /// <param name="id">The id of the file to write</param>
        /// <param name="buffer">The value to write</param>
        /// <param name="bytesToWrite">The number of bytes to write from the buffer.</param>
        /// <param name="template">A template to use for the file entry metadata (Flags, Version, Iteration)</param>
        public Result<bool, string> TryWriteFileBytes(uint id, byte[] buffer, int bytesToWrite, DatBTreeFile template) {
            return TryWriteBytesCore(id, buffer, bytesToWrite, false, (ref DatBTreeFile entry) => {
                entry.Flags = (template.Flags & ~DatBTreeFileFlags.IsCompressed) |
                              (entry.Flags & DatBTreeFileFlags.IsCompressed);
                entry.Version = template.Version;
                entry.Iteration = template.Iteration;
            });
        }

        /// <summary>
        /// Try and write raw bytes to the dat, attempting compression.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buffer"></param>
        /// <param name="bytesToWrite"></param>
        /// <param name="iteration"></param>
        /// <returns></returns>
        public Result<bool, string> TryWriteCompressedBytes(uint id, byte[] buffer, int bytesToWrite, int iteration) {
            return TryWriteBytesCore(id, buffer, bytesToWrite, true, (ref DatBTreeFile entry) => {
                entry.Iteration = iteration;
            });
        }

        /// <summary>
        /// Try and write raw bytes to the dat, attempting compression.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buffer"></param>
        /// <param name="bytesToWrite"></param>
        /// <param name="template">A template to use for the file entry metadata (Flags, Version, Iteration)</param>
        /// <returns></returns>
        public Result<bool, string> TryWriteCompressedBytes(uint id, byte[] buffer, int bytesToWrite,
            DatBTreeFile template) {
            return TryWriteBytesCore(id, buffer, bytesToWrite, true, (ref DatBTreeFile entry) => {
                entry.Flags = (template.Flags & ~DatBTreeFileFlags.IsCompressed) |
                              (entry.Flags & DatBTreeFileFlags.IsCompressed);
                entry.Version = template.Version;
                entry.Iteration = template.Iteration;
            });
        }

        private Result<bool, string>
            TryWriteBytesCore(uint id, byte[] buffer, int bytesToWrite, bool compress,
                ConfigureEntryDelegate? configureEntry) {
            if (!BlockAllocator.CanWrite) {
                return "Block allocator was opened as read only.";
            }

            int startingBlockId = 0;
            DatBTreeFileFlags existingFlags = DatBTreeFileFlags.None;
            ushort existingVersion = 2;
            int existingIteration = 0;

            if (Tree.TryGetFile(id, out var existingFile)) {
                startingBlockId = existingFile.Offset;
                existingFlags = existingFile.Flags;
                existingVersion = existingFile.Version;
                existingIteration = existingFile.Iteration;
            }

            byte[] writeData = buffer;
            int writeLen = bytesToWrite;
            byte[]? compressedBuffer = null;

            try {
                if (compress) {
                    compressedBuffer = BaseBlockAllocator.SharedBytes.Rent(writeLen + 1024);
                    if (AttemptToCompress(buffer, writeLen, compressedBuffer, out var compressedLen)) {
                        writeData = compressedBuffer;
                        writeLen = compressedLen;
                        existingFlags |= DatBTreeFileFlags.IsCompressed;
                    }
                    else {
                        existingFlags &= ~DatBTreeFileFlags.IsCompressed;
                    }
                }
                else {
                    existingFlags &= ~DatBTreeFileFlags.IsCompressed;
                }

                startingBlockId = Tree.BlockAllocator.WriteBlock(writeData, writeLen, startingBlockId);
            }
            finally {
                if (compressedBuffer != null)
                    BaseBlockAllocator.SharedBytes.Return(compressedBuffer);
            }

            var entry = new DatBTreeFile {
                Flags = existingFlags,
                Version = existingVersion,
                Id = id,
                Size = (uint)writeLen,
                Offset = startingBlockId,
                Date = DateTime.UtcNow,
                Iteration = existingIteration
            };

            configureEntry?.Invoke(ref entry);

            Tree.Insert(entry);

            // update dat iteration if needed
            if (entry.Iteration > Iteration.CurrentIteration) {
                Iteration.CurrentIteration = entry.Iteration;
                var iterationUpdateRes = TryWriteFile(Iteration);
                if (!iterationUpdateRes) {
                    return iterationUpdateRes.Error ?? "Failed to update dat iteration.";
                }
            }

            return true;
        }

        /// <summary>
        /// Try and write a <see cref="IDBObj"/> to the dat asynchronously.
        /// </summary>
        /// <param name="id">The id of the file to write</param>
        /// <param name="buffer">The value to write</param>
        /// <param name="bytesToWrite">The number of bytes to write from the buffer.</param>
        /// <param name="iteration">The iteration to use.</param>
        /// <param name="ct">Cancellation token</param>
#if (NET8_0_OR_GREATER)
        public async ValueTask<Result<bool, string>> TryWriteFileBytesAsync(uint id, byte[] buffer, int bytesToWrite,
            int iteration, CancellationToken ct = default) {
#else
        public async Task<Result<bool, string>> TryWriteFileBytesAsync(uint id, byte[] buffer, int bytesToWrite, int iteration, CancellationToken ct
  = default) {
#endif
            return await TryWriteBytesCoreAsync(id, buffer, bytesToWrite, false, (ref DatBTreeFile entry) => {
                entry.Iteration = iteration;
            }, ct);
        }

        /// <summary>
        /// Try and write a <see cref="IDBObj"/> to the dat asynchronously.
        /// </summary>
        /// <param name="id">The id of the file to write</param>
        /// <param name="buffer">The value to write</param>
        /// <param name="bytesToWrite">The number of bytes to write from the buffer.</param>
        /// <param name="template">A template to use for the file entry metadata (Flags, Version, Iteration)</param>
        /// <param name="ct">Cancellation token</param>
#if (NET8_0_OR_GREATER)
        public async ValueTask<Result<bool, string>> TryWriteFileBytesAsync(uint id, byte[] buffer, int bytesToWrite,
            DatBTreeFile template, CancellationToken ct = default) {
#else
        public async Task<Result<bool, string>> TryWriteFileBytesAsync(uint id, byte[] buffer, int bytesToWrite, DatBTreeFile template, CancellationToken ct
  = default) {
#endif
            return await TryWriteBytesCoreAsync(id, buffer, bytesToWrite, false, (ref DatBTreeFile entry) => {
                entry.Flags = (template.Flags & ~DatBTreeFileFlags.IsCompressed) |
                              (entry.Flags & DatBTreeFileFlags.IsCompressed);
                entry.Version = template.Version;
                entry.Iteration = template.Iteration;
            }, ct);
        }

        /// <summary>
        /// Try and write raw bytes to the dat asynchronously, attempting compression.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buffer"></param>
        /// <param name="bytesToWrite"></param>
        /// <param name="iteration"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
#if (NET8_0_OR_GREATER)
        public async ValueTask<Result<bool, string>> TryWriteCompressedBytesAsync(uint id, byte[] buffer,
            int bytesToWrite,
            int iteration, CancellationToken ct = default) {
#else
        public async Task<Result<bool, string>> TryWriteCompressedBytesAsync(uint id, byte[] buffer, int bytesToWrite, int iteration, CancellationToken ct
  = default) {
#endif
            return await TryWriteBytesCoreAsync(id, buffer, bytesToWrite, true, (ref DatBTreeFile entry) => {
                entry.Iteration = iteration;
            }, ct);
        }

        /// <summary>
        /// Try and write raw bytes to the dat asynchronously, attempting compression.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buffer"></param>
        /// <param name="bytesToWrite"></param>
        /// <param name="template">A template to use for the file entry metadata (Flags, Version, Iteration)</param>
        /// <param name="ct"></param>
        /// <returns></returns>
#if (NET8_0_OR_GREATER)
        public async ValueTask<Result<bool, string>> TryWriteCompressedBytesAsync(uint id, byte[] buffer,
            int bytesToWrite,
            DatBTreeFile template, CancellationToken ct = default) {
#else
        public async Task<Result<bool, string>> TryWriteCompressedBytesAsync(uint id, byte[] buffer, int bytesToWrite, DatBTreeFile template, CancellationToken ct
  = default) {
#endif
            return await TryWriteBytesCoreAsync(id, buffer, bytesToWrite, true, (ref DatBTreeFile entry) => {
                entry.Flags = (template.Flags & ~DatBTreeFileFlags.IsCompressed) |
                              (entry.Flags & DatBTreeFileFlags.IsCompressed);
                entry.Version = template.Version;
                entry.Iteration = template.Iteration;
            }, ct);
        }

#if (NET8_0_OR_GREATER)
        private async ValueTask<Result<bool, string>> TryWriteBytesCoreAsync(uint id, byte[] buffer,
            int bytesToWrite, bool compress, ConfigureEntryDelegate? configureEntry =
                null, CancellationToken ct = default) {
#else
        private async Task<Result<bool, string>> TryWriteBytesCoreAsync(uint id, byte[] buffer, int bytesToWrite,
            bool compress, ConfigureEntryDelegate? configureEntry = null, CancellationToken ct = default) {
#endif
            if (!BlockAllocator.CanWrite) {
                return "Block allocator was opened as read only.";
            }

            int startingBlockId = 0;
            DatBTreeFileFlags existingFlags = DatBTreeFileFlags.None;
            ushort existingVersion = 2;
            int existingIteration = 0;

            var (found, existingFile) = await Tree.TryGetFileAsync(id, ct);
            if (found) {
                startingBlockId = existingFile.Offset;
                existingFlags = existingFile.Flags;
                existingVersion = existingFile.Version;
                existingIteration = existingFile.Iteration;
            }

            byte[] writeData = buffer;
            int writeLen = bytesToWrite;
            byte[]? compressedBuffer = null;

            try {
                if (compress) {
                    compressedBuffer = BaseBlockAllocator.SharedBytes.Rent(writeLen + 1024);
                    if (AttemptToCompress(buffer, writeLen, compressedBuffer, out var compressedLen)) {
                        writeData = compressedBuffer;
                        writeLen = compressedLen;
                        existingFlags |= DatBTreeFileFlags.IsCompressed;
                    }
                    else {
                        existingFlags &= ~DatBTreeFileFlags.IsCompressed;
                    }
                }
                else {
                    existingFlags &= ~DatBTreeFileFlags.IsCompressed;
                }

                startingBlockId = await BlockAllocator.WriteBlockAsync(writeData, writeLen, startingBlockId, ct);
            }
            finally {
                if (compressedBuffer != null)
                    BaseBlockAllocator.SharedBytes.Return(compressedBuffer);
            }

            var entry = new DatBTreeFile {
                Flags = existingFlags,
                Version = existingVersion,
                Id = id,
                Size = (uint)writeLen,
                Offset = startingBlockId,
                Date = DateTime.UtcNow,
                Iteration = existingIteration
            };

            configureEntry?.Invoke(ref entry);

            await Tree.InsertAsync(entry, ct);

            // update dat iteration if needed
            if (entry.Iteration > Iteration.CurrentIteration) {
                Iteration.CurrentIteration = entry.Iteration;
                var iterationUpdateRes = await TryWriteFileAsync(Iteration, null, ct);
                if (!iterationUpdateRes) {
                    return iterationUpdateRes.Error ?? "Failed to update dat iteration.";
                }
            }

            return true;
        }

        private bool AttemptToCompress(byte[] data, int validLen, byte[] dest, out int destLen) {
            destLen = 0;
            if (validLen < 16) return false;

#if (NET8_0_OR_GREATER)
            using var outputStream = new System.IO.MemoryStream(dest);
            // Reserve 4 bytes for size
            outputStream.Position = 4;

            using (var zlibStream = new ZLibStream(outputStream, CompressionLevel.SmallestSize, true)) {
                zlibStream.Write(data, 0, validLen);
            }

            var compressedLen = (int)outputStream.Position;
            if (compressedLen < validLen) {
                // Write size at beginning
                System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(dest.AsSpan(0, 4), (uint)validLen);
                destLen = compressedLen;
                return true;
            }

            return false;
#else
            var zlib = new ZLibDotNet.ZLib();
            var tempDest = new byte[dest.Length];
            int zlibLen;
            if (zlib.Compress(tempDest, out zlibLen, data, validLen, 9) == 0) {
                 if (zlibLen + 4 < validLen) {
                     Array.Copy(tempDest, 0, dest, 4, zlibLen);
                     System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(dest.AsSpan(0, 4), (uint)validLen);
                     destLen = zlibLen + 4;
                     return true;
                 }
            }
            return false;
#endif
        }

        private byte[] AttemptToCompress(byte[] data, out bool compressed) {
            compressed = false;
            var dest = new byte[(int)(data.Length * 1.1) + 12];
            if (AttemptToCompress(data, data.Length, dest, out var len)) {
                compressed = true;
                return dest.AsSpan(0, len).ToArray();
            }

            return data;
        }

        private int Decompress(ReadOnlySpan<byte> data, Span<byte> destination) {
            if (data.Length < 4) {
                data.CopyTo(destination);
                return data.Length;
            }

            var uncompressedSize = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(data);

#if (NET8_0_OR_GREATER)
            using var outputStream = new System.IO.MemoryStream(data.Slice(4).ToArray());
            using var zlibStream = new ZLibStream(outputStream, CompressionMode.Decompress);
            return zlibStream.Read(destination);
#else
            var zlib = new ZLibDotNet.ZLib();
            var dataArray = data.ToArray(); 
            var destArray = new byte[destination.Length]; 
            int outSize;
            zlib.Uncompress(destArray, out outSize, dataArray.AsSpan(4).ToArray(), out _);
            destArray.AsSpan(0, outSize).CopyTo(destination);
            return outSize;
#endif
        }

        private byte[] Decompress(byte[] data) {
            if (data.Length < 4) return data;
            var uncompressedSize = BitConverter.ToUInt32(data, 0);
            var dest = new byte[uncompressedSize];
            Decompress(data.AsSpan(), dest);
            return dest;
        }

        /// <inheritdoc/>
        public void Dispose() {
            ClearCache();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                Tree?.Dispose();
                BlockAllocator?.Dispose();
            }
        }
    }
}