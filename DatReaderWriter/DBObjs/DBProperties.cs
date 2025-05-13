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
    /// DB_TYPE_DBPROPERTIES in the client.
    /// </summary>
    [DBObjType(typeof(DBProperties), DatFileType.Portal, DBObjType.DBProperties, DBObjHeaderFlags.HasId, 0x78000000, 0x7FFFFFFF, 0x00000000)]
    public partial class DBProperties : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.DBProperties;

        public Dictionary<uint, BaseProperty> Properties = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            var _bucketSize = reader.ReadByte();
            var _numProperties = reader.ReadByte();

            for (var i = 0; i < _numProperties; i++) {
                var _key = reader.ReadUInt32();
                var _val = BaseProperty.UnpackGeneric(reader);
                Properties.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteByte(0);
            writer.WriteByte((byte)Properties.Count());
            foreach (var kv in Properties) {
                writer.WriteUInt32(kv.Key);
                writer.WriteItem<BaseProperty>(kv.Value);
            }
            return true;
        }

    }

}
