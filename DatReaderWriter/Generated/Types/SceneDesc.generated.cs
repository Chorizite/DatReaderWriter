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
    public partial class SceneDesc : IDatObjType {
        public List<SceneType> SceneTypes = [];

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            var _numSceneTypes = reader.ReadUInt32();
            for (var i=0; i < _numSceneTypes; i++) {
                SceneTypes.Add(reader.ReadItem<SceneType>());
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32((uint)SceneTypes.Count());
            foreach (var item in SceneTypes) {
                writer.WriteItem<SceneType>(item);
            }
            return true;
        }

    }

}
