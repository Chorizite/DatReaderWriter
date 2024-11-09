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
    public partial class CloObjectEffect : IDatObjType {
        public uint Index;

        public uint ModelId;

        public List<CloTextureEffect> CloTextureEffects = [];

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Index = reader.ReadUInt32();
            ModelId = reader.ReadUInt32();
            var _numCloTextureEffects = reader.ReadUInt32();
            for (var i=0; i < _numCloTextureEffects; i++) {
                CloTextureEffects.Add(reader.ReadItem<CloTextureEffect>());
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(Index);
            writer.WriteUInt32(ModelId);
            writer.WriteUInt32((uint)CloTextureEffects.Count());
            foreach (var item in CloTextureEffects) {
                writer.WriteItem<CloTextureEffect>(item);
            }
            return true;
        }

    }

}
