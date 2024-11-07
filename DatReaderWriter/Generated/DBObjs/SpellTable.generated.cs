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
    /// DB_TYPE_SPELL_TABLE_0 in the client.
    /// </summary>
    [DBObjType(typeof(SpellTable), DatFileType.Portal, DBObjType.SpellTable, DBObjHeaderFlags.HasId, 0x0E00000E, 0x0E00000E, 0x00000000)]
    public partial class SpellTable : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.SpellTable;

        public Dictionary<uint, SpellBase> Spells = [];

        public Dictionary<EquipmentSet, SpellSet> SpellsSets = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            var _numSpells = reader.ReadUInt16();
            var _numSpellBuckets = reader.ReadUInt16();
            for (var i=0; i < _numSpells; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadItem<SpellBase>();
                Spells.Add(_key, _val);
            }
            var _numSpellSets = reader.ReadUInt16();
            var _numSpellSetBuckets = reader.ReadUInt16();
            for (var i=0; i < _numSpellSets; i++) {
                var _key = (EquipmentSet)reader.ReadInt32();
                var _val = reader.ReadItem<SpellSet>();
                SpellsSets.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt16((ushort)Spells.Count());
            writer.WriteUInt16(8192);
            foreach (var kv in Spells) {
                writer.WriteUInt32(kv.Key);
                writer.WriteItem<SpellBase>(kv.Value);
            }
            writer.WriteUInt16((ushort)SpellsSets.Count());
            writer.WriteUInt16(256);
            foreach (var kv in SpellsSets) {
                writer.WriteInt32((int)kv.Key);
                writer.WriteItem<SpellSet>(kv.Value);
            }
            return true;
        }

    }

}
