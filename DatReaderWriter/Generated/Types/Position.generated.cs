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
    /// Information about an ingame position containing a cell id and a frame
    /// </summary>
    public partial class Position : IDatObjType {
        /// <summary>
        /// The full id of the landcell
        /// </summary>
        public uint CellId;

        public Frame Frame;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            CellId = reader.ReadUInt32();
            Frame = reader.ReadItem<Frame>();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(CellId);
            writer.WriteItem<Frame>(Frame);
            return true;
        }

    }

}
