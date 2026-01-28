using DatReaderWriter.Lib;
using DatReaderWriter.Lib.IO;
using System.Collections.Generic;
using System.Linq;

namespace DatReaderWriter.Types {
    /// <summary>
    /// A hash table that can automatically grow and supports packing and unpacking.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class AutoGrowHashTable<TKey, TValue> : Dictionary<TKey, TValue>, IUnpackable, IPackable where TKey : unmanaged {
        public bool Unpack(DatBinReader reader) {
            _ = reader.ReadByte(); // bucket size index, not used
            var numElements = reader.ReadCompressedUInt();
            for (var i = 0; i < numElements; i++) {
                var key = reader.ReadGeneric<TKey>();
                var val = reader.ReadGeneric<TValue>();
                this.Add(key, val);
            }
            return true;
        }

        public bool Pack(DatBinWriter writer) {
            var bucketSizeIndex = HashTableHelpers.GetBucketSizeIndex(Count, true);
            writer.WriteByte(bucketSizeIndex);
            writer.WriteCompressedUInt((uint)this.Count);
            
            var bucketSize = HashTableHelpers.BucketSizes[bucketSizeIndex];
            
            // Sort by key modulus bucketSize
            var sortedItems = this
                .OrderBy(x => ConvertToUlong(x.Key) % (ulong)bucketSize);
            
            foreach (var kvp in sortedItems) {
                writer.WriteGeneric(kvp.Key);
                writer.WriteGeneric(kvp.Value);
            }
            return true;
        }
        
        private static ulong ConvertToUlong(TKey key) {
            unsafe {
                return sizeof(TKey) switch {
                    1 => *(byte*)&key,
                    2 => *(ushort*)&key,
                    4 => *(uint*)&key,
                    8 => *(ulong*)&key,
                    _ => throw new System.NotSupportedException(
                        $"Key type size {sizeof(TKey)} not supported for modulus operation")
                };
            }
        }
    }
}