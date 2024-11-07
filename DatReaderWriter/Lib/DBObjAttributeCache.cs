using DatReaderWriter.DBObjs;
using DatReaderWriter.Enums;
using DatReaderWriter.Types;
using DatReaderWriter.Lib.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Lib {
    internal static class DBObjAttributeCache {
        private static Dictionary<Type, DBObjTypeAttribute>? _typeCache;
        private static Dictionary<Type, DBObjTypeAttribute>? _typeCacheMasks;
        private static Dictionary<Type, DBObjTypeAttribute>? _typeCacheIds;
        private static Dictionary<Type, DBObjTypeAttribute>? _typeCacheRanges;

        public static IReadOnlyDictionary<Type, DBObjTypeAttribute> TypeCache {
            get {
                return _typeCache ??= InitTypeCache();
            }
        }

        public static IReadOnlyDictionary<Type, DBObjTypeAttribute> TypeCacheMasks {
            get {
                return _typeCacheMasks ??= TypeCache
                    .Where(kv => kv.Value.MaskId != 0)
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
            }
        }

        public static IReadOnlyDictionary<Type, DBObjTypeAttribute> TypeCacheIds {
            get {
                return _typeCacheIds ??= TypeCache
                    .Where(kv => kv.Value.FirstId != 0 && kv.Value.FirstId == kv.Value.LastId)
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
            }
        }

        public static IReadOnlyDictionary<Type, DBObjTypeAttribute> TypeCacheRanges {
            get {
                return _typeCacheRanges ??= TypeCache
                    .Where(kv => kv.Value.LastId != 0 && kv.Value.FirstId != kv.Value.LastId)
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
            }
        }

        public static Type? TypeFromId(DatFileType datType, uint id) {
            switch (datType) {
                case DatFileType.Cell:
                    // check exact
                    foreach (var kv in TypeCacheIds.Where(kv => kv.Value.DatFileType == datType)) {
                        if (id == kv.Value.FirstId) {
                            return kv.Key;
                        }
                    }
                    // check masks
                    foreach (var kv in TypeCacheMasks.Where(kv => kv.Value.DatFileType == datType)) {
                        if ((id & 0x0000FFFF) == kv.Value.MaskId) {
                            return kv.Key;
                        }
                    }
                    return typeof(EnvCell);

                case DatFileType.Portal:
                    // check exact
                    foreach (var kv in TypeCacheIds.Where(kv => kv.Value.DatFileType == datType)) {
                        if (id == kv.Value.FirstId) {
                            return kv.Key;
                        }
                    }
                    // check ranges
                    foreach (var kv in TypeCacheRanges.Where(kv => kv.Value.DatFileType == datType)) {
                        if (id >= kv.Value.FirstId && id <= kv.Value.LastId) {
                            return kv.Key;
                        }
                    }
                    break;

                case DatFileType.Local:
                    // check exact
                    foreach (var kv in TypeCacheIds.Where(kv => kv.Value.DatFileType == datType)) {
                        if (id == kv.Value.FirstId) {
                            return kv.Key;
                        }
                    }
                    // check masks
                    foreach (var kv in TypeCacheMasks.Where(kv => kv.Value.DatFileType == datType)) {
                        if ((id & 0x0000FFFF) == kv.Value.MaskId) {
                            return kv.Key;
                        }
                    }
                    // check ranges
                    foreach (var kv in TypeCacheRanges.Where(kv => kv.Value.DatFileType == datType)) {
                        if (id >= kv.Value.FirstId && id <= kv.Value.LastId) {
                            return kv.Key;
                        }
                    }
                    break;
            }

            return null;
        }

        internal static DBObjType DBObjTypeFromId(DatFileType datType, uint id) {
            var type = TypeFromId(datType, id);

            if (type is not null && TypeCache.TryGetValue(type, out var attr)) {
                return attr.DBObjType;
            }

            return DBObjType.Unknown;
        }

        private static Dictionary<Type, DBObjTypeAttribute> InitTypeCache() {
            var typeCache = new Dictionary<Type, DBObjTypeAttribute>();
            foreach (var type in typeof(DBObjTypeAttribute).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(DBObj)))) {
                if (type.GetCustomAttributes(typeof(DBObjTypeAttribute), true).Length > 0) {
                    var attr = (DBObjTypeAttribute)type.GetCustomAttributes(typeof(DBObjTypeAttribute), true)[0];
                    typeCache.Add(type, attr);
                }
            }

            return typeCache;
        }
    }
}
