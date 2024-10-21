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
    public class MotionCommandData : IDatObjType {
        public Dictionary<int, MotionData> MotionData = [];

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            var _numCommands = reader.ReadUInt32();
            for (var i=0; i < _numCommands; i++) {
                var _key = reader.ReadInt32();
                var _val = reader.ReadItem<MotionData>();
                MotionData.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteUInt32((uint)MotionData.Count());
            foreach (var kv in MotionData) {
                writer.WriteInt32(kv.Key);
                writer.WriteItem<MotionData>(kv.Value);
            }
            return true;
        }

    }

}
