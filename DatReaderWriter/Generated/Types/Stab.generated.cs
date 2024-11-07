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
    /// <summary>
    /// Information about a static object spawn.
    /// </summary>
    public partial class Stab : IDatObjType {
        /// <summary>
        /// The id of this static object
        /// </summary>
        public uint Id;

        /// <summary>
        /// The position information
        /// </summary>
        public Frame Frame;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Id = reader.ReadUInt32();
            Frame = reader.ReadItem<Frame>();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(Id);
            writer.WriteItem<Frame>(Frame);
            return true;
        }

    }

}
