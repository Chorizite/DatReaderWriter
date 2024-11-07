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
    /// DB_TYPE_SURFACE in the client.
    /// </summary>
    [DBObjType(typeof(Surface), DatFileType.Portal, DBObjType.Surface, DBObjHeaderFlags.None, 0x08000000, 0x0800FFFF, 0x00000000)]
    public partial class Surface : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.None;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.Surface;

        public SurfaceType Type;

        public uint OrigTextureId;

        public uint OrigPaletteId;

        public ColorARGB ColorValue;

        public float Translucency;

        public float Luminosity;

        public float Diffuse;

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            Type = (SurfaceType)reader.ReadUInt32();
            if (Type.HasFlag(SurfaceType.Base1Image) || Type.HasFlag(SurfaceType.Base1ClipMap)) {
                OrigTextureId = reader.ReadUInt32();
                OrigPaletteId = reader.ReadUInt32();
            }
            else {
                ColorValue = reader.ReadItem<ColorARGB>();
            }
            Translucency = reader.ReadSingle();
            Luminosity = reader.ReadSingle();
            Diffuse = reader.ReadSingle();
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32((uint)Type);
            if (Type.HasFlag(SurfaceType.Base1Image) || Type.HasFlag(SurfaceType.Base1ClipMap)) {
                writer.WriteUInt32(OrigTextureId);
                writer.WriteUInt32(OrigPaletteId);
            }
            else {
                writer.WriteItem<ColorARGB>(ColorValue);
            }
            writer.WriteSingle(Translucency);
            writer.WriteSingle(Luminosity);
            writer.WriteSingle(Diffuse);
            return true;
        }

    }

}
