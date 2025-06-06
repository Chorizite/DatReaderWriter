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
    public partial class Bitfield64BaseProperty : BaseProperty {
        /// <inheritdoc />
        public override BasePropertyType PropertyType => BasePropertyType.Bitfield64;

        public ulong Value;

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            Value = reader.ReadUInt64();
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt64(Value);
            return true;
        }

    }

}
