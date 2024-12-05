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
    public partial class TextureMapChange : IDatObjType {
        public byte PartIndex;

        public uint OldTexture;

        public uint NewTexture;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            PartIndex = reader.ReadByte();
            OldTexture = reader.ReadDataIdOfKnownType(0x05000000);
            NewTexture = reader.ReadDataIdOfKnownType(0x05000000);
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteByte(PartIndex);
            writer.WriteDataIdOfKnownType(OldTexture, 0x05000000);
            writer.WriteDataIdOfKnownType(NewTexture, 0x05000000);
            return true;
        }

    }

}
