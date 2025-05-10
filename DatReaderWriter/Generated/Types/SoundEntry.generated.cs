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
    public partial class SoundEntry : IDatObjType {
        /// <summary>
        /// A 0x0A sound entry
        /// </summary>
        public uint Id;

        public float Priority;

        public float Probability;

        public float Volume;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Id = reader.ReadUInt32();
            Priority = reader.ReadSingle();
            Probability = reader.ReadSingle();
            Volume = reader.ReadSingle();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(Id);
            writer.WriteSingle(Priority);
            writer.WriteSingle(Probability);
            writer.WriteSingle(Volume);
            return true;
        }

    }

}
