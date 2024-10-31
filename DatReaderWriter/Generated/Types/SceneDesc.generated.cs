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
    public class SceneDesc : IDatObjType {
        public List<SceneType> SceneTypes = [];

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            var _numSceneTypes = reader.ReadUInt32();
            for (var i=0; i < _numSceneTypes; i++) {
                SceneTypes.Add(reader.ReadItem<SceneType>());
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteUInt32((uint)SceneTypes.Count());
            foreach (var item in SceneTypes) {
                writer.WriteItem<SceneType>(item);
            }
            return true;
        }

    }

}
