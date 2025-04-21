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
    /// DB_TYPE_MASTER_PROPERTY in the client.
    /// </summary>
    [DBObjType(typeof(MasterProperty), DatFileType.Portal, DBObjType.MasterProperty, DBObjHeaderFlags.HasId, 0x39000000, 0x39FFFFFF, 0x00000000)]
    public partial class MasterProperty : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.MasterProperty;

        public EnumMapperData EnumMapper;

        public Dictionary<uint, BasePropertyDesc> Properties = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            EnumMapper = reader.ReadItem<EnumMapperData>();

            var _bucketSize = reader.ReadByte();
            var _numProperties = reader.ReadCompressedUInt();
            for (var i = 0; i < _numProperties; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadItem<BasePropertyDesc>();
                Properties.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteItem<EnumMapperData>(EnumMapper);

            writer.WriteByte(6); // bucket size
            writer.WriteCompressedUInt((uint)Properties.Count());
            foreach (var kv in Properties) {
                writer.WriteUInt32(kv.Key);
                writer.WriteItem<BasePropertyDesc>(kv.Value);
            }
            return true;
        }

    }

}
