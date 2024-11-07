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
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.Types {
    public partial class TerrainDesc : IDatObjType {
        public List<TerrainType> TerrainTypes = [];

        public LandSurf LandSurfaces;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            var _numTerrainTypes = reader.ReadUInt32();
            for (var i=0; i < _numTerrainTypes; i++) {
                TerrainTypes.Add(reader.ReadItem<TerrainType>());
            }
            LandSurfaces = reader.ReadItem<LandSurf>();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32((uint)TerrainTypes.Count());
            foreach (var item in TerrainTypes) {
                writer.WriteItem<TerrainType>(item);
            }
            writer.WriteItem<LandSurf>(LandSurfaces);
            return true;
        }

    }

}
