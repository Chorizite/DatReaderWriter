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
    public partial class HairStyleCG : IDatObjType {
        public uint IconId;

        public bool Bald;

        public uint AlternateSetup;

        public ObjDesc ObjDesc;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            IconId = reader.ReadUInt32();
            Bald = reader.ReadBool(1);
            AlternateSetup = reader.ReadUInt32();
            ObjDesc = reader.ReadItem<ObjDesc>();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(IconId);
            writer.WriteBool(Bald, 1);
            writer.WriteUInt32(AlternateSetup);
            writer.WriteItem<ObjDesc>(ObjDesc);
            return true;
        }

    }

}
