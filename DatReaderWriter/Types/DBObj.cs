using System;
using ACClientLib.DatReaderWriter.IO;

namespace ACClientLib.DatReaderWriter.Types {
    /// <summary>
    /// Base class for all DBObjs
    /// </summary>
    public abstract class DBObj : IDBObj {
        /// <inheritdoc />
        public uint Id { get; set; }

        /// <inheritdoc />
        public abstract bool HasDataCategory { get; }

        /// <summary>
        /// Only valid if <see cref="HasDataCategory"/> is true.
        /// </summary>
        public uint DataCategory { get; set; }

        /// <inheritdoc />
        public virtual bool Unpack(DatFileReader reader) {
            Id = reader.ReadUInt32();
            if (HasDataCategory) {
                DataCategory = reader.ReadUInt32();
            }
            return true;
        }

        /// <inheritdoc />
        public virtual bool Pack(DatFileWriter writer) {
            writer.WriteUInt32(Id);
            if (HasDataCategory) {
                writer.WriteUInt32(DataCategory);
            }
            return true;
        }

    }

}
