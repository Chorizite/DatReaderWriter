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
    /// DB_TYPE_SURFACETEXTURE in the client.
    /// </summary>
    [DBObjType(typeof(SurfaceTexture), DatFileType.Portal, DBObjType.SurfaceTexture, DBObjHeaderFlags.HasId | DBObjHeaderFlags.HasDataCategory, 0x05000000, 0x05FFFFFF, 0x00000000)]
    public partial class SurfaceTexture : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId | DBObjHeaderFlags.HasDataCategory;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.SurfaceTexture;

        /// <summary>
        /// This seems to always be TextureType.Texture2D in the dats.
        /// </summary>
        public TextureType Type;

        public List<uint> Textures = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            Type = (TextureType)reader.ReadByte();
            var _numtextures = reader.ReadInt32();
            for (var i=0; i < _numtextures; i++) {
                Textures.Add(reader.ReadUInt32());
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteByte((byte)Type);
            writer.WriteInt32((int)Textures.Count());
            foreach (var item in Textures) {
                writer.WriteUInt32(item);
            }
            return true;
        }

    }

}
