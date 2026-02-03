using System;
using System.Collections.Generic;
using System.Linq;
using DatReaderWriter.Types;

namespace DatReaderWriter.Lib {
    /// <summary>
    /// Helpers for hash table bucket sizes.
    /// </summary>
    public static class HashTableHelpers {
        /// <summary>
        /// A list of prime bucket sizes used for hash tables.
        /// </summary>
        public static readonly int[] BucketSizes = new int[] {
            11, 23, 47, 89, 191, 383, 761, 1531, 3067, 6143, 12281, 24571, 49139, 98299, 196597, 393209, 786431,
            1572853, 3145721, 6291449, 12582893, 25165813, 50331599
        };

        /// <summary>
        /// Get the appropriate bucket size for a given number of entries.
        /// </summary>
        /// <param name="entryCount">Number of entries in the hashtable</param>
        /// <param name="isAutoGrow">If this is an auto-growing hash table.</param>
        /// <returns></returns>
        public static int GetBucketSize(int entryCount, bool isAutoGrow = false) {
            if (isAutoGrow) {
                // elements + 1 > 2 * buckets
                foreach (var size in BucketSizes) {
                    if (entryCount + 1 > 2 * size)
                        continue;
                    return size;
                }
            }
            else {
                // pick the smallest bucket
                foreach (var size in BucketSizes) {
                    if (size >= entryCount)
                        return size;
                }
            }

            return BucketSizes[BucketSizes.Length - 1]; // largest
        }

        /// <summary>
        /// Gets the bucket size index in <see cref="BucketSizes"/> for a given number of hashtable entries.
        /// </summary>
        /// <param name="numEntries">The number of entries in the hash table</param>
        /// <param name="isAutoGrow">If this is an auto-growing hash table.</param>
        /// <returns>The index of the calculated bucket size in the <see cref="BucketSizes"/> table</returns>
        public static byte GetBucketSizeIndex(int numEntries, bool isAutoGrow = false) {
            var bucketSize = GetBucketSize(numEntries, isAutoGrow);
            return (byte)Array.IndexOf(BucketSizes, bucketSize);
        }
        
        public static ulong GetHashKey<TKey>(TKey key) {
            unsafe {
                if (key is QualifiedDataId qdid) {
                    return qdid.DataId;
                }

                if (key is StringBase strKey) {
                    return (ulong)strKey.GetHashCode();
                }
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
                return sizeof(TKey) switch {
                    1 => *(byte*)&key,
                    2 => *(ushort*)&key,
                    4 => *(uint*)&key,
                    8 => *(ulong*)&key,
                    _ => throw new System.NotSupportedException(
                        $"Key type size {sizeof(TKey)} not supported for modulus operation")
                };
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
            }
        }
    }
}
