using System;
using System.Numerics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DatReaderWriter.Enums;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.DBObjs;

namespace DatReaderWriter.Types {
    public partial class StructBaseProperty : BaseProperty {
        /// <inheritdoc />
        public override BasePropertyType PropertyType => BasePropertyType.Struct;

        public Dictionary<uint, BaseProperty> Value = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            var _bucketSize = reader.ReadByte();
            var _numValues = reader.ReadByte();
            for (var i = 0; i < _numValues; i++) {
                var _key = reader.ReadUInt32();
                var _val = BaseProperty.UnpackGeneric(reader);
                Value.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteByte(0);
            writer.WriteByte((byte)Value.Count());
            foreach (var kv in Value) {
                writer.WriteUInt32(kv.Key);
                writer.WriteItem<BaseProperty>(kv.Value);
            }
            return true;
        }

    }

}
