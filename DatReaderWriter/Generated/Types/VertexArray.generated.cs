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
    public partial class VertexArray : IDatObjType {
        public VertexType VertexType;

        public Dictionary<ushort, SWVertex> Vertices = [];

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            VertexType = (VertexType)reader.ReadInt32();
            var _numVertices = reader.ReadUInt32();
            for (var i=0; i < _numVertices; i++) {
                var _key = reader.ReadUInt16();
                var _val = reader.ReadItem<SWVertex>();
                Vertices.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteInt32((int)VertexType);
            writer.WriteUInt32((uint)Vertices.Count());
            foreach (var kv in Vertices) {
                writer.WriteUInt16(kv.Key);
                writer.WriteItem<SWVertex>(kv.Value);
            }
            return true;
        }

    }

}
