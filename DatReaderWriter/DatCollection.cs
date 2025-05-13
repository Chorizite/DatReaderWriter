using DatReaderWriter.DBObjs;
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
        private MasterProperty? _masterProperty;

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

            Cell = new CellDatabase((o) => {
                o.FilePath = options.CellDatPath;
                o.AccessType = options.AccessType;
                o.IndexCachingStrategy = options.CellIndexCachingStrategy;
                o.FileCachingStrategy = options.CellFileCachingStrategy;
            });

            Portal = new PortalDatabase((o) => {
                o.FilePath = options.PortalDatPath;
                o.AccessType = options.AccessType;
                o.IndexCachingStrategy = options.PortalIndexCachingStrategy;
                o.FileCachingStrategy = options.PortalFileCachingStrategy;
            });

            Local = new LocalDatabase((o) => {
                o.FilePath = options.LocalDatPath;
                o.AccessType = options.AccessType;
                o.IndexCachingStrategy = options.LocalIndexCachingStrategy;
                o.FileCachingStrategy = options.LocalFileCachingStrategy;
            });

            HighRes = new PortalDatabase((o) => {
                o.FilePath = options.HighResDatPath;
                o.AccessType = options.AccessType;
                o.IndexCachingStrategy = options.HighResIndexCachingStrategy;
                o.FileCachingStrategy = options.HighResFileCachingStrategy;
            });

            Cell.DatCollection = this;
            Portal.DatCollection = this;
            Local.DatCollection = this;
            HighRes.DatCollection = this;
        }

        /// <summary>
        /// Clear the file cache, only applicable if <see cref="DatCollectionOptions.FileCachingStrategy"/>
        /// is <see cref="FileCachingStrategy.OnDemand"/>
        /// </summary>
        public void ClearCache() {
            Cell.ClearCache();
            Portal.ClearCache();
            Local.ClearCache();
            HighRes.ClearCache();
        }

        /// <summary>
        /// Read a dat file, and caches it regardless of <see cref="DatCollectionOptions.FileCachingStrategy"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public T? GetCached<T>(uint fileId) where T : IDBObj {
            switch (MapToDatFileType<T>()) {
                case DatFileType.Cell:
                    return Cell.GetCached<T>(fileId);
                case DatFileType.Portal:
                    return Portal.GetCached<T>(fileId) ?? HighRes.GetCached<T>(fileId);
                case DatFileType.Local:
                    return Local.GetCached<T>(fileId);
                default:
                    return default;
            }
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
        /// Get an enumerable of all existing ids of a specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public IEnumerable<uint> GetAllIdsOfType<T>() where T : IDBObj {
            switch (MapToDatFileType<T>()) {
                case DatFileType.Cell:
                    return Cell.GetAllIdsOfType<T>();
                case DatFileType.Portal:
                    return Portal.GetAllIdsOfType<T>().Concat(HighRes.GetAllIdsOfType<T>());
                case DatFileType.Local:
                    return Local.GetAllIdsOfType<T>();
                default:
                    return [];
            }
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
            switch (MapToDatFileType<T>()) {
                case DatFileType.Cell:
                    return Cell.TryGet(fileId, out value);
                case DatFileType.Portal:
                    var portalRes = Portal.TryGet(fileId, out value);
                    if (!portalRes) {
                        portalRes = HighRes.TryGet(fileId, out value);
                    }
                    return portalRes;
                case DatFileType.Local:
                    return Local.TryGet(fileId, out value);
                default:
                    value = default!;
                    return false;
            }
        }

        /// <summary>
        /// OBSOLETE: This has been replaced by <see cref="TryGet{T}(uint, out T)"/>!
        /// </summary>
        /// <typeparam name="T">The dat file type</typeparam>
        /// <param name="fileId">The id of the file to get</param>
        /// <param name="value">The unpacked file</param>
        /// <returns></returns>
        [Obsolete("This has been renamed to TryGet<T>(uint, out T)!")]
#if (NET8_0_OR_GREATER)
        public bool TryReadFile<T>(uint fileId, [MaybeNullWhen(false)] out T value) where T : IDBObj {
#else
        public bool TryReadFile<T>(uint fileId, out T value) where T : IDBObj {
#endif
            return TryGet<T>(fileId, out value);
        }

        /// <summary>
        /// Get the DatFileType associated with the specified IDBObj type
        /// </summary>
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
