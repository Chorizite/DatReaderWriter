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
    public class GfxObj : DBObj {
        /// <inheritdoc />
        public override bool HasDataCategory => false;

        public GfxObjFlags Flags;

        /// <summary>
        /// The list of surfaces
        /// </summary>
        public List<uint> Surfaces = [];

        public VertexArray VertexArray;

        public Dictionary<ushort, Polygon> PhysicsPolygons = [];

        public PhysicsBSPTree PhysicsBSP;

        public Vector3 SortCenter;

        public Dictionary<ushort, Polygon> Polygons = [];

        public DrawingBSPTree DrawingBSP;

        public uint DIDDegrade;

        /// <inheritdoc />
        public override bool Unpack(DatFileReader reader) {
            base.Unpack(reader);
            Flags = (GfxObjFlags)reader.ReadUInt32();
            var _numSurfaces = reader.ReadCompressedUInt();
            for (var i=0; i < _numSurfaces; i++) {
                Surfaces.Add(reader.ReadUInt32());
            }
            VertexArray = reader.ReadItem<VertexArray>();
            if (Flags.HasFlag(GfxObjFlags.HasPhysics)) {
                var _numPhysicsPolys = reader.ReadCompressedUInt();
                for (var i=0; i < _numPhysicsPolys; i++) {
                    var _key = reader.ReadUInt16();
                    var _val = reader.ReadItem<Polygon>();
                    PhysicsPolygons.Add(_key, _val);
                }
                PhysicsBSP = reader.ReadItem<PhysicsBSPTree>();
            }
            SortCenter = reader.ReadVector3();
            if (Flags.HasFlag(GfxObjFlags.HasDrawing)) {
                var _numPolys = reader.ReadCompressedUInt();
                for (var i=0; i < _numPolys; i++) {
                    var _key = reader.ReadUInt16();
                    var _val = reader.ReadItem<Polygon>();
                    Polygons.Add(_key, _val);
                }
                DrawingBSP = reader.ReadItem<DrawingBSPTree>();
            }
            if (Flags.HasFlag(GfxObjFlags.HasDIDDegrade)) {
                DIDDegrade = reader.ReadUInt32();
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatFileWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32((uint)Flags);
            writer.WriteCompressedUInt((uint)Surfaces.Count());
            foreach (var item in Surfaces) {
                writer.WriteUInt32(item);
            }
            writer.WriteItem<VertexArray>(VertexArray);
            if (Flags.HasFlag(GfxObjFlags.HasPhysics)) {
                writer.WriteCompressedUInt((uint)PhysicsPolygons.Count());
                foreach (var kv in PhysicsPolygons) {
                    writer.WriteUInt16(kv.Key);
                    writer.WriteItem<Polygon>(kv.Value);
                }
                writer.WriteItem<PhysicsBSPTree>(PhysicsBSP);
            }
            writer.WriteVector3(SortCenter);
            if (Flags.HasFlag(GfxObjFlags.HasDrawing)) {
                writer.WriteCompressedUInt((uint)Polygons.Count());
                foreach (var kv in Polygons) {
                    writer.WriteUInt16(kv.Key);
                    writer.WriteItem<Polygon>(kv.Value);
                }
                writer.WriteItem<DrawingBSPTree>(DrawingBSP);
            }
            if (Flags.HasFlag(GfxObjFlags.HasDIDDegrade)) {
                writer.WriteUInt32(DIDDegrade);
            }
            return true;
        }

    }

}
