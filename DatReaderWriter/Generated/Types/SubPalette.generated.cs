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
    public partial class SubPalette : IDatObjType {
        public uint SubId;

        public byte Offset;

        public byte NumColors;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            SubId = reader.ReadDataIdOfKnownType(0x04000000);
            Offset = reader.ReadByte();
            NumColors = reader.ReadByte();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteDataIdOfKnownType(SubId, 0x04000000);
            writer.WriteByte(Offset);
            writer.WriteByte(NumColors);
            return true;
        }

    }

}
