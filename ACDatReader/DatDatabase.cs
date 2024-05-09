using ACDatReader.IO;
using ACDatReader.IO.BlockReaders;
using ACDatReader.Options;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ACDatReader {
    /// <summary>
    /// Provides access to a dat database
    /// </summary>
    public class DatDatabase : IDisposable {
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
        public DatDatabase(Action<DatDatabaseOptions>? options = null, IDatBlockReader? blockReader = null) {
            Options = new DatDatabaseOptions();
            options?.Invoke(Options);

            _blockReader = blockReader ?? new MemoryMappedDatBlockReader(Options.FilePath);

            Init();
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

        private void PreloadFileEntries(uint directoryOffset, ref byte[] buffer) {
            var dirEntry = new DatDirectoryEntry {
                Offset = directoryOffset
            };

            _blockReader.ReadBlocks(ref buffer, directoryOffset, Header.BlockSize);

            dirEntry.Unpack(buffer);

            if (Options.CacheDirectories) {
                _directoryCache.Add(directoryOffset, dirEntry);
            }

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