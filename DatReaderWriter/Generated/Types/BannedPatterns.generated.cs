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
    public partial class BannedPatterns : IDatObjType {
        public Dictionary<uint, string> Patterns = [];

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            var _numPatterns = reader.ReadUInt16();
            var _numPatternsBuckets = reader.ReadUInt16();
            for (var i=0; i < _numPatterns; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadString16L();
                Patterns.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt16((ushort)Patterns.Count());
            writer.WriteUInt16(32);
            foreach (var kv in Patterns) {
                writer.WriteUInt32(kv.Key);
                writer.WriteString16L(kv.Value);
            }
            return true;
        }

    }

}
