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
    /// DB_TYPE_DUAL_DID_MAPPER in the client.
    /// </summary>
    [DBObjType(typeof(DualDataIdMapper), DatFileType.Portal, DBObjType.DualDataIdMapper, DBObjHeaderFlags.HasId, 0x27000000, 0x27FFFFFF, 0x00000000)]
    public partial class DualDataIdMapper : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.DualDataIdMapper;

        public NumberingType ClientIDNumberingType;

        public Dictionary<uint, uint> ClientEnumToID = [];

        public NumberingType ClientNameNumberingType;

        public Dictionary<uint, string> ClientEnumToName = [];

        public NumberingType ServerIDNumberingType;

        public Dictionary<uint, uint> ServerEnumToID = [];

        public NumberingType ServerNameNumberingType;

        public Dictionary<uint, string> ServerEnumToName = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            ClientIDNumberingType = (NumberingType)reader.ReadByte();
            var _numClientEnumToIDs = reader.ReadCompressedUInt();
            for (var i=0; i < _numClientEnumToIDs; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadUInt32();
                ClientEnumToID.Add(_key, _val);
            }
            ClientNameNumberingType = (NumberingType)reader.ReadByte();
            var _numClientEnumToNames = reader.ReadCompressedUInt();
            for (var i=0; i < _numClientEnumToNames; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadString16LByte();
                ClientEnumToName.Add(_key, _val);
            }
            ServerIDNumberingType = (NumberingType)reader.ReadByte();
            var _numServerEnumToIDs = reader.ReadCompressedUInt();
            for (var i=0; i < _numServerEnumToIDs; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadUInt32();
                ServerEnumToID.Add(_key, _val);
            }
            ServerNameNumberingType = (NumberingType)reader.ReadByte();
            var _numServerEnumToNames = reader.ReadCompressedUInt();
            for (var i=0; i < _numServerEnumToNames; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadString16LByte();
                ServerEnumToName.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteByte((byte)ClientIDNumberingType);
            writer.WriteCompressedUInt((uint)ClientEnumToID.Count());
            foreach (var kv in ClientEnumToID) {
                writer.WriteUInt32(kv.Key);
                writer.WriteUInt32(kv.Value);
            }
            writer.WriteByte((byte)ClientNameNumberingType);
            writer.WriteCompressedUInt((uint)ClientEnumToName.Count());
            foreach (var kv in ClientEnumToName) {
                writer.WriteUInt32(kv.Key);
                writer.WriteString16LByte(kv.Value);
            }
            writer.WriteByte((byte)ServerIDNumberingType);
            writer.WriteCompressedUInt((uint)ServerEnumToID.Count());
            foreach (var kv in ServerEnumToID) {
                writer.WriteUInt32(kv.Key);
                writer.WriteUInt32(kv.Value);
            }
            writer.WriteByte((byte)ServerNameNumberingType);
            writer.WriteCompressedUInt((uint)ServerEnumToName.Count());
            foreach (var kv in ServerEnumToName) {
                writer.WriteUInt32(kv.Key);
                writer.WriteString16LByte(kv.Value);
            }
            return true;
        }

    }

}
