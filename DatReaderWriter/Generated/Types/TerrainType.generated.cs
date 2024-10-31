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
    public class TerrainType : IDatObjType {
        public string TerrainName;

        public ColorARGB TerrainColor;

        public List<uint> SceneTypes = [];

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            TerrainName = reader.ReadString16L();
            reader.Align(4);
            TerrainColor = reader.ReadItem<ColorARGB>();
            var _numSceneTypes = reader.ReadUInt32();
            for (var i=0; i < _numSceneTypes; i++) {
                SceneTypes.Add(reader.ReadUInt32());
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteString16L(TerrainName);
            writer.Align(4);
            writer.WriteItem<ColorARGB>(TerrainColor);
            writer.WriteUInt32((uint)SceneTypes.Count());
            foreach (var item in SceneTypes) {
                writer.WriteUInt32(item);
            }
            return true;
        }

    }

}
