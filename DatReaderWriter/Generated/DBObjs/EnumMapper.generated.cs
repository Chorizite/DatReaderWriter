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
    /// DB_TYPE_ENUM_MAPPER in the client.
    /// </summary>
    [DBObjType(typeof(EnumMapper), DatFileType.Portal, DBObjType.EnumMapper, DBObjHeaderFlags.HasId, 0x22000000, 0x22FFFFFF, 0x00000000)]
    public partial class EnumMapper : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.EnumMapper;

        public uint BaseEnumMap;

        public NumberingType NumberingType;

        public Dictionary<uint, string> IdToStringMap = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            BaseEnumMap = reader.ReadUInt32();
            NumberingType = (NumberingType)reader.ReadByte();
            var _numIdToStringMaps = reader.ReadCompressedUInt();
            for (var i=0; i < _numIdToStringMaps; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadString16LByte();
                IdToStringMap.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32(BaseEnumMap);
            writer.WriteByte((byte)NumberingType);
            writer.WriteCompressedUInt((uint)IdToStringMap.Count());
            foreach (var kv in IdToStringMap) {
                writer.WriteUInt32(kv.Key);
                writer.WriteString16LByte(kv.Value);
            }
            return true;
        }

    }

}
