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
using DatReaderWriter.Types;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.DBObjs {
    /// <summary>
    /// DB_TYPE_ATTRIBUTE_2ND_TABLE_0 in the client.
    /// </summary>
    [DBObjType(typeof(VitalTable), DatFileType.Portal, DBObjType.VitalTable, DBObjHeaderFlags.HasId, 0x0E000003, 0x0E000003, 0x00000000)]
    public partial class VitalTable : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.VitalTable;

        public SkillFormula Health;

        public SkillFormula Stamina;

        public SkillFormula Mana;

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            Health = reader.ReadItem<SkillFormula>();
            Stamina = reader.ReadItem<SkillFormula>();
            Mana = reader.ReadItem<SkillFormula>();
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteItem<SkillFormula>(Health);
            writer.WriteItem<SkillFormula>(Stamina);
            writer.WriteItem<SkillFormula>(Mana);
            return true;
        }

    }

}
