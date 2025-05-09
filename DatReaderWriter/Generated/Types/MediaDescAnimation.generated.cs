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
    public partial class MediaDescAnimation : MediaDesc {
        /// <inheritdoc />
        public override MediaType MediaType => MediaType.Animation;

        public float Duration;

        public uint DrawMode;

        public List<uint> Frames = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            Duration = reader.ReadSingle();
            DrawMode = reader.ReadUInt32();
            var _numValues = reader.ReadUInt32();
            for (var i=0; i < _numValues; i++) {
                Frames.Add(reader.ReadUInt32());
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteSingle(Duration);
            writer.WriteUInt32(DrawMode);
            writer.WriteUInt32((uint)Frames.Count());
            foreach (var item in Frames) {
                writer.WriteUInt32(item);
            }
            return true;
        }

    }

}
