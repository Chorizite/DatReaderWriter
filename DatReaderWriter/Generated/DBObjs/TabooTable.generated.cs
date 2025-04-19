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
    /// DB_TYPE_TABOO_TABLE in the client.
    /// </summary>
    [DBObjType(typeof(TabooTable), DatFileType.Portal, DBObjType.TabooTable, DBObjHeaderFlags.HasId, 0x0E00001E, 0x0E00001E, 0x00000000)]
    public partial class TabooTable : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.TabooTable;

        public Dictionary<uint, TabooTableEntry> Entries = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            var _numEntriesBuckets = reader.ReadByte();
            var _numEntries = reader.ReadByte();
            for (var i=0; i < _numEntries; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadItem<TabooTableEntry>();
                Entries.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteByte(1);
            writer.WriteByte((byte)Entries.Count());
            foreach (var kv in Entries) {
                writer.WriteUInt32(kv.Key);
                writer.WriteItem<TabooTableEntry>(kv.Value);
            }
            return true;
        }

    }

}
