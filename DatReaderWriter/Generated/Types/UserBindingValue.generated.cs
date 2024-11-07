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
    public partial class UserBindingValue : IDatObjType {
        public uint ActionClass;

        /// <summary>
        /// These correspond to 0x2200002B enum dat entry
        /// </summary>
        public uint ActionName;

        public ControlSpecification ControlSpec;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            ActionClass = reader.ReadUInt32();
            ActionName = reader.ReadUInt32();
            ControlSpec = reader.ReadItem<ControlSpecification>();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(ActionClass);
            writer.WriteUInt32(ActionName);
            writer.WriteItem<ControlSpecification>(ControlSpec);
            return true;
        }

    }

}
