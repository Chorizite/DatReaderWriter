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
    public partial class TerrainTex : IDatObjType {
        public uint TexGID;

        public uint TexTiling;

        public uint MaxVertBright;

        public uint MinVertBright;

        public uint MaxVertSaturate;

        public uint MinVertSaturate;

        public uint MaxVertHue;

        public uint MinVertHue;

        public uint DetailTexTiling;

        public uint DetailTexGID;

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            TexGID = reader.ReadUInt32();
            TexTiling = reader.ReadUInt32();
            MaxVertBright = reader.ReadUInt32();
            MinVertBright = reader.ReadUInt32();
            MaxVertSaturate = reader.ReadUInt32();
            MinVertSaturate = reader.ReadUInt32();
            MaxVertHue = reader.ReadUInt32();
            MinVertHue = reader.ReadUInt32();
            DetailTexTiling = reader.ReadUInt32();
            DetailTexGID = reader.ReadUInt32();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteUInt32(TexGID);
            writer.WriteUInt32(TexTiling);
            writer.WriteUInt32(MaxVertBright);
            writer.WriteUInt32(MinVertBright);
            writer.WriteUInt32(MaxVertSaturate);
            writer.WriteUInt32(MinVertSaturate);
            writer.WriteUInt32(MaxVertHue);
            writer.WriteUInt32(MinVertHue);
            writer.WriteUInt32(DetailTexTiling);
            writer.WriteUInt32(DetailTexGID);
            return true;
        }

    }

}
