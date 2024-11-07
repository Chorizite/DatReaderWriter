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
    public partial class AnimData : IDatObjType {
        public uint AnimId;

        public int LowFrame;

        public int HighFrame;

        public float Framerate;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            AnimId = reader.ReadUInt32();
            LowFrame = reader.ReadInt32();
            HighFrame = reader.ReadInt32();
            Framerate = reader.ReadSingle();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(AnimId);
            writer.WriteInt32(LowFrame);
            writer.WriteInt32(HighFrame);
            writer.WriteSingle(Framerate);
            return true;
        }

    }

}
