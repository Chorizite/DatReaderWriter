using DatReaderWriter.Tests.Lib;
using ACClientLib.DatReaderWriter;
using ACClientLib.DatReaderWriter.Options;
using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.DBObjs;
using ACClientLib.DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class EnvCellTests {
        [TestMethod]
        public void CanInsertAndReadEnvCells() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabaseReader(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatDatabaseType.Cell, 0);

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

            res = dat.TryReadFile<EnvCell>(0x00010100, out var readEnvCell1);
            Assert.IsTrue(res);
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
            using var dat = new DatDatabaseReader(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_cell_1.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });


            var res = dat.TryReadFile<EnvCell>(0x00020102, out var envCell1);
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

            dat.Dispose();
        }
    }
}
