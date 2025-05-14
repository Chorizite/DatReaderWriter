using System;
using System.Numerics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DatReaderWriter.Enums;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.Types {
    /// <summary>
    /// User binding value used in ActionMap
    /// </summary>
    public partial class UserBindingData : IDatObjType {
        public uint ActionClass;

        /// <summary>
        /// This can be looked up in the ActionMap StringTable
        /// </summary>
        public uint ActionName;

        /// <summary>
        /// This can be looked up in the ActionMap StringTable
        /// </summary>
        public uint ActionDescription;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            ActionClass = reader.ReadUInt32();
            ActionName = reader.ReadUInt32();
            ActionDescription = reader.ReadUInt32();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(ActionClass);
            writer.WriteUInt32(ActionName);
            writer.WriteUInt32(ActionDescription);
            return true;
        }

    }

}
