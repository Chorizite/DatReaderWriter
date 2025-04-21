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
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.Types {
    public partial class EnumMapperData : IDatObjType {
        public uint BaseEnumMap;

        public uint Unknown;

        public Dictionary<uint, string> IdToStringMap = [];

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            BaseEnumMap = reader.ReadUInt32();
            Unknown = reader.ReadUInt32();
            var _bucketSize = reader.ReadByte();
            var _numIdToStringMaps = reader.ReadCompressedUInt();
            for (var i=0; i < _numIdToStringMaps; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadString16LByte();
                IdToStringMap.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(BaseEnumMap);
            writer.WriteUInt32(Unknown);
            writer.WriteByte(5);
            writer.WriteCompressedUInt((uint)IdToStringMap.Count());
            foreach (var kv in IdToStringMap) {
                writer.WriteUInt32(kv.Key);
                writer.WriteString16LByte(kv.Value);
            }
            return true;
        }

    }

}
