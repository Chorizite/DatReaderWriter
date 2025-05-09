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
    public partial class MediaDescState : MediaDesc {
        /// <inheritdoc />
        public override MediaType MediaType => MediaType.State;

        public uint StateId;

        public float Probability;

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            StateId = reader.ReadUInt32();
            Probability = reader.ReadSingle();
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32(StateId);
            writer.WriteSingle(Probability);
            return true;
        }

    }

}
