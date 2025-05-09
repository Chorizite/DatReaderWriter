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
    public partial class ArrayBaseProperty : BaseProperty {
        /// <inheritdoc />
        public override BasePropertyType PropertyType => BasePropertyType.Array;

        public List<BaseProperty> Value = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            if (reader.Database?.DatCollection is null) {
                throw new Exception("reader.Database.DatCollection is null! Unable to read MasterProperties and unpack StateDesc. Use DatCollection instead of creating a standalone DatDatabase");
            }
            if (!reader.Database.DatCollection.TryReadFile<MasterProperty>(0x39000001u, out var masterProperty)) {
                throw new Exception("Unable to read MasterProperty (0x39000001)");
            }

            base.Unpack(reader);
            var _numValues = reader.ReadUInt32();

            for (var i = 0; i < _numValues; i++) {
                var _peekedValue = reader.ReadUInt32();
                var _peekedPropType = masterProperty.Properties[_peekedValue].Type;
                reader.Skip(-sizeof(BasePropertyType) + 4);
                Value.Add(BaseProperty.Unpack(reader, _peekedPropType));
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32((uint)Value.Count());

            foreach (var item in Value) {
                writer.WriteUInt32((uint)item.PropertyType);
                item._includeType = _includeType;
                item.Pack(writer);
            }
            return true;
        }

    }

}
