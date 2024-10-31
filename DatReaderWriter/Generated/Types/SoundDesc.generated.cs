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
    public class SoundDesc : IDatObjType {
        public List<AmbientSTBDesc> STBDesc = [];

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            var _numSTBDescs = reader.ReadUInt32();
            for (var i=0; i < _numSTBDescs; i++) {
                STBDesc.Add(reader.ReadItem<AmbientSTBDesc>());
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteUInt32((uint)STBDesc.Count());
            foreach (var item in STBDesc) {
                writer.WriteItem<AmbientSTBDesc>(item);
            }
            return true;
        }

    }

}
