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
    /// DB_TYPE_BADDATA in the client.
    /// </summary>
    [DBObjType(typeof(BadDataTable), DatFileType.Portal, DBObjType.BadDataTable, DBObjHeaderFlags.HasId, 0x0E00001A, 0x0E00001A, 0x00000000)]
    public partial class BadDataTable : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.BadDataTable;

        public Dictionary<uint, uint> BadIds = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            var _numBadIds = reader.ReadUInt16();
            var _numBadIdBuckets = reader.ReadUInt16();
            for (var i=0; i < _numBadIds; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadUInt32();
                BadIds.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt16((ushort)BadIds.Count());
            writer.WriteUInt16(32);
            foreach (var kv in BadIds) {
                writer.WriteUInt32(kv.Key);
                writer.WriteUInt32(kv.Value);
            }
            return true;
        }

    }

}
