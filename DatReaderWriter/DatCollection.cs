using DatReaderWriter.Enums;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DatReaderWriter {
    /// <summary>
    /// Represents a collection of eor .dat files
    /// </summary>
    public class DatCollection : IDisposable {
        private Dictionary<uint, IDBObj> _cellCache = new();
        private Dictionary<uint, IDBObj> _portalCache = new();
        private Dictionary<uint, IDBObj> _localCache = new();

        /// <summary>
        /// The options this collection was created with
        /// </summary>
        public DatCollectionOptions Options { get; }

        /// <summary>
        /// The cell database
        /// </summary>
        public CellDatabase Cell { get; }

        /// <summary>
        /// The portal database
        /// </summary>
        public PortalDatabase Portal { get; }

        /// <summary>
        /// The local database
        /// </summary>
        public LocalDatabase Local { get; }

        /// <summary>
        /// The high res database
        /// </summary>
        public PortalDatabase HighRes { get; }

        /// <summary>
        /// Create a new dat collection from the specified directory. Loads the following eor dat files:
        /// client_cell_1.dat
        /// client_portal.dat
        /// client_highres.dat
        /// client_local_English.dat
        /// </summary>
        /// <param name="datDirectory"></param>
        /// <param name="datAccessType"></param>
        public DatCollection(string datDirectory, DatAccessType datAccessType = DatAccessType.Read) : this(new DatCollectionOptions() {
            DatDirectory = datDirectory,
            AccessType = datAccessType
        }) {

        }

        /// <summary>
        /// Create a new dat collection, using the specified options
        /// </summary>
        /// <param name="options"></param>
        public DatCollection(DatCollectionOptions options) {
            Options = options;

            Cell = new CellDatabase(options.CellDatPath, options.AccessType);
            Portal = new PortalDatabase(options.PortalDatPath, options.AccessType);
            Local = new LocalDatabase(options.LocalDatPath, options.AccessType);
            HighRes = new PortalDatabase(options.HighResDatPath, options.AccessType);

            Cell.DatCollection = this;
            Portal.DatCollection = this;
            Local.DatCollection = this;
            HighRes.DatCollection = this;
        }

        /// <summary>
        /// Clear the file cache, only applicable if <see cref="DatCollectionOptions.CacheFiles"/> is true
        /// </summary>
        public void ClearCache() {
            _cellCache.Clear();
            _portalCache.Clear();
            _localCache.Clear();
        }

        /// <summary>
        /// Read a dat file, returns null if the file is not found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public T? Read<T>(uint fileId) where T : IDBObj {
            TryReadFile<T>(fileId, out var value);
            return value;
        }

        /// <summary>
        /// Read a dat file
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
            switch (MapToDatFileType<T>()) {
                case DatFileType.Cell:
                    if (Options.CacheFiles && _cellCache.TryGetValue(fileId, out var cached) && cached is T t) {
                        value = t;
                        return true;
                    }
                    var res = Cell.TryReadFile(fileId, out value);
                    if (res && Options.CacheFiles) {
                        _cellCache[fileId] = value!;
                    }
                    return res;
                case DatFileType.Portal:
                    if (Options.CacheFiles && _portalCache.TryGetValue(fileId, out cached) && cached is T t2) {
                        value = t2;
                        return true;
                    }
                    var res2 = Portal.TryReadFile(fileId, out value);
                    if (!res2) {
                        res2 = HighRes.TryReadFile(fileId, out value);
                    }

                    if (res2 && Options.CacheFiles) {
                        _portalCache[fileId] = value!;
                    }

                    return res2;
                case DatFileType.Local:
                    if (Options.CacheFiles && _localCache.TryGetValue(fileId, out cached) && cached is T t3) {
                        value = t3;
                        return true;
                    }
                    var res3 = Local.TryReadFile(fileId, out value);
                    if (res3 && Options.CacheFiles) {
                        _localCache[fileId] = value!;
                    }
                    return res3;
                default:
                    value = default!;
                    return false;
            }
        }

        /// <summary>
        /// Get the dat file that stores an id
        /// </summary>
        /// <param name="id">The id of the dat file entry</param>
        /// <returns>The DatFileType this entry is stored in</returns>
        public DatFileType MapToDatFileType<T>() where T : IDBObj {
            var typeCacheVal = DBObjAttributeCache.TypeCache.Values.FirstOrDefault(t => t.Type == typeof(T));
            return typeCacheVal?.DatFileType ?? DatFileType.Undefined;
        }

        /// <summary>
        /// Dispose of all databases (flush changes to disk, close files)
        /// </summary>
        public void Dispose() {
            Cell.Dispose();
            Portal.Dispose();
            Local.Dispose();
            HighRes.Dispose();
        }
    }
}
