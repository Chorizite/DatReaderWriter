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
    public partial class AttackCone : IDatObjType {
        public uint PartIndex;

        public float LeftX;

        public float LeftY;

        public float RightX;

        public float RightY;

        public float Radius;

        public float Height;

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            PartIndex = reader.ReadUInt32();
            LeftX = reader.ReadSingle();
            LeftY = reader.ReadSingle();
            RightX = reader.ReadSingle();
            RightY = reader.ReadSingle();
            Radius = reader.ReadSingle();
            Height = reader.ReadSingle();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteUInt32(PartIndex);
            writer.WriteSingle(LeftX);
            writer.WriteSingle(LeftY);
            writer.WriteSingle(RightX);
            writer.WriteSingle(RightY);
            writer.WriteSingle(Radius);
            writer.WriteSingle(Height);
            return true;
        }

    }

}
