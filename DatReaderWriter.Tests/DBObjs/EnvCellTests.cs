using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class EnvCellTests {
        [TestMethod]
        public void CanInsertAndReadEnvCells() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Cell, 0);

            var envCell1 = new EnvCell() {
                Id = 0x00010100,
                Flags = EnvCellFlags.HasRestrictionObj | EnvCellFlags.HasStaticObjs,
                CellPortals = [
                    new CellPortal(){
                        Flags = PortalFlags.ExactMatch,
                        OtherCellId = 100,
                        OtherPortalId = 200,
                        PolygonId = 300
                    }
                ],
                CellStructure = 123,
                EnvironmentId = 456,
                Position = new Frame() {
                    Origin = Vector3.UnitZ,
                    Orientation = Quaternion.Identity
                },
                VisibleCells = [1, 2, 3],
                RestrictionObj = 0xBEEFu,
                Surfaces = [4, 5, 6, 7],
                StaticObjects = [
                    new Stab() {
                        Id = 414,
                        Frame = new Frame() {
                            Orientation = new Quaternion(4, 3, 2, 1),
                            Origin = Vector3.One,
                        }
                    }    
                ]
            };

            var res = dat.TryWriteFile(envCell1);
            Assert.IsTrue(res);

            var res2 = dat.TryGet<EnvCell>(0x00010100, out var readEnvCell1);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readEnvCell1);

            Assert.AreEqual(0x00010100u, readEnvCell1.Id);
            Assert.AreEqual(EnvCellFlags.HasRestrictionObj | EnvCellFlags.HasStaticObjs, readEnvCell1.Flags);

            Assert.AreEqual(1, readEnvCell1.CellPortals.Count);
            Assert.AreEqual(PortalFlags.ExactMatch, readEnvCell1.CellPortals[0].Flags);
            Assert.AreEqual(100, readEnvCell1.CellPortals[0].OtherCellId);
            Assert.AreEqual(200, readEnvCell1.CellPortals[0].OtherPortalId);
            Assert.AreEqual(300, readEnvCell1.CellPortals[0].PolygonId);

            Assert.AreEqual(123, readEnvCell1.CellStructure);
            Assert.AreEqual(456, readEnvCell1.EnvironmentId);
            Assert.AreEqual(Vector3.UnitZ, readEnvCell1.Position.Origin);
            Assert.AreEqual(Quaternion.Identity, readEnvCell1.Position.Orientation);

            CollectionAssert.AreEqual(new List<ushort>() { 1, 2, 3 }, readEnvCell1.VisibleCells);
            CollectionAssert.AreEqual(new List<ushort>() { 4, 5, 6, 7 }, readEnvCell1.Surfaces);

            Assert.AreEqual(1, readEnvCell1.StaticObjects.Count);
            Assert.AreEqual(414u, readEnvCell1.StaticObjects[0].Id);
            Assert.AreEqual(Vector3.One, readEnvCell1.StaticObjects[0].Frame.Origin);
            Assert.AreEqual(new Quaternion(4, 3, 2, 1), readEnvCell1.StaticObjects[0].Frame.Orientation);

            Assert.AreEqual(0xBEEFu, readEnvCell1.RestrictionObj);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEOREnvCells() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_cell_1.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });


            var res = dat.TryGet<EnvCell>(0x00020102, out var envCell1);
            Assert.IsTrue(res);
            Assert.IsNotNull(envCell1);
            Assert.AreEqual(0x00020102u, envCell1.Id);

            CollectionAssert.AreEquivalent(new List<ushort>() { 0x10C, 0x10D, 0x10E, 0x34, 0x10F, 0x10C }, envCell1.Surfaces);
            Assert.AreEqual((ushort)0x2E6, envCell1.EnvironmentId);
            Assert.AreEqual((ushort)2, envCell1.CellStructure);
            Assert.AreEqual(new Vector3(0, -40f, -12f), envCell1.Position.Origin);
            Assert.AreEqual(new Quaternion(0, 0, -0.707107f, -0.707107f), envCell1.Position.Orientation);

            Assert.AreEqual(3, envCell1.CellPortals.Count);

            Assert.AreEqual(PortalFlags.PortalSide, envCell1.CellPortals[0].Flags);
            Assert.AreEqual(3, envCell1.CellPortals[0].PolygonId);
            Assert.AreEqual(0x104, envCell1.CellPortals[0].OtherCellId);
            Assert.AreEqual(3, envCell1.CellPortals[0].OtherPortalId);

            Assert.AreEqual(PortalFlags.PortalSide | PortalFlags.ExactMatch, envCell1.CellPortals[1].Flags);
            Assert.AreEqual(4, envCell1.CellPortals[1].PolygonId);
            Assert.AreEqual(0x101, envCell1.CellPortals[1].OtherCellId);
            Assert.AreEqual(1, envCell1.CellPortals[1].OtherPortalId);

            Assert.AreEqual(PortalFlags.ExactMatch, envCell1.CellPortals[2].Flags);
            Assert.AreEqual(5, envCell1.CellPortals[2].PolygonId);
            Assert.AreEqual(0x103, envCell1.CellPortals[2].OtherCellId);
            Assert.AreEqual(2, envCell1.CellPortals[2].OtherPortalId);

            CollectionAssert.AreEquivalent(new List<ushort>() { 0x101, 0x100, 0x103, 0x104, 0x110, 0x10D, 0x10E, 0x10F, 0x10B, 0x10A, 0x108, 0x105, 0x107, 0x106, 0x109, 0x10C, 0x113, 0x112, 0x114, 0x111 }, envCell1.VisibleCells);


            var res2 = dat.TryGet<EnvCell>(0x7d64010du, out var envCell2);
            Assert.IsTrue(res2);
            Assert.IsNotNull(envCell2);
            Assert.AreEqual(0x7d64010du, envCell2.Id);

            Assert.AreEqual(EnvCellFlags.SeenOutside | EnvCellFlags.HasStaticObjs, envCell2.Flags);
            Assert.AreEqual(16, envCell2.Surfaces.Count);

            Assert.AreEqual(0x0372, envCell2.Surfaces.First());
            Assert.AreEqual(0x071B, envCell2.Surfaces.Last());

            Assert.AreEqual(0x03B2, envCell2.EnvironmentId);
            Assert.AreEqual(13, envCell2.CellStructure);

            Assert.AreEqual(new Vector3(85.5f, 109.5f, 12f), envCell2.Position.Origin);
            Assert.AreEqual(new Quaternion(0, 0, -0.707107f, 0.707107f), envCell2.Position.Orientation);

            Assert.AreEqual(2, envCell2.CellPortals.Count);

            Assert.AreEqual(PortalFlags.ExactMatch, envCell2.CellPortals[0].Flags);
            Assert.AreEqual(13, envCell2.CellPortals[0].PolygonId);
            Assert.AreEqual(0x105, envCell2.CellPortals[0].OtherCellId);
            Assert.AreEqual(1, envCell2.CellPortals[0].OtherPortalId);

            Assert.AreEqual(PortalFlags.ExactMatch, envCell2.CellPortals[1].Flags);
            Assert.AreEqual(12, envCell2.CellPortals[1].PolygonId);
            Assert.AreEqual(0x109, envCell2.CellPortals[1].OtherCellId);
            Assert.AreEqual(0, envCell2.CellPortals[1].OtherPortalId);

            Assert.AreEqual(13, envCell2.VisibleCells.Count);
            Assert.AreEqual(0x100, envCell2.VisibleCells[0]);
            Assert.AreEqual(0x101, envCell2.VisibleCells[1]);
            Assert.AreEqual(0x10C, envCell2.VisibleCells.Last());

            Assert.AreEqual(4, envCell2.StaticObjects.Count);
            Assert.AreEqual(0x0200033Fu, envCell2.StaticObjects[0].Id);
            Assert.AreEqual(new Vector3(77, 118, 12), envCell2.StaticObjects[0].Frame.Origin);
            Assert.AreEqual(new Quaternion(0, 0, -0.707107f, 0.707107f), envCell2.StaticObjects[0].Frame.Orientation);

            Assert.AreEqual(0x0200016Du, envCell2.StaticObjects[1].Id);
            Assert.AreEqual(new Vector3(79.725f, 113.1f, 12), envCell2.StaticObjects[1].Frame.Origin);
            Assert.AreEqual(new Quaternion(0, 0, -0.707107f, 0.707107f), envCell2.StaticObjects[1].Frame.Orientation);

            Assert.AreEqual(0x0200036Eu, envCell2.StaticObjects[2].Id);
            Assert.AreEqual(new Vector3(80.5f, 118.4f, 12), envCell2.StaticObjects[2].Frame.Origin);
            Assert.AreEqual(new Quaternion(0, 0, -0.707107f, 0.707107f), envCell2.StaticObjects[2].Frame.Orientation);

            Assert.AreEqual(0x02000372u, envCell2.StaticObjects[3].Id);
            Assert.AreEqual(new Vector3(80.675f, 114.325f, 14.375f), envCell2.StaticObjects[3].Frame.Origin);
            Assert.AreEqual(new Quaternion(0, 0, -0.707107f, 0.707107f), envCell2.StaticObjects[3].Frame.Orientation);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<EnvCell>(Path.Combine(EORCommonData.DatDirectory, $"client_cell_1.dat"), 0x00020102u);
            TestHelpers.CanReadAndWriteIdentical<EnvCell>(Path.Combine(EORCommonData.DatDirectory, $"client_cell_1.dat"), 0x7D64010Du);
            TestHelpers.CanReadAndWriteIdentical<EnvCell>(Path.Combine(EORCommonData.DatDirectory, $"client_cell_1.dat"), 0x7D7E0111u);
        }
    }
}
