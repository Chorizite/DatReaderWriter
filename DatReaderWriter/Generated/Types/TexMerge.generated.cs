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
    public class TexMerge : IDatObjType {
        public uint BaseTexSize;

        public List<TerrainAlphaMap> CornerTerrainMaps = [];

        public List<TerrainAlphaMap> SideTerrainMaps = [];

        public List<RoadAlphaMap> RoadMaps = [];

        public List<TMTerrainDesc> TerrainDesc = [];

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            BaseTexSize = reader.ReadUInt32();
            var _numCornerTerrainMaps = reader.ReadUInt32();
            for (var i=0; i < _numCornerTerrainMaps; i++) {
                CornerTerrainMaps.Add(reader.ReadItem<TerrainAlphaMap>());
            }
            var _numSideTerrainMaps = reader.ReadUInt32();
            for (var i=0; i < _numSideTerrainMaps; i++) {
                SideTerrainMaps.Add(reader.ReadItem<TerrainAlphaMap>());
            }
            var _numRoadMaps = reader.ReadUInt32();
            for (var i=0; i < _numRoadMaps; i++) {
                RoadMaps.Add(reader.ReadItem<RoadAlphaMap>());
            }
            var _numTerrainDescs = reader.ReadUInt32();
            for (var i=0; i < _numTerrainDescs; i++) {
                TerrainDesc.Add(reader.ReadItem<TMTerrainDesc>());
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteUInt32(BaseTexSize);
            writer.WriteUInt32((uint)CornerTerrainMaps.Count());
            foreach (var item in CornerTerrainMaps) {
                writer.WriteItem<TerrainAlphaMap>(item);
            }
            writer.WriteUInt32((uint)SideTerrainMaps.Count());
            foreach (var item in SideTerrainMaps) {
                writer.WriteItem<TerrainAlphaMap>(item);
            }
            writer.WriteUInt32((uint)RoadMaps.Count());
            foreach (var item in RoadMaps) {
                writer.WriteItem<RoadAlphaMap>(item);
            }
            writer.WriteUInt32((uint)TerrainDesc.Count());
            foreach (var item in TerrainDesc) {
                writer.WriteItem<TMTerrainDesc>(item);
            }
            return true;
        }

    }

}
