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
using ACClientLib.DatReaderWriter.Types;
namespace ACClientLib.DatReaderWriter.DBObjs {
    public class Environment : DBObj {
        /// <inheritdoc />
        public override bool HasDataCategory => false;

        /// <summary>
        /// The cells in this environment
        /// </summary>
        public Dictionary<uint, CellStruct> Cells = [];

        /// <inheritdoc />
        public override bool Unpack(DatFileReader reader) {
            base.Unpack(reader);
            var _cellCount = reader.ReadUInt32();
            for (var i=0; i < _cellCount; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadItem<CellStruct>();
                Cells.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatFileWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32((uint)Cells.Count());
            foreach (var kv in Cells) {
                writer.WriteUInt32(kv.Key);
                writer.WriteItem<CellStruct>(kv.Value);
            }
            return true;
        }

    }

}
