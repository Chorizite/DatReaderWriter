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
        public bool Unpack(DatFileReader reader) {
            Id = reader.ReadUInt32();
            Frame = reader.ReadItem<Frame>();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteUInt32(Id);
            writer.WriteItem<Frame>(Frame);
            return true;
        }

    }

}
