using ACDatReader.IO;
using ACDatReader.IO.BlockReaders;
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
        private readonly static ArrayPool<byte> sharedBytes = ArrayPool<byte>.Shared;

        private readonly Dictionary<uint, DatFileEntry> _fileEntryCache = [];
        private readonly Dictionary<uint, DatDirectoryEntry> _directoryCache = [];
        private readonly IDatBlockReader _blockReader;

        /// <summary>
        /// Database Options
        /// </summary>
        public DatDatabaseOptions Options { get; }

        /// <summary>
        /// All dat driectories currently in the cache. Key is the dat offset. This
        /// will be null unless <see cref="DatDatabaseOptions.CacheDirectories"/> was
        /// set to true.
        /// </summary>
        public IReadOnlyDictionary<uint, DatDirectoryEntry>? DirectoryCache => _directoryCache;

        /// <summary>
        /// All dat file entries currently in the cache. Key is the file id.
        /// </summary>
        public IReadOnlyDictionary<uint, DatFileEntry> FileEntryCache => _fileEntryCache;

        /// <summary>
        /// Dat header
        /// </summary>
        public DatHeader Header { get; set; }

        /// <summary>
        /// Create a new DatDatabase
        /// </summary>
        /// <param name="options">Options configuration action</param>
        /// <param name="blockReader">Block reader instance to use</param>
        public DatDatabaseReader(Action<DatDatabaseOptions>? options = null, IDatBlockReader? blockReader = null) {
            Options = new DatDatabaseOptions();
            options?.Invoke(Options);

            _blockReader = blockReader ?? new MemoryMappedDatBlockReader(Options.FilePath);

            Init();
        }

        /// <summary>
        /// Try to get the byte contents of a file
        /// </summary>
        /// <param name="fileId">The id of the file to get</param>
        /// <param name="bytes">The bytes that were found, if any.</param>
        /// <returns>True if the file bytes were found, false otherwise</returns>
        public bool TryGetFileBytes(uint fileId, out byte[]? bytes) {
            if (TryGetFileEntry(fileId, out var fileEntry)) {
                bytes = new byte[fileEntry.Size];
                _blockReader.ReadBlocks(ref bytes, fileEntry.Offset, Header.BlockSize);
                return true;
            }

            bytes = null;
            return false;
        }

        /// <summary>
        /// Try to get a file entry
        /// </summary>
        /// <param name="fileId">The id of the file</param>
        /// <param name="fileEntry">The file entry, if it exists</param>
        /// <returns>True if a file entry with the specific <paramref name="fileId"/> was found, false otherwise</returns>
        public bool TryGetFileEntry(uint fileId, out DatFileEntry fileEntry) {
            if (_fileEntryCache.TryGetValue(fileId, out fileEntry)) {
                return true;
            }

            var buffer = sharedBytes.Rent(DatDirectoryEntry.SIZE);

            Span<uint> node = [Header.RootBlock];
            Span<int> track = [0, 0, 0];

            // @paradox logic
            while (node[0] != 0 && node[0] != 0xcdcdcdcd) {
                DatDirectoryEntry de = GetDirectoryEntry(node[0], ref buffer);

                track[0] = 0; // left
                track[1] = de.EntryCount - 1; // right
                track[2] = 0; // i

                while (track[0] <= track[1]) {
                    track[2] = (track[0] + track[1]) / 2;
                    fileEntry = de.Entries![track[2]];

                    if (fileId == fileEntry.Id) {
                        sharedBytes.Return(buffer);
                        return true;
                    }
                    else if (fileId < fileEntry.Id)
                        track[1] = track[2] - 1;
                    else
                        track[0] = track[2] + 1;
                }

                if (de.IsLeaf)
                    break;

                if (fileId > de.Entries![track[2]].Id)
                    track[2]++;

                node[0] = de.Branches![track[2]];
            }

            return false;
        }

        private void Init() {
            InitHeader();

            if (Options.PreloadFileEntries) {
                var buffer = sharedBytes.Rent(DatDirectoryEntry.SIZE);
                PreloadFileEntries(Header.RootBlock, ref buffer);
                sharedBytes.Return(buffer);
            }
        }

        unsafe private void InitHeader() {
            var buffer = sharedBytes.Rent(DatHeader.SIZE);
            _blockReader.ReadBytes(ref buffer, 0, DatHeader.SIZE);

            fixed (byte* pData = &buffer[0]) {
                Header = Marshal.PtrToStructure<DatHeader>((nint)pData);
            }

            InitCaches();
        }

        private void InitCaches() {
            if (Options.CacheDirectories) {
                // todo: what's a good size here...
                _directoryCache.EnsureCapacity(256);
            }

            if (Options.PreloadFileEntries) {
                // init cache size smartly, based on dat type
                switch (Header.Type) {
                    case DatDatabaseType.Portal:
                        if (Header.SubSet == 0) { // portal
                            _fileEntryCache.EnsureCapacity(80000);
                        }
                        else { // highres
                            _fileEntryCache.EnsureCapacity(3000);
                        }
                        break;
                    case DatDatabaseType.Cell:
                        _fileEntryCache.EnsureCapacity(806000);
                        break;
                    default: // language
                        _fileEntryCache.EnsureCapacity(150);
                        break;
                }
            }
        }

        private DatDirectoryEntry GetDirectoryEntry(uint offset, ref byte[] buffer) {
            if (Options.CacheDirectories && _directoryCache.TryGetValue(offset, out var dirEntry)) {
                return dirEntry;
            }

            dirEntry = new DatDirectoryEntry {
                Offset = offset
            };

            _blockReader.ReadBlocks(ref buffer, offset, Header.BlockSize);
            dirEntry.Unpack(buffer);

            if (Options.CacheDirectories) {
                _directoryCache.Add(offset, dirEntry);
            }

            return dirEntry;
        }

        private void PreloadFileEntries(uint directoryOffset, ref byte[] buffer) {
            var dirEntry = GetDirectoryEntry(directoryOffset, ref buffer);

            if (dirEntry.Entries is not null) {
                foreach (var entry in dirEntry.Entries) {
                    _fileEntryCache.Add(entry.Id, entry);
                }
            }

            if (!dirEntry.IsLeaf && dirEntry.Branches is not null) {
                foreach (var branch in dirEntry.Branches) {
                    PreloadFileEntries(branch, ref buffer);
                }
            }
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
                _blockReader?.Dispose();
            }
        }
    }
}