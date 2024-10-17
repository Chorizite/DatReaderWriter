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
    public class PortalPoly : IDatObjType {
        public short PortalIndex;

        public short PolygonId;

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            PortalIndex = reader.ReadInt16();
            PolygonId = reader.ReadInt16();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteInt16(PortalIndex);
            writer.WriteInt16(PolygonId);
            return true;
        }

    }

}
