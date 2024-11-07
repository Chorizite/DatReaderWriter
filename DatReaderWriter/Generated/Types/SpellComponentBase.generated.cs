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
    /// <summary>
    /// Information about a spell component
    /// </summary>
    public partial class SpellComponentBase : IDatObjType {
        public string Name;

        public uint Category;

        public uint Icon;

        public ComponentType Type;

        public uint Gesture;

        public float Time;

        public string Text;

        public float CDM;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Name = reader.ReadObfuscatedString();
            reader.Align(4);
            Category = reader.ReadUInt32();
            Icon = reader.ReadUInt32();
            Type = (ComponentType)reader.ReadUInt32();
            Gesture = reader.ReadUInt32();
            Time = reader.ReadSingle();
            Text = reader.ReadObfuscatedString();
            reader.Align(4);
            CDM = reader.ReadSingle();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteObfuscatedString(Name);
            writer.Align(4);
            writer.WriteUInt32(Category);
            writer.WriteUInt32(Icon);
            writer.WriteUInt32((uint)Type);
            writer.WriteUInt32(Gesture);
            writer.WriteSingle(Time);
            writer.WriteObfuscatedString(Text);
            writer.Align(4);
            writer.WriteSingle(CDM);
            return true;
        }

    }

}
