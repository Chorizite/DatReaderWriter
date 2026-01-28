using System;
using System.Collections.Generic;
using System.Linq;
using DatReaderWriter.Lib.IO;

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
        public static int GetBucketSizeIndex(int numEntries, bool isAutoGrow = false) {
            var bucketSize = GetBucketSize(numEntries, isAutoGrow);
            return Array.IndexOf(BucketSizes, bucketSize);
        }
    }

    public static class DatReaderWriterExtensions {
        public static T ReadGeneric<T>(this DatBinReader reader) {
            var type = typeof(T);
            if (type == typeof(uint)) return (T)(object)reader.ReadUInt32();
            if (type == typeof(int)) return (T)(object)reader.ReadInt32();
            if (type == typeof(ulong)) return (T)(object)reader.ReadUInt64();
            if (type == typeof(long)) return (T)(object)reader.ReadInt64();
            if (type == typeof(ushort)) return (T)(object)reader.ReadUInt16();
            if (type == typeof(short)) return (T)(object)reader.ReadInt16();
            if (type == typeof(byte)) return (T)(object)reader.ReadByte();
            if (type == typeof(sbyte)) return (T)(object)reader.ReadSByte();
            if (type == typeof(bool)) return (T)(object)reader.ReadBool();
            if (type == typeof(float)) return (T)(object)reader.ReadSingle();
            if (type == typeof(double)) return (T)(object)reader.ReadDouble();
            if (type == typeof(string)) return (T)(object)reader.ReadString16L();
            if (type == typeof(Guid)) return (T)(object)reader.ReadGuid();

            if (typeof(IUnpackable).IsAssignableFrom(type)) {
                var item = (IUnpackable)Activator.CreateInstance(type);
                item.Unpack(reader);
                return (T)item;
            }

            throw new NotSupportedException($"Type {type.Name} is not supported by ReadGeneric.");
        }

        public static void WriteGeneric<T>(this DatBinWriter writer, T value) {
            var type = typeof(T);
            if (type == typeof(uint)) writer.WriteUInt32((uint)(object)value);
            else if (type == typeof(int)) writer.WriteInt32((int)(object)value);
            else if (type == typeof(ulong)) writer.WriteUInt64((ulong)(object)value);
            else if (type == typeof(long)) writer.WriteInt64((long)(object)value);
            else if (type == typeof(ushort)) writer.WriteUInt16((ushort)(object)value);
            else if (type == typeof(short)) writer.WriteInt16((short)(object)value);
            else if (type == typeof(byte)) writer.WriteByte((byte)(object)value);
            else if (type == typeof(sbyte)) writer.WriteSByte((sbyte)(object)value);
            else if (type == typeof(bool)) writer.WriteBool((bool)(object)value);
            else if (type == typeof(float)) writer.WriteSingle((float)(object)value);
            else if (type == typeof(double)) writer.WriteDouble((double)(object)value);
            else if (type == typeof(string)) writer.WriteString16L((string)(object)value);
            else if (type == typeof(Guid)) writer.WriteGuid((Guid)(object)value);
            else if (value is IPackable packable) {
                packable.Pack(writer);
            }
            else {
                throw new NotSupportedException($"Type {type.Name} is not supported by WriteGeneric.");
            }
        }
    }
}
