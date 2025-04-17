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
    /// DB_TYPE_FONT in the client.
    /// </summary>
    [DBObjType(typeof(Font), DatFileType.Portal, DBObjType.Font, DBObjHeaderFlags.HasId, 0x40000000, 0x40000FFF, 0x00000000)]
    public partial class Font : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.Font;

        public uint MaxCharHeight;

        public uint MaxCharWidth;

        public List<FontCharDesc> CharDescs = [];

        public uint NumHorizontalBorderPixels;

        public uint NumVerticalBorderPixels;

        public uint BaselineOffset;

        public uint ForegroundSurfaceDataId;

        public uint BackgroundSurfaceDataId;

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            MaxCharHeight = reader.ReadUInt32();
            MaxCharWidth = reader.ReadUInt32();
            var _numCharDescs = reader.ReadUInt32();
            for (var i=0; i < _numCharDescs; i++) {
                CharDescs.Add(reader.ReadItem<FontCharDesc>());
            }
            NumHorizontalBorderPixels = reader.ReadUInt32();
            NumVerticalBorderPixels = reader.ReadUInt32();
            BaselineOffset = reader.ReadUInt32();
            ForegroundSurfaceDataId = reader.ReadUInt32();
            BackgroundSurfaceDataId = reader.ReadUInt32();
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32(MaxCharHeight);
            writer.WriteUInt32(MaxCharWidth);
            writer.WriteUInt32((uint)CharDescs.Count());
            foreach (var item in CharDescs) {
                writer.WriteItem<FontCharDesc>(item);
            }
            writer.WriteUInt32(NumHorizontalBorderPixels);
            writer.WriteUInt32(NumVerticalBorderPixels);
            writer.WriteUInt32(BaselineOffset);
            writer.WriteUInt32(ForegroundSurfaceDataId);
            writer.WriteUInt32(BackgroundSurfaceDataId);
            return true;
        }

    }

}
