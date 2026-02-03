using DatReaderWriter.Lib;
using DatReaderWriter.Lib.IO;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace DatReaderWriter.Types {
    public abstract class PackedQualifiedDataId {
        // implicit conversion to uint
        public static implicit operator uint(PackedQualifiedDataId qualifiedDataId) => qualifiedDataId.DataId;
        
        /// <summary>
        /// The id of the data object.
        /// </summary>
        public uint DataId { get; set; }
    }
    
    /// <summary>
    /// A qualified data ID referencing a DBObj of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PackedQualifiedDataId<T> : PackedQualifiedDataId, IUnpackable, IPackable, IEquatable<uint> where T : DBObj {
        public static implicit operator PackedQualifiedDataId<T>(uint dataId) => new PackedQualifiedDataId<T> { DataId = dataId };
        public static implicit operator PackedQualifiedDataId<T>(int dataId) => new PackedQualifiedDataId<T> { DataId = (uint)dataId };

        /// <summary>
        /// Gets the data object for this qualified data ID from the given DatCollection.
        /// </summary>
        /// <param name="datCollection"></param>
        /// <returns></returns>
        public T? Get(DatCollection datCollection) {
            return datCollection.Get<T>(DataId);
        }

        /// <summary>
        /// Try and get the data object for this qualified data ID from the given DatCollection.
        /// </summary>
        /// <typeparam name="TDBObj">The dat file type</typeparam>
        /// <param name="datCollection">The dat collection to read from</param>
        /// <param name="value">The unpacked file</param>
        /// <returns></returns>
#if (NET8_0_OR_GREATER)
        public bool TryGet(DatCollection datCollection, [MaybeNullWhen(false)] out T value) {
#else
        public bool TryGet(DatCollection datCollection, out T value) {
#endif
            return datCollection.TryGet<T>(DataId, out value);
        }
        
        
#if (NET8_0_OR_GREATER)
        public async ValueTask<(bool Success, T? Value)> TryGetAsync(DatCollection datCollection, CancellationToken ct = default) {
#else
        public async Task<(bool Success, T? Value)> TryGetAsync(DatCollection datCollection, CancellationToken ct =
            default) {
#endif
                return await datCollection.TryGetAsync<T>(DataId, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            var mask = DBObjAttributeCache.MaskFromType(typeof(T));
            DataId = reader.ReadDataIdOfKnownType(mask);
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            var mask = DBObjAttributeCache.MaskFromType(typeof(T));
            writer.WriteDataIdOfKnownType(DataId, mask);
            return true;
        }

        public bool Equals(uint other)
        {
            return DataId == other;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }
            
            if (obj is uint dataId) {
                return DataId == dataId;
            }

            if (obj.GetType() != GetType()) {
                return false;
            }

            return Equals((PackedQualifiedDataId<T>)obj);
        }

        public override int GetHashCode()
        {
            return (int)DataId.GetHashCode();
        }

        public override string ToString() {
            return $"0x{DataId:X8}({typeof(T).Name})";
        }
    }
}