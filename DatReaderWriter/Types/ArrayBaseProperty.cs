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
            base.Unpack(reader);
            var _numValues = reader.ReadUInt32();

            for (var i = 0; i < _numValues; i++) {
                Value.Add(BaseProperty.UnpackGeneric(reader)!);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32((uint)Value.Count());

            foreach (var item in Value) {
                item.Pack(writer);
            }
            return true;
        }

    }

}
