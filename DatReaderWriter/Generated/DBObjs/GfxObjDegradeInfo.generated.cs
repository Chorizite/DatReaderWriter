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
using DatReaderWriter.Types;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.DBObjs {
    /// <summary>
    /// DB_TYPE_DEGRADEINFO in the client.
    /// </summary>
    [DBObjType(typeof(GfxObjDegradeInfo), DatFileType.Portal, DBObjType.GfxObjDegradeInfo, DBObjHeaderFlags.HasId, 0x11000000, 0x1100FFFF, 0x00000000)]
    public partial class GfxObjDegradeInfo : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.GfxObjDegradeInfo;

        public List<GfxObjInfo> Degrades = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            var _numDegrades = reader.ReadUInt32();
            for (var i=0; i < _numDegrades; i++) {
                Degrades.Add(reader.ReadItem<GfxObjInfo>());
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32((uint)Degrades.Count());
            foreach (var item in Degrades) {
                writer.WriteItem<GfxObjInfo>(item);
            }
            return true;
        }

    }

}
