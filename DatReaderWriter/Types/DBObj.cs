using System;
using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.IO;

namespace ACClientLib.DatReaderWriter.Types {
    /// <summary>
    /// Base class for all DBObjs
    /// </summary>
    public abstract class DBObj : IDBObj {

        /// <inheritdoc />
        public abstract DBObjHeaderFlags HeaderFlags { get; }

        /// <inheritdoc />
        /// <remarks>
        /// Only valid if <see cref="HeaderFlags"/> has <see cref="DBObjHeaderFlags.HasId"/>.
        /// </remarks>
        public uint Id { get; set; }

        /// <remarks>
        /// Only valid if <see cref="HeaderFlags"/> has <see cref="DBObjHeaderFlags.HasDataCategory"/>.
        /// </remarks>
        public uint DataCategory { get; set; }

        /// <inheritdoc />
        public virtual bool Unpack(DatFileReader reader) {
            if (HeaderFlags.HasFlag(DBObjHeaderFlags.HasId)) {
                Id = reader.ReadUInt32();
            }
            if (HeaderFlags.HasFlag(DBObjHeaderFlags.HasDataCategory)) {
                DataCategory = reader.ReadUInt32();
            }
            return true;
        }

        /// <inheritdoc />
        public virtual bool Pack(DatFileWriter writer) {
            if (HeaderFlags.HasFlag(DBObjHeaderFlags.HasId)) {
                writer.WriteUInt32(Id);
            }
            if (HeaderFlags.HasFlag(DBObjHeaderFlags.HasDataCategory)) {
                writer.WriteUInt32(DataCategory);
            }
            return true;
        }

    }

}
