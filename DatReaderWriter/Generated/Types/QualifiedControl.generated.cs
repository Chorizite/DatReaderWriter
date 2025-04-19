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
    public partial class QualifiedControl : IDatObjType {
        public ControlSpecification Key;

        public uint Activation;

        public uint Unknown;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Key = reader.ReadItem<ControlSpecification>();
            Activation = reader.ReadUInt32();
            Unknown = reader.ReadUInt32();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteItem<ControlSpecification>(Key);
            writer.WriteUInt32(Activation);
            writer.WriteUInt32(Unknown);
            return true;
        }

    }

}
