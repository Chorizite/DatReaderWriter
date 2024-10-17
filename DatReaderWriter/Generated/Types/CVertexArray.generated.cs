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
    public class CVertexArray : IDatObjType {
        public VertexType VertexType;

        public Dictionary<ushort, SWVertex> VertexArray = new Dictionary<ushort, SWVertex>();

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            VertexType = (VertexType)reader.ReadInt32();
            var _numVertices = reader.ReadUInt32();
            for (var i=0; i < _numVertices; i++) {
                VertexArray.Add(reader.ReadUInt16(), reader.ReadItem<SWVertex>());
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteInt32((int)VertexType);
            writer.WriteUInt32((uint)VertexArray.Count());
            foreach (var kv in VertexArray) {
                writer.WriteUInt16(kv.Key);
                writer.WriteItem<SWVertex>(kv.Value);
            }
            return true;
        }

    }

}
