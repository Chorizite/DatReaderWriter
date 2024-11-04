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
using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.IO;

namespace ACClientLib.DatReaderWriter.Types {
    public partial class SkyObject : IDatObjType {
        public float BeginTime;

        public float EndTime;

        public float BeginAngle;

        public float EndAngle;

        public float TexVelocityX;

        public float TexVelocityY;

        public uint DefaultGFXObjectId;

        public uint DefaultPESObjectId;

        public uint Properties;

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            BeginTime = reader.ReadSingle();
            EndTime = reader.ReadSingle();
            BeginAngle = reader.ReadSingle();
            EndAngle = reader.ReadSingle();
            TexVelocityX = reader.ReadSingle();
            TexVelocityY = reader.ReadSingle();
            DefaultGFXObjectId = reader.ReadUInt32();
            DefaultPESObjectId = reader.ReadUInt32();
            Properties = reader.ReadUInt32();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteSingle(BeginTime);
            writer.WriteSingle(EndTime);
            writer.WriteSingle(BeginAngle);
            writer.WriteSingle(EndAngle);
            writer.WriteSingle(TexVelocityX);
            writer.WriteSingle(TexVelocityY);
            writer.WriteUInt32(DefaultGFXObjectId);
            writer.WriteUInt32(DefaultPESObjectId);
            writer.WriteUInt32(Properties);
            return true;
        }

    }

}
