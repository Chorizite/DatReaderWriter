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
using ACClientLib.DatReaderWriter.Types;
using ACClientLib.DatReaderWriter.Attributes;

namespace ACClientLib.DatReaderWriter.DBObjs {
    /// <summary>
    /// DB_TYPE_PALETTE in the client.
    /// </summary>
    [DBObjType(typeof(Palette), DatFileType.Portal, DBObjType.Palette, DBObjHeaderFlags.HasId, 0x04000000, 0x0400FFFF, 0x00000000)]
    public partial class Palette : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.Palette;

        public List<ColorARGB> Colors = [];

        /// <inheritdoc />
        public override bool Unpack(DatFileReader reader) {
            base.Unpack(reader);
            var _numColors = reader.ReadInt32();
            for (var i=0; i < _numColors; i++) {
                Colors.Add(reader.ReadItem<ColorARGB>());
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatFileWriter writer) {
            base.Pack(writer);
            writer.WriteInt32((int)Colors.Count());
            foreach (var item in Colors) {
                writer.WriteItem<ColorARGB>(item);
            }
            return true;
        }

    }

}
