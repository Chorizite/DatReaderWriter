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
    public partial class CellStruct : IDatObjType {
        public VertexArray VertexArray;

        public Dictionary<ushort, Polygon> Polygons = [];

        public List<ushort> Portals = [];

        public CellBSPTree CellBSP;

        public Dictionary<ushort, Polygon> PhysicsPolygons = [];

        public PhysicsBSPTree PhysicsBSP;

        public DrawingBSPTree DrawingBSP;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            var _numPolys = reader.ReadUInt32();
            var _numPhysicsPolys = reader.ReadUInt32();
            var _numPortals = reader.ReadUInt32();
            VertexArray = reader.ReadItem<VertexArray>();
            for (var i=0; i < _numPolys; i++) {
                var _key = reader.ReadUInt16();
                var _val = reader.ReadItem<Polygon>();
                Polygons.Add(_key, _val);
            }
            for (var i=0; i < _numPortals; i++) {
                Portals.Add(reader.ReadUInt16());
            }
            reader.Align(4);
            CellBSP = reader.ReadItem<CellBSPTree>();
            for (var i=0; i < _numPhysicsPolys; i++) {
                var _key = reader.ReadUInt16();
                var _val = reader.ReadItem<Polygon>();
                PhysicsPolygons.Add(_key, _val);
            }
            PhysicsBSP = reader.ReadItem<PhysicsBSPTree>();
            var _hasDrawingBSP = reader.ReadBool();
            if (_hasDrawingBSP) {
                DrawingBSP = reader.ReadItem<DrawingBSPTree>();
            }
            reader.Align(4);
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32((uint)Polygons.Count());
            writer.WriteUInt32((uint)PhysicsPolygons.Count());
            writer.WriteUInt32((uint)Portals.Count());
            writer.WriteItem<VertexArray>(VertexArray);
            foreach (var kv in Polygons) {
                writer.WriteUInt16(kv.Key);
                writer.WriteItem<Polygon>(kv.Value);
            }
            foreach (var item in Portals) {
                writer.WriteUInt16(item);
            }
            writer.Align(4);
            writer.WriteItem<CellBSPTree>(CellBSP);
            foreach (var kv in PhysicsPolygons) {
                writer.WriteUInt16(kv.Key);
                writer.WriteItem<Polygon>(kv.Value);
            }
            writer.WriteItem<PhysicsBSPTree>(PhysicsBSP);
            writer.WriteBool(DrawingBSP != null);
            if (DrawingBSP != null) {
                writer.WriteItem<DrawingBSPTree>(DrawingBSP);
            }
            writer.Align(4);
            return true;
        }

    }

}
