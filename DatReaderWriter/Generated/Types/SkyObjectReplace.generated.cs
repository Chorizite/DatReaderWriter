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
    public partial class SkyObjectReplace : IDatObjType {
        public uint ObjectIndex;

        public uint GFXObjId;

        public float Rotate;

        public float Transparent;

        public float Luminosity;

        public float MaxBright;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            ObjectIndex = reader.ReadUInt32();
            GFXObjId = reader.ReadUInt32();
            Rotate = reader.ReadSingle();
            Transparent = reader.ReadSingle();
            Luminosity = reader.ReadSingle();
            MaxBright = reader.ReadSingle();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(ObjectIndex);
            writer.WriteUInt32(GFXObjId);
            writer.WriteSingle(Rotate);
            writer.WriteSingle(Transparent);
            writer.WriteSingle(Luminosity);
            writer.WriteSingle(MaxBright);
            return true;
        }

    }

}
