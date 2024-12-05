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
    /// DB_TYPE_SKILL_TABLE_0 in the client.
    /// </summary>
    [DBObjType(typeof(SkillTable), DatFileType.Portal, DBObjType.SkillTable, DBObjHeaderFlags.HasId, 0x0E000004, 0x0E000004, 0x00000000)]
    public partial class SkillTable : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.SkillTable;

        public Dictionary<SkillId, SkillBase> Skills = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            var _numSkills = reader.ReadUInt16();
            var _numSkillBuckets = reader.ReadUInt16();
            for (var i=0; i < _numSkills; i++) {
                var _key = (SkillId)reader.ReadInt32();
                var _val = reader.ReadItem<SkillBase>();
                Skills.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt16((ushort)Skills.Count());
            writer.WriteUInt16(32);
            foreach (var kv in Skills) {
                writer.WriteInt32((int)kv.Key);
                writer.WriteItem<SkillBase>(kv.Value);
            }
            return true;
        }

    }

}
