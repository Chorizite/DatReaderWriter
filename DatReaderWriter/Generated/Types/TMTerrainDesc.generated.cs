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
    public partial class TMTerrainDesc : IDatObjType {
        public TerrainTextureType TerrainType;

        public TerrainTex TerrainTex;

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            TerrainType = (TerrainTextureType)reader.ReadInt32();
            TerrainTex = reader.ReadItem<TerrainTex>();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteInt32((int)TerrainType);
            writer.WriteItem<TerrainTex>(TerrainTex);
            return true;
        }

    }

}
