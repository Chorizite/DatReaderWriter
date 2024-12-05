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
    public partial class SkillCG : IDatObjType {
        public SkillId Id;

        public int NormalCost;

        public int PrimaryCost;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Id = (SkillId)reader.ReadInt32();
            NormalCost = reader.ReadInt32();
            PrimaryCost = reader.ReadInt32();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteInt32((int)Id);
            writer.WriteInt32(NormalCost);
            writer.WriteInt32(PrimaryCost);
            return true;
        }

    }

}
