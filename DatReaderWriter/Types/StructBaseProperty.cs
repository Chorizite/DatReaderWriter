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
            if (reader.Database?.DatCollection is null) {
                throw new Exception("reader.Database.DatCollection is null! Unable to read MasterProperties and unpack StateDesc. Use DatCollection instead of creating a standalone DatDatabase");
            }
            if (!reader.Database.DatCollection.TryReadFile<MasterProperty>(0x39000001u, out var masterProperty)) {
                throw new Exception("Unable to read MasterProperty (0x39000001)");
            }

            base.Unpack(reader);
            var _bucketSize = reader.ReadByte();
            var _numValues = reader.ReadByte();
            for (var i = 0; i < _numValues; i++) {
                var _key = reader.ReadUInt32();
                var _peekedValue = reader.ReadUInt32();
                var _peekedPropType = masterProperty.Properties[_peekedValue].Type;
                reader.Skip(-sizeof(BasePropertyType) + 4);
                var _val = BaseProperty.Unpack(reader, _peekedPropType);
                Value.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteByte(0xD3);
            writer.WriteUInt32(0);
            writer.WriteByte((byte)Value.Count());
            foreach (var kv in Value) {
                writer.WriteUInt32(kv.Key);
                writer.WriteUInt32(kv.Key);
                kv.Value._includeType = false;
                writer.WriteItem<BaseProperty>(kv.Value);
            }
            return true;
        }

    }

}
