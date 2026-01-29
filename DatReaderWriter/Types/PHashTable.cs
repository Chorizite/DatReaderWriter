using DatReaderWriter.Lib;
using DatReaderWriter.Lib.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatReaderWriter.Types {
    /// <summary>
    /// A hash table that supports packing and unpacking. Does not sort entries when packing.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class PHashTable<TKey, TValue> : Dictionary<TKey, TValue>, IUnpackable, IPackable {
        /// <summary>
        /// The size of the hash table buckets.
        /// </summary>
        public ushort BucketSize { get; set; } = 256;

        public bool Unpack(DatBinReader reader) {
            var numElements = reader.ReadUInt16();
            BucketSize = reader.ReadUInt16();
            for (var i = 0; i < numElements; i++) {
                var key = reader.ReadGeneric<TKey>();
                var val = reader.ReadGeneric<TValue>();
                this.Add(key, val);
            }
            return true;
        }

        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt16((ushort)this.Count);
            writer.WriteUInt16((ushort)BucketSize);
            
            foreach (var kvp in this) {
                writer.WriteGeneric(kvp.Key);
                writer.WriteGeneric(kvp.Value);
            }
            return true;
        }
    }
}