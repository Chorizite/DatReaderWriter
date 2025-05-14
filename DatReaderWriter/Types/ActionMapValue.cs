using System;
using System.Numerics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DatReaderWriter.Enums;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.DBObjs;

namespace DatReaderWriter.Types {
    /// <summary>
    /// Represents a value in an action map
    /// </summary>
    public partial class ActionMapValue : IDatObjType {
        /// <summary>
        /// The magic value, this is always 0
        /// </summary>
        public uint Magic { get; set; }

        /// <summary>
        /// This is always 0, hardcoded in the client
        /// </summary>
        public byte Unknown { get; set; }

        public ToggleType ToggleType { get; set; }

        /// <summary>
        /// Always 0? This appears to be the length of a dummy list that is always 0
        /// </summary>
        public uint DummyListLength { get; set; }

        public UserBindingData UserBinding { get; set; }

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Magic = reader.ReadUInt32();
            Unknown = reader.ReadByte();
            ToggleType = (ToggleType)reader.ReadUInt32();
            DummyListLength = reader.ReadUInt32();
            UserBinding = reader.ReadItem<UserBindingData>();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(Magic);
            writer.WriteByte(Unknown);
            writer.WriteUInt32((uint)ToggleType);
            writer.WriteUInt32(DummyListLength);
            UserBinding?.Pack(writer);
            return true;
        }

    }

}
