using DatReaderWriter.Lib;
using DatReaderWriter.Lib.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatReaderWriter.Types {
    /// <summary>
    /// A hash table that supports packing and unpacking.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class PackableHashTable<TKey, TValue> : Dictionary<TKey, TValue>, IUnpackable, IPackable {
        /// <summary>
        /// The size of the hash table buckets.
        /// </summary>
        public ushort BucketSize { get; set; } = 32;

        public bool Unpack(DatBinReader reader) {
            var numElements = reader.ReadUInt16();
            BucketSize = reader.ReadUInt16();
            Console.WriteLine($"Read BucketSize: {BucketSize}");
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
            
            // Sort by key modulus bucketSize
            var sortedItems = this
                .OrderBy(x => HashTableHelpers.GetHashKey(x.Key) % (ulong)BucketSize);
            
            foreach (var kvp in sortedItems) {
                writer.WriteGeneric(kvp.Key);
                writer.WriteGeneric(kvp.Value);
            }
            return true;
        }
    }
}