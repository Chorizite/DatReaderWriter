using DatReaderWriter.Lib.IO;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace DatReaderWriter.Types {
    public abstract class QualifiedDataId {
        // implicit conversion to uint
        public static implicit operator uint(QualifiedDataId qualifiedDataId) => qualifiedDataId.DataId;
        
        /// <summary>
        /// The id of the data object.
        /// </summary>
        public uint DataId { get; set; }
    }
    
    /// <summary>
    /// A qualified data ID referencing a DBObj of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QualifiedDataId<T> : QualifiedDataId, IUnpackable, IPackable, IEquatable<uint> where T : DBObj {
        public static implicit operator QualifiedDataId<T>(uint dataId) => new QualifiedDataId<T> { DataId = dataId };
        public static implicit operator QualifiedDataId<T>(int dataId) => new QualifiedDataId<T> { DataId = (uint)dataId };

        /// <summary>
        /// Gets the data object for this qualified data ID from the given DatCollection.
        /// </summary>
        /// <param name="datCollection"></param>
        /// <returns></returns>
        public T Get(DatCollection datCollection) {
            return datCollection.Get<T>(DataId);
        }

        /// <summary>
        /// Try and get the data object for this qualified data ID from the given DatCollection.
        /// </summary>
        /// <typeparam name="T">The dat file type</typeparam>
        /// <param name="datCollection">The dat collection to read from</param>
        /// <param name="value">The unpacked file</param>
        /// <returns></returns>
#if (NET8_0_OR_GREATER)
        public bool TryGet<T>(DatCollection datCollection, [MaybeNullWhen(false)] out T value) where T : IDBObj {
#else
        public bool TryGet<T>(DatCollection datCollection, out T value) where T : IDBObj {
#endif
            return datCollection.TryGet<T>(DataId, out value);
        }
        
        
#if (NET8_0_OR_GREATER)
        public async ValueTask<(bool Success, T? Value)> TryGetAsync<T>(DatCollection datCollection, CancellationToken ct = default)
            where T : IDBObj {
#else
        public async Task<(bool Success, T Value)> TryGetAsync<T>(DatCollection datCollection, CancellationToken ct =
            default) where T : IDBObj {
#endif
                return await datCollection.TryGetAsync<T>(DataId, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            DataId = reader.ReadUInt32();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(DataId);
            return true;
        }

        public bool Equals(uint other)
        {
            return DataId == other;
        }

        public override bool Equals(object obj)
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

            return Equals((QualifiedDataId<T>)obj);
        }

        public override int GetHashCode()
        {
            return (int)DataId;
        }

        public override string ToString() {
            return $"0x{DataId:X8}({typeof(T).Name})";
        }
    }
}