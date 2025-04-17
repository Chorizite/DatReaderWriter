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
    /// DB_TYPE_PHYSICS_SCRIPT_TABLE in the client.
    /// </summary>
    [DBObjType(typeof(PhysicsScriptTable), DatFileType.Portal, DBObjType.PhysicsScriptTable, DBObjHeaderFlags.HasId, 0x34000000, 0x3400FFFF, 0x00000000)]
    public partial class PhysicsScriptTable : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.PhysicsScriptTable;

        public Dictionary<PlayScript, PhysicsScriptTableData> ScriptTable = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            var _numPhysicsScriptTables = reader.ReadUInt32();
            for (var i=0; i < _numPhysicsScriptTables; i++) {
                var _key = (PlayScript)reader.ReadUInt32();
                var _val = reader.ReadItem<PhysicsScriptTableData>();
                ScriptTable.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32((uint)ScriptTable.Count());
            foreach (var kv in ScriptTable) {
                writer.WriteUInt32((uint)kv.Key);
                writer.WriteItem<PhysicsScriptTableData>(kv.Value);
            }
            return true;
        }

    }

}
