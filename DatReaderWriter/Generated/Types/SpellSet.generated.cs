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
    public partial class SpellSet : IDatObjType {
        public Dictionary<uint, SpellSetTiers> SpellSetTiers = [];

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            var _numSpellSetTiers = reader.ReadUInt16();
            var _numSpellSetTiersBuckets = reader.ReadUInt16();
            for (var i=0; i < _numSpellSetTiers; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadItem<SpellSetTiers>();
                SpellSetTiers.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteUInt16((ushort)SpellSetTiers.Count());
            writer.WriteUInt16(0);
            foreach (var kv in SpellSetTiers) {
                writer.WriteUInt32(kv.Key);
                writer.WriteItem<SpellSetTiers>(kv.Value);
            }
            return true;
        }

    }

}
