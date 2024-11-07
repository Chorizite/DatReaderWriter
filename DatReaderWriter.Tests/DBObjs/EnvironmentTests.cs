using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using Environment = DatReaderWriter.DBObjs.Environment;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class EnvironmentTests {
        [TestMethod]
        public void CanInsertAndReadEnvironments() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var environment = new Environment() {
                Id = 0xD000001,
                Cells = new Dictionary<uint, CellStruct>() {
                    { 4123, new CellStruct(){
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
                            RootNode = new BSPLeaf() {
                                Type = "LEAF"
                            }
                        },
                        DrawingBSP = new DrawingBSPTree() {
                            RootNode = new BSPLeaf() {
                                Type = "LEAF"
                            }
                        },
                        PhysicsBSP = new PhysicsBSPTree() {
                            RootNode = new BSPLeaf() {
                                Type = "LEAF"
                            }
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
                    } }
                },
            };

            var res = dat.TryWriteFile(environment);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<Environment>(0xD000001, out var readEnv);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readEnv);

            Assert.AreEqual(0xD000001u, readEnv.Id);
            Assert.IsNotNull(readEnv.Cells);
            Assert.AreEqual(1, readEnv.Cells.Count);

            Assert.AreEqual(4123u, readEnv.Cells.First().Key);
            Assert.IsNotNull(readEnv.Cells.First().Value.VertexArray);
            Assert.AreEqual(1, readEnv.Cells.First().Value.VertexArray.Vertices.Count);
            Assert.AreEqual(12, readEnv.Cells.First().Value.VertexArray.Vertices.First().Key);
            Assert.AreEqual(Vector3.UnitX, readEnv.Cells.First().Value.VertexArray.Vertices.First().Value.Normal);
            Assert.AreEqual(Vector3.UnitY, readEnv.Cells.First().Value.VertexArray.Vertices.First().Value.Origin);

            Assert.AreEqual(2, readEnv.Cells.First().Value.VertexArray.Vertices.First().Value.UVs.Count);
            Assert.AreEqual(3, readEnv.Cells.First().Value.VertexArray.Vertices.First().Value.UVs.First().U);
            Assert.AreEqual(2, readEnv.Cells.First().Value.VertexArray.Vertices.First().Value.UVs.First().V);
            Assert.AreEqual(2, readEnv.Cells.First().Value.VertexArray.Vertices.First().Value.UVs.Skip(1).First().U);
            Assert.AreEqual(1, readEnv.Cells.First().Value.VertexArray.Vertices.First().Value.UVs.Skip(1).First().V);

            Assert.IsNotNull(readEnv.Cells.First().Value.CellBSP);
            Assert.IsNotNull("LEAF", readEnv.Cells.First().Value.CellBSP.RootNode.Type);
            Assert.IsNotNull(readEnv.Cells.First().Value.PhysicsBSP);
            Assert.IsNotNull("LEAF", readEnv.Cells.First().Value.PhysicsBSP.RootNode.Type);
            Assert.IsNotNull(readEnv.Cells.First().Value.DrawingBSP);
            Assert.IsNotNull("LEAF", readEnv.Cells.First().Value.DrawingBSP.RootNode.Type);

            Assert.AreEqual(1, readEnv.Cells.First().Value.PhysicsPolygons.Count);
            Assert.AreEqual(1, readEnv.Cells.First().Value.PhysicsPolygons.First().Key);
            Assert.AreEqual(StipplingType.Both, readEnv.Cells.First().Value.PhysicsPolygons.First().Value.Stippling);
            Assert.AreEqual(1, readEnv.Cells.First().Value.PhysicsPolygons.First().Value.NegSurface);
            Assert.AreEqual(1, readEnv.Cells.First().Value.PhysicsPolygons.First().Value.PosSurface);
            Assert.AreEqual(1, readEnv.Cells.First().Value.PhysicsPolygons.First().Value.NegUVIndices.Count);
            Assert.AreEqual(1, readEnv.Cells.First().Value.PhysicsPolygons.First().Value.NegUVIndices.First());
            Assert.AreEqual(1, readEnv.Cells.First().Value.PhysicsPolygons.First().Value.PosUVIndices.Count);
            Assert.AreEqual(2, readEnv.Cells.First().Value.PhysicsPolygons.First().Value.PosUVIndices.First());
            Assert.AreEqual(CullMode.Clockwise, readEnv.Cells.First().Value.PhysicsPolygons.First().Value.SidesType);
            Assert.AreEqual(1, readEnv.Cells.First().Value.PhysicsPolygons.First().Value.VertexIds.Count);
            Assert.AreEqual(3, readEnv.Cells.First().Value.PhysicsPolygons.First().Value.VertexIds.First());

            Assert.AreEqual(1, readEnv.Cells.First().Value.Polygons.Count);
            Assert.AreEqual(1, readEnv.Cells.First().Value.Polygons.First().Key);
            Assert.AreEqual(StipplingType.Both, readEnv.Cells.First().Value.Polygons.First().Value.Stippling);
            Assert.AreEqual(1, readEnv.Cells.First().Value.Polygons.First().Value.NegSurface);
            Assert.AreEqual(1, readEnv.Cells.First().Value.Polygons.First().Value.PosSurface);
            Assert.AreEqual(1, readEnv.Cells.First().Value.Polygons.First().Value.NegUVIndices.Count);
            Assert.AreEqual(1, readEnv.Cells.First().Value.Polygons.First().Value.NegUVIndices.First());
            Assert.AreEqual(1, readEnv.Cells.First().Value.Polygons.First().Value.PosUVIndices.Count);
            Assert.AreEqual(6, readEnv.Cells.First().Value.Polygons.First().Value.PosUVIndices.First());
            Assert.AreEqual(CullMode.Clockwise, readEnv.Cells.First().Value.Polygons.First().Value.SidesType);
            Assert.AreEqual(1, readEnv.Cells.First().Value.Polygons.First().Value.VertexIds.Count);
            Assert.AreEqual(3, readEnv.Cells.First().Value.Polygons.First().Value.VertexIds.First());

            Assert.AreEqual(3, readEnv.Cells.First().Value.Portals.Count);
            Assert.AreEqual(1, readEnv.Cells.First().Value.Portals[0]);
            Assert.AreEqual(2, readEnv.Cells.First().Value.Portals[1]);
            Assert.AreEqual(3, readEnv.Cells.First().Value.Portals[2]);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEOREnvironments() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.OnDemand;
            });


            var res = dat.TryReadFile<Environment>(0x0D00062E, out var env);
            Assert.IsTrue(res);
            Assert.IsNotNull(env);

            Assert.AreEqual(0u, env.DataCategory);
            Assert.AreEqual(1, env.Cells.Count);
            Assert.AreEqual(VertexType.CSWVertexType, env.Cells[0].VertexArray.VertexType);
            Assert.AreEqual(8, env.Cells[0].VertexArray.Vertices.Count);
            Assert.AreEqual(new Vector3(4, -0.6f, 1.8f), env.Cells[0].VertexArray.Vertices[0].Origin);

            Assert.AreEqual(6, env.Cells[0].Polygons.Count);

            var firstPoly = env.Cells[0].Polygons.First().Value;
            Assert.AreEqual(StipplingType.NoPos, firstPoly.Stippling);
            Assert.AreEqual(CullMode.Landblock, firstPoly.SidesType);
            Assert.AreEqual(0, firstPoly.PosSurface);
            Assert.AreEqual(-1, firstPoly.NegSurface);
            Assert.AreEqual("0,1,2,3", string.Join(",", firstPoly.VertexIds));

            var lastPoly = env.Cells[0].Polygons.Last().Value;
            Assert.AreEqual(StipplingType.NoPos, lastPoly.Stippling);
            Assert.AreEqual(CullMode.Landblock, lastPoly.SidesType);
            Assert.AreEqual(0, lastPoly.PosSurface);
            Assert.AreEqual(-1, lastPoly.NegSurface);
            Assert.AreEqual("7,6,5,4", string.Join(",", lastPoly.VertexIds));

            Assert.AreEqual("BPnn", env.Cells[0].CellBSP.RootNode.Type);
            Assert.AreEqual(-1.8f, env.Cells[0].CellBSP.RootNode.SplittingPlane.D);

            Assert.AreEqual(6, env.Cells[0].PhysicsPolygons.Count);
            var firstPhysicsPoly = env.Cells[0].PhysicsPolygons.First().Value;
            Assert.AreEqual(StipplingType.NoPos, firstPhysicsPoly.Stippling);
            Assert.AreEqual(CullMode.Landblock, firstPhysicsPoly.SidesType);
            Assert.AreEqual(-1, firstPhysicsPoly.PosSurface);
            Assert.AreEqual(-1, firstPhysicsPoly.NegSurface);
            Assert.AreEqual("3,2,1,0", string.Join(",", firstPhysicsPoly.VertexIds));

            var lastPhysicsPoly = env.Cells[0].PhysicsPolygons.Last().Value;
            Assert.AreEqual(StipplingType.NoPos, lastPhysicsPoly.Stippling);
            Assert.AreEqual(CullMode.Landblock, lastPhysicsPoly.SidesType);
            Assert.AreEqual(-1, lastPhysicsPoly.PosSurface);
            Assert.AreEqual(-1, lastPhysicsPoly.NegSurface);
            Assert.AreEqual("3,0,5,4", string.Join(",", lastPhysicsPoly.VertexIds));

            Assert.AreEqual("BPnN", env.Cells[0].PhysicsBSP.RootNode.Type);
            Assert.AreEqual(1.8f, env.Cells[0].PhysicsBSP.RootNode.SplittingPlane.D);
            Assert.AreEqual("LEAF", env.Cells[0].PhysicsBSP.RootNode.PosNode.Type);

            var leafNode = env.Cells[0].PhysicsBSP.RootNode.PosNode as BSPLeaf;
            Assert.IsNotNull(leafNode);
            Assert.AreEqual(0, leafNode.LeafIndex);
            Assert.AreEqual(0, leafNode.Solid);
            Assert.AreEqual("BPnN", env.Cells[0].PhysicsBSP.RootNode.NegNode.Type);

            Assert.AreEqual("BPIn", env.Cells[0].DrawingBSP.RootNode.Type);
            Assert.AreEqual(-1.8f, env.Cells[0].DrawingBSP.RootNode.SplittingPlane.D);
            Assert.AreEqual("BPIn", env.Cells[0].DrawingBSP.RootNode.PosNode.Type);

            dat.Dispose();
        }
    }
}
