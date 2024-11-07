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
    public partial class CellPortal : IDatObjType {
        public PortalFlags Flags;

        public ushort PolygonId;

        public ushort OtherCellId;

        public ushort OtherPortalId;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Flags = (PortalFlags)reader.ReadUInt16();
            PolygonId = reader.ReadUInt16();
            OtherCellId = reader.ReadUInt16();
            OtherPortalId = reader.ReadUInt16();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt16((ushort)Flags);
            writer.WriteUInt16(PolygonId);
            writer.WriteUInt16(OtherCellId);
            writer.WriteUInt16(OtherPortalId);
            return true;
        }

    }

}
