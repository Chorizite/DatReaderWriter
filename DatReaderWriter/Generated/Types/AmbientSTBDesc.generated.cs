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
    public partial class AmbientSTBDesc : IDatObjType {
        public uint STBId;

        public List<AmbientSoundDesc> AmbientSounds = [];

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            STBId = reader.ReadUInt32();
            var _numAmbientSounds = reader.ReadUInt32();
            for (var i=0; i < _numAmbientSounds; i++) {
                AmbientSounds.Add(reader.ReadItem<AmbientSoundDesc>());
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(STBId);
            writer.WriteUInt32((uint)AmbientSounds.Count());
            foreach (var item in AmbientSounds) {
                writer.WriteItem<AmbientSoundDesc>(item);
            }
            return true;
        }

    }

}
