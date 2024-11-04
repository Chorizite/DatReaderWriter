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
using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.IO;

namespace ACClientLib.DatReaderWriter.Types {
    public partial class TimeOfDay : IDatObjType {
        public float Start;

        public bool IsNight;

        public string Name;

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            Start = reader.ReadSingle();
            IsNight = reader.ReadBool(4);
            Name = reader.ReadString16L();
            reader.Align(4);
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteSingle(Start);
            writer.WriteBool(IsNight, 4);
            writer.WriteString16L(Name);
            writer.Align(4);
            return true;
        }

    }

}
