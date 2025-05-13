using System;
using DatReaderWriter.Enums;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests.Types {
    [TestClass]
    public class CellStructTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new CellStruct() {
                VertexArray = new VertexArray() {
                    VertexType = VertexType.CSWVertexType,
                    Vertices = new() {
                                { 12, new SWVertex() {
                                    Normal = Vector3.UnitX,
                                    Origin = Vector3.UnitY,
                                    UVs = [
                                        new Vec2Duv() {
                                            U = 3,
                                            V = 2
                                        },
                                        new Vec2Duv() {
                                            U = 2,
                                            V = 1
                                        }
                                    ]
                                }},
                            }
                },
                CellBSP = new CellBSPTree() {
                    Root = new CellBSPNode() { Type = BSPNodeType.Leaf }
                },
                DrawingBSP = new DrawingBSPTree() {
                    Root = new DrawingBSPNode() { Type = BSPNodeType.Leaf }
                },
                PhysicsBSP = new PhysicsBSPTree() {
                    Root = new PhysicsBSPNode() { Type = BSPNodeType.Leaf }
                },
                PhysicsPolygons = new Dictionary<ushort, Polygon>() {
                            { 1, new Polygon() {
                                Stippling = StipplingType.Both,
                                NegSurface = 1,
                                PosSurface = 1,
                                NegUVIndices = [1],
                                PosUVIndices = [2],
                                SidesType = CullMode.Clockwise,
                                VertexIds = [3],
                            } }
                        },
                Polygons = new Dictionary<ushort, Polygon>() {
                            { 1, new Polygon() {
                                Stippling = StipplingType.Both,
                                NegSurface = 1,
                                PosSurface = 1,
                                NegUVIndices = [1],
                                PosUVIndices = [6],
                                SidesType = CullMode.Clockwise,
                                VertexIds = [3],
                            } }
                        },
                Portals = [1, 2, 3]
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new CellStruct();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.IsNotNull(readObj.VertexArray);
            Assert.AreEqual(1, readObj.VertexArray.Vertices.Count);
            Assert.AreEqual(12, readObj.VertexArray.Vertices.First().Key);
            Assert.AreEqual(Vector3.UnitX, readObj.VertexArray.Vertices.First().Value.Normal);
            Assert.AreEqual(Vector3.UnitY, readObj.VertexArray.Vertices.First().Value.Origin);

            Assert.AreEqual(2, readObj.VertexArray.Vertices.First().Value.UVs.Count);
            Assert.AreEqual(3, readObj.VertexArray.Vertices.First().Value.UVs.First().U);
            Assert.AreEqual(2, readObj.VertexArray.Vertices.First().Value.UVs.First().V);
            Assert.AreEqual(2, readObj.VertexArray.Vertices.First().Value.UVs.Skip(1).First().U);
            Assert.AreEqual(1, readObj.VertexArray.Vertices.First().Value.UVs.Skip(1).First().V);

            Assert.IsNotNull(readObj.CellBSP);
            Assert.AreEqual(BSPNodeType.Leaf, readObj.CellBSP.Root.Type);
            Assert.IsNotNull(readObj.PhysicsBSP);
            Assert.AreEqual(BSPNodeType.Leaf, readObj.PhysicsBSP.Root.Type);
            Assert.IsNotNull(readObj.DrawingBSP);
            Assert.AreEqual(BSPNodeType.Leaf, readObj.DrawingBSP.Root.Type);

            Assert.AreEqual(1, readObj.PhysicsPolygons.Count);
            Assert.AreEqual(1, readObj.PhysicsPolygons.First().Key);
            Assert.AreEqual(StipplingType.Both, readObj.PhysicsPolygons.First().Value.Stippling);
            Assert.AreEqual(1, readObj.PhysicsPolygons.First().Value.NegSurface);
            Assert.AreEqual(1, readObj.PhysicsPolygons.First().Value.PosSurface);
            Assert.AreEqual(1, readObj.PhysicsPolygons.First().Value.NegUVIndices.Count);
            Assert.AreEqual(1, readObj.PhysicsPolygons.First().Value.NegUVIndices.First());
            Assert.AreEqual(1, readObj.PhysicsPolygons.First().Value.PosUVIndices.Count);
            Assert.AreEqual(2, readObj.PhysicsPolygons.First().Value.PosUVIndices.First());
            Assert.AreEqual(CullMode.Clockwise, readObj.PhysicsPolygons.First().Value.SidesType);
            Assert.AreEqual(1, readObj.PhysicsPolygons.First().Value.VertexIds.Count);
            Assert.AreEqual(3, readObj.PhysicsPolygons.First().Value.VertexIds.First());

            Assert.AreEqual(1, readObj.Polygons.Count);
            Assert.AreEqual(1, readObj.Polygons.First().Key);
            Assert.AreEqual(StipplingType.Both, readObj.Polygons.First().Value.Stippling);
            Assert.AreEqual(1, readObj.Polygons.First().Value.NegSurface);
            Assert.AreEqual(1, readObj.Polygons.First().Value.PosSurface);
            Assert.AreEqual(1, readObj.Polygons.First().Value.NegUVIndices.Count);
            Assert.AreEqual(1, readObj.Polygons.First().Value.NegUVIndices.First());
            Assert.AreEqual(1, readObj.Polygons.First().Value.PosUVIndices.Count);
            Assert.AreEqual(6, readObj.Polygons.First().Value.PosUVIndices.First());
            Assert.AreEqual(CullMode.Clockwise, readObj.Polygons.First().Value.SidesType);
            Assert.AreEqual(1, readObj.Polygons.First().Value.VertexIds.Count);
            Assert.AreEqual(3, readObj.Polygons.First().Value.VertexIds.First());

            Assert.AreEqual(3, readObj.Portals.Count);
            Assert.AreEqual(1, readObj.Portals[0]);
            Assert.AreEqual(2, readObj.Portals[1]);
            Assert.AreEqual(3, readObj.Portals[2]);
        }
    }
}
