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
    /// DB_TYPE_SURFACETEXTURE in the client.
    /// </summary>
    [DBObjType(DatFileType.Portal, true, 0x05000000, 0x05FFFFFF)]
    public class SurfaceTexture : DBObj {
        /// <inheritdoc />
        public override bool HasDataCategory => true;

        /// <summary>
        /// This seems to always be TextureType.Texture2D in the dats.
        /// </summary>
        public TextureType Type;

        public List<uint> Textures = [];

        /// <inheritdoc />
        public override bool Unpack(DatFileReader reader) {
            base.Unpack(reader);
            Type = (TextureType)reader.ReadByte();
            var _numtextures = reader.ReadInt32();
            for (var i=0; i < _numtextures; i++) {
                Textures.Add(reader.ReadUInt32());
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatFileWriter writer) {
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
