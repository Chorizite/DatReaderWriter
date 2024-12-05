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
    public partial class EyeStripCG : IDatObjType {
        public uint IconId;

        public uint BaldIconId;

        public ObjDesc ObjDesc;

        public ObjDesc BaldObjDesc;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            IconId = reader.ReadUInt32();
            BaldIconId = reader.ReadUInt32();
            ObjDesc = reader.ReadItem<ObjDesc>();
            BaldObjDesc = reader.ReadItem<ObjDesc>();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(IconId);
            writer.WriteUInt32(BaldIconId);
            writer.WriteItem<ObjDesc>(ObjDesc);
            writer.WriteItem<ObjDesc>(BaldObjDesc);
            return true;
        }

    }

}
