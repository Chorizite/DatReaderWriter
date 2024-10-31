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
    public class TerrainDesc : IDatObjType {
        public List<TerrainType> TerrainTypes = [];

        public LandSurf LandSurfaces;

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            var _numTerrainTypes = reader.ReadUInt32();
            for (var i=0; i < _numTerrainTypes; i++) {
                TerrainTypes.Add(reader.ReadItem<TerrainType>());
            }
            LandSurfaces = reader.ReadItem<LandSurf>();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteUInt32((uint)TerrainTypes.Count());
            foreach (var item in TerrainTypes) {
                writer.WriteItem<TerrainType>(item);
            }
            writer.WriteItem<LandSurf>(LandSurfaces);
            return true;
        }

    }

}
