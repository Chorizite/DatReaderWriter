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
    public partial class SkyObjectReplace : IDatObjType {
        public uint ObjectIndex;

        public uint GFXObjId;

        public float Rotate;

        public float Transparent;

        public float Luminosity;

        public float MaxBright;

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            ObjectIndex = reader.ReadUInt32();
            GFXObjId = reader.ReadUInt32();
            Rotate = reader.ReadSingle();
            Transparent = reader.ReadSingle();
            Luminosity = reader.ReadSingle();
            MaxBright = reader.ReadSingle();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
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
