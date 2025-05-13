using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class GfxObjTests {
        [TestMethod]
        public void CanInsertAndReadGfxObjs() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var gfxObj = new GfxObj() {
                Id = 0x1000001,
                VertexArray = new VertexArray(),
                
            };

            var res = dat.TryWriteFile(gfxObj);

            var res2 = dat.TryGet<GfxObj>(0x1000001, out var readEnv);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readEnv);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORGfxObjs() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.OnDemand;
            });


            var res = dat.TryGet<GfxObj>(0x010005E8, out var env);
            Assert.IsTrue(res);
            Assert.IsNotNull(env);

            Assert.AreEqual(0u, env.DataCategory);
            Assert.AreEqual(1, env.Surfaces.Count);
            Assert.AreEqual(0x080001B8u, env.Surfaces.First());

            Assert.AreEqual(4, env.VertexArray.Vertices.Count);
            Assert.AreEqual(VertexType.CSWVertexType, env.VertexArray.VertexType);

            Assert.AreEqual(new Vector3(0.05f, 0, -0.0759375f), env.VertexArray.Vertices.First().Value.Origin);
            Assert.AreEqual(new Vector3(0, -1, 0), env.VertexArray.Vertices.First().Value.Normal);
            Assert.AreEqual(1, env.VertexArray.Vertices.First().Value.UVs.Count);
            Assert.AreEqual(1, env.VertexArray.Vertices.First().Value.UVs.First().U);
            Assert.AreEqual(1, env.VertexArray.Vertices.First().Value.UVs.First().V);

            Assert.AreEqual(new Vector3(-0.05f, 0, -0.0759375f), env.VertexArray.Vertices.Last().Value.Origin);
            Assert.AreEqual(new Vector3(0, -1, 0), env.VertexArray.Vertices.Last().Value.Normal);
            Assert.AreEqual(1, env.VertexArray.Vertices.Last().Value.UVs.Count);
            Assert.AreEqual(0, env.VertexArray.Vertices.Last().Value.UVs.First().U);
            Assert.AreEqual(1, env.VertexArray.Vertices.Last().Value.UVs.First().V);

            Assert.AreEqual(new Vector3(0.024821f, 0, 0.0127083f), env.SortCenter);

            Assert.AreEqual(1, env.Polygons.Count);
            Assert.AreEqual(0, env.PhysicsPolygons.Count);
            Assert.AreEqual(GfxObjFlags.HasDrawing, env.Flags);

            Assert.IsNotNull(env.DrawingBSP);
            Assert.IsNull(env.PhysicsBSP);

            Assert.AreEqual(BSPNodeType.BPOL, env.DrawingBSP.Root.Type);
            Assert.IsNull(env.DrawingBSP.Root.PosNode);
            Assert.IsNull(env.DrawingBSP.Root.NegNode);
            Assert.AreEqual("0", string.Join(",", env.DrawingBSP.Root.Polygons));
            Assert.AreEqual(0.126001f, env.DrawingBSP.Root.BoundingSphere.Radius);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<GfxObj>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x01000331);
            TestHelpers.CanReadAndWriteIdentical<GfxObj>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x010005E8);
            TestHelpers.CanReadAndWriteIdentical<GfxObj>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x01001B2A);
            TestHelpers.CanReadAndWriteIdentical<GfxObj>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x01004E57);
            TestHelpers.CanReadAndWriteIdentical<GfxObj>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x01003E17);
        }
    }
}
