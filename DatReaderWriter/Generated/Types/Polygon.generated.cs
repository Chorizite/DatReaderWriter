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
    public class Polygon : IDatObjType {
        public StipplingType Stippling;

        public CullMode SidesType;

        public short PosSurface;

        public short NegSurface;

        public List<short> VertexIds = [];

        public List<byte> PosUVIndices = [];

        public List<byte> NegUVIndices = [];

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            var _numVertices = reader.ReadByte();
            Stippling = (StipplingType)reader.ReadByte();
            SidesType = (CullMode)reader.ReadInt32();
            PosSurface = reader.ReadInt16();
            NegSurface = reader.ReadInt16();
            for (var i=0; i < _numVertices; i++) {
                VertexIds.Add(reader.ReadInt16());
            }
            if (!Stippling.HasFlag(StipplingType.NoPos)) {
                for (var i=0; i < _numVertices; i++) {
                    PosUVIndices.Add(reader.ReadByte());
                }
            }
            if (!Stippling.HasFlag(StipplingType.NoNeg)) {
                if (SidesType == CullMode.Clockwise) {
                    for (var i=0; i < _numVertices; i++) {
                        NegUVIndices.Add(reader.ReadByte());
                    }
                }
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteByte((byte)VertexIds.Count());
            writer.WriteByte((byte)Stippling);
            writer.WriteInt32((int)SidesType);
            writer.WriteInt16(PosSurface);
            writer.WriteInt16(NegSurface);
            foreach (var item in VertexIds) {
                writer.WriteInt16(item);
            }
            if (!Stippling.HasFlag(StipplingType.NoPos)) {
                foreach (var item in PosUVIndices) {
                    writer.WriteByte(item);
                }
            }
            if (!Stippling.HasFlag(StipplingType.NoNeg)) {
                if (SidesType == CullMode.Clockwise) {
                    foreach (var item in NegUVIndices) {
                        writer.WriteByte(item);
                    }
                }
            }
            return true;
        }

    }

}
