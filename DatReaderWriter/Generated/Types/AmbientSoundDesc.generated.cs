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
    public class AmbientSoundDesc : IDatObjType {
        public uint SType;

        public float Volume;

        public float BaseChance;

        public float MinRate;

        public float MaxRate;

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            SType = reader.ReadUInt32();
            Volume = reader.ReadSingle();
            BaseChance = reader.ReadSingle();
            MinRate = reader.ReadSingle();
            MaxRate = reader.ReadSingle();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteUInt32(SType);
            writer.WriteSingle(Volume);
            writer.WriteSingle(BaseChance);
            writer.WriteSingle(MinRate);
            writer.WriteSingle(MaxRate);
            return true;
        }

    }

}
