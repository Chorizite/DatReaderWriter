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
    /// DB_TYPE_SPELLCOMPONENT_TABLE_0 in the client.
    /// </summary>
    [DBObjType(typeof(SpellComponentTable), DatFileType.Portal, DBObjType.SpellComponentTable, DBObjHeaderFlags.HasId, 0x0E00000F, 0x0E00000F, 0x00000000)]
    public partial class SpellComponentTable : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.SpellComponentTable;

        public Dictionary<uint, SpellComponentBase> Components = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            var _numComponents = reader.ReadUInt16();
            reader.Align(4);
            for (var i=0; i < _numComponents; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadItem<SpellComponentBase>();
                Components.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt16((ushort)Components.Count());
            writer.Align(4);
            foreach (var kv in Components) {
                writer.WriteUInt32(kv.Key);
                writer.WriteItem<SpellComponentBase>(kv.Value);
            }
            return true;
        }

    }

}
