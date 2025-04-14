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
    /// DB_TYPE_RENDERTEXTURE in the client.
    /// </summary>
    [DBObjType(typeof(RenderTexture), DatFileType.Portal, DBObjType.RenderTexture, DBObjHeaderFlags.HasId | DBObjHeaderFlags.HasDataCategory, 0x15000000, 0x15FFFFFF, 0x00000000)]
    public partial class RenderTexture : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId | DBObjHeaderFlags.HasDataCategory;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.RenderTexture;

        public TextureType TextureType;

        public List<uint> SourceLevels = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            TextureType = (TextureType)reader.ReadByte();
            var _numSourceLevels = reader.ReadUInt32();
            for (var i=0; i < _numSourceLevels; i++) {
                SourceLevels.Add(reader.ReadUInt32());
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteByte((byte)TextureType);
            writer.WriteUInt32((uint)SourceLevels.Count());
            foreach (var item in SourceLevels) {
                writer.WriteUInt32(item);
            }
            return true;
        }

    }

}
