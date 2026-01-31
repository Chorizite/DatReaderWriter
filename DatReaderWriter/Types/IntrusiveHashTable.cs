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
    public class IntrusiveHashTable<TKey, TValue> : Dictionary<TKey, TValue>, IUnpackable, IPackable {
        /// <summary>
        /// The size of the hash table buckets.
        /// </summary>
        public byte BucketSizeIndex { get; set; } = 1;

        public bool Unpack(DatBinReader reader) {
            BucketSizeIndex = reader.ReadByte();
            var numElements = reader.ReadCompressedUInt();
            for (var i = 0; i < numElements; i++) {
                var key = reader.ReadGeneric<TKey>();
                var val = reader.ReadGeneric<TValue>();
                this.Add(key, val);
            }
            return true;
        }

        public bool Pack(DatBinWriter writer) {
            writer.WriteByte(BucketSizeIndex);
            writer.WriteCompressedUInt((uint)this.Count);
            
            var bucketSize = HashTableHelpers.BucketSizes[BucketSizeIndex];
            
            // Sort by key modulus bucketSize
            var sortedItems = this
                .OrderBy(x => HashTableHelpers.GetHashKey(x.Key) % (ulong)bucketSize);
            
            foreach (var kvp in sortedItems) {
                writer.WriteGeneric(kvp.Key);
                writer.WriteGeneric(kvp.Value);
            }
            return true;
        }
    }
}