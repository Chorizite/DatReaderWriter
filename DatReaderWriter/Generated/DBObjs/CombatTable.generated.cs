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
    /// DB_TYPE_COMBAT_TABLE_0 in the client.
    /// </summary>
    [DBObjType(typeof(CombatTable), DatFileType.Portal, DBObjType.CombatTable, DBObjHeaderFlags.HasId, 0x30000000, 0x3000FFFF, 0x00000000)]
    public partial class CombatTable : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.CombatTable;

        public List<CombatManeuver> CombatManeuvers = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            var _numCombatManeuvers = reader.ReadUInt32();
            for (var i=0; i < _numCombatManeuvers; i++) {
                CombatManeuvers.Add(reader.ReadItem<CombatManeuver>());
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32((uint)CombatManeuvers.Count());
            foreach (var item in CombatManeuvers) {
                writer.WriteItem<CombatManeuver>(item);
            }
            return true;
        }

    }

}
