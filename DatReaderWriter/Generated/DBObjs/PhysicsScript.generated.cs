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
    /// DB_TYPE_PHYSICS_SCRIPT in the client.
    /// </summary>
    [DBObjType(typeof(PhysicsScript), DatFileType.Portal, DBObjType.PhysicsScript, DBObjHeaderFlags.HasId, 0x33000000, 0x3300FFFF, 0x00000000)]
    public partial class PhysicsScript : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.PhysicsScript;

        public List<PhysicsScriptData> ScriptData = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            var _numScriptDatas = reader.ReadUInt32();
            for (var i=0; i < _numScriptDatas; i++) {
                ScriptData.Add(reader.ReadItem<PhysicsScriptData>());
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32((uint)ScriptData.Count());
            foreach (var item in ScriptData) {
                writer.WriteItem<PhysicsScriptData>(item);
            }
            return true;
        }

    }

}
