//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//
//                                                            //
//                          WARNING                           //
//                                                            //
//           DO NOT MAKE LOCAL CHANGES TO THIS FILE           //
//               EDIT THE .tt TEMPLATE INSTEAD                //
//                                                            //
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//


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
    public partial class StringInfo : IDatObjType {
        public byte Token;

        /// <summary>
        /// This is the hashed string value, which is the key in TableId
        /// </summary>
        public uint StringId;

        /// <summary>
        /// The StringTable (0x23) used to look up the StringId in
        /// </summary>
        public uint TableId;

        public StringInfoOverrideFlag Override;

        public byte English;

        public byte Comment;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Token = reader.ReadByte();
            StringId = reader.ReadUInt32();
            TableId = reader.ReadUInt32();
            Override = (StringInfoOverrideFlag)reader.ReadByte();
            English = reader.ReadByte();
            Comment = reader.ReadByte();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteByte(Token);
            writer.WriteUInt32(StringId);
            writer.WriteUInt32(TableId);
            writer.WriteByte((byte)Override);
            writer.WriteByte(English);
            writer.WriteByte(Comment);
            return true;
        }

    }

}
