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
    /// This holds raw texture data. DB_TYPE_RENDERSURFACE in the client.
    /// </summary>
    [DBObjType(typeof(RenderSurface), DatFileType.Portal, DBObjType.RenderSurface, DBObjHeaderFlags.HasId | DBObjHeaderFlags.HasDataCategory, 0x06000000, 0x07FFFFFF, 0x00000000)]
    public partial class RenderSurface : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId | DBObjHeaderFlags.HasDataCategory;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.RenderSurface;

        /// <summary>
        /// The width of the SurfaceTexture
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the SurfaceTexture
        /// </summary>
        public int Height;

        /// <summary>
        /// The format of the SourceData
        /// </summary>
        public PixelFormat Format;

        /// <summary>
        /// The source data of the texture
        /// </summary>
        public byte[] SourceData = [];

        /// <summary>
        /// This is only set when Format is PixelFormat.PFID_INDEX16 | PixelFormat.PFID_P8.
        /// </summary>
        public uint DefaultPaletteId;

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            Format = (PixelFormat)reader.ReadUInt32();
            var _sourceDataLength = reader.ReadInt32();
            SourceData = new byte[_sourceDataLength];
            for (var i=0; i < _sourceDataLength; i++) {
                SourceData[i] = reader.ReadByte();
            }
            switch(Format) {
                case PixelFormat.PFID_INDEX16:
                case PixelFormat.PFID_P8:
                    DefaultPaletteId = reader.ReadUInt32();
                    break;
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteInt32(Width);
            writer.WriteInt32(Height);
            writer.WriteUInt32((uint)Format);
            writer.WriteInt32((int)SourceData.Count());
            for (var i=0; i < SourceData.Count(); i++) {
                writer.WriteByte(SourceData[i]);
            }
            switch(Format) {
                case PixelFormat.PFID_INDEX16:
                case PixelFormat.PFID_P8:
                    writer.WriteUInt32(DefaultPaletteId);
                    break;
            }
            return true;
        }

    }

}
