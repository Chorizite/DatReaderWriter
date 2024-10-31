using DatReaderWriter.Tests.Lib;
using ACClientLib.DatReaderWriter;
using ACClientLib.DatReaderWriter.Options;
using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.DBObjs;
using ACClientLib.DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class LandBlockInfoTests {
        [TestMethod]
        public void CanInsertAndReadLandblockInfos() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabaseReader(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Cell, 0);

            var landblock1 = new LandBlockInfo() {
                Id = 0x0001FFFE,
                NumCells = 6,
                Objects = [
                    new Stab(){
                        Id = 1,
                        Frame = new Frame(){
                            Origin = Vector3.UnitX,
                            Orientation = Quaternion.Identity
                        },
                    }
                ],
                Buildings = [
                    new BuildingInfo() {
                        ModelId = 1000,
                        Frame = new Frame() {
                            Origin = Vector3.UnitY,
                            Orientation = new Quaternion(1, 2, 3, 4)
                        },
                        NumLeaves = 5,
                        Portals = [
                            new BuildingPortal() {
                                Flags = PortalFlags.ExactMatch,
                                OtherCellId = 5,
                                OtherPortalId = 6,
                                StabList = [1, 2, 3, 4, 5]
                            }
                        ]
                    }
                ],
                RestrictionTable = new Dictionary<uint, uint>() {
                    { 1, 1 },
                    { 2, 2 }
                }
            };
            var landblock2 = new LandBlockInfo() {
                Id = 0x0002FFFE,
                NumCells = 300,
                Objects = [],
                Buildings = []
            };

            var res = dat.TryWriteFile(landblock1);
            Assert.IsTrue(res);

            res = dat.TryWriteFile(landblock2);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<LandBlockInfo>(0x0001FFFE, out var readLandblock1);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readLandblock1);

            Assert.AreEqual(0x0001FFFEu, readLandblock1.Id);
            Assert.AreEqual(6u, readLandblock1.NumCells);

            Assert.AreEqual(1, readLandblock1.Objects.Count);
            Assert.AreEqual(1u, readLandblock1.Objects[0].Id);
            Assert.AreEqual(Vector3.UnitX, readLandblock1.Objects[0].Frame.Origin);
            Assert.AreEqual(Quaternion.Identity, readLandblock1.Objects[0].Frame.Orientation);
            
            Assert.AreEqual(1, readLandblock1.Buildings.Count);
            Assert.AreEqual(1000u, readLandblock1.Buildings[0].ModelId);
            Assert.AreEqual(Vector3.UnitY, readLandblock1.Buildings[0].Frame.Origin);
            Assert.AreEqual(new Quaternion(1, 2, 3, 4), readLandblock1.Buildings[0].Frame.Orientation);
            Assert.AreEqual(5u, readLandblock1.Buildings[0].NumLeaves);
            Assert.AreEqual(1, readLandblock1.Buildings[0].Portals.Count);
            Assert.AreEqual(PortalFlags.ExactMatch, readLandblock1.Buildings[0].Portals[0].Flags);
            Assert.AreEqual((ushort)5, readLandblock1.Buildings[0].Portals[0].OtherCellId);
            Assert.AreEqual((ushort)6, readLandblock1.Buildings[0].Portals[0].OtherPortalId);
            CollectionAssert.AreEqual(new List<ushort>() { 1, 2, 3, 4, 5 }, readLandblock1.Buildings[0].Portals[0].StabList);


            Assert.AreEqual(2, readLandblock1.RestrictionTable.Count);
            Assert.AreEqual(1u, readLandblock1.RestrictionTable[1]);
            Assert.AreEqual(2u, readLandblock1.RestrictionTable[2]);

            res2 = dat.TryReadFile<LandBlockInfo>(0x0002FFFE, out var readLandblock2);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readLandblock2);

            Assert.AreEqual(0x0002FFFEu, readLandblock2.Id);
            Assert.AreEqual(300u, readLandblock2.NumCells);
            Assert.AreEqual(0, readLandblock2.Objects.Count);
            Assert.AreEqual(0, readLandblock2.Buildings.Count);
            Assert.AreEqual(0, readLandblock2.RestrictionTable.Count);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORLandblockInfos() {
            using var dat = new DatDatabaseReader(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_cell_1.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });


            var res = dat.TryReadFile<LandBlockInfo>(0x1455FFFE, out var landblock1);
            Assert.IsTrue(res);
            Assert.IsNotNull(landblock1);

            Assert.AreEqual(5u, landblock1.NumCells);
            Assert.AreEqual(1, landblock1.Buildings.Count);

            Assert.AreEqual(0x010011A9u, landblock1.Buildings[0].ModelId);
            Assert.AreEqual(127u, landblock1.Buildings[0].NumLeaves);
            Assert.AreEqual(new Vector3(174.003f, 17.634f, 70f), landblock1.Buildings[0].Frame.Origin);
            Assert.AreEqual(new Quaternion(0, 0, -0.91965f, 0.392739f), landblock1.Buildings[0].Frame.Orientation);

            Assert.AreEqual(1, landblock1.Buildings[0].Portals.Count);
            Assert.AreEqual(PortalFlags.PortalSide | PortalFlags.ExactMatch, landblock1.Buildings[0].Portals[0].Flags);
            Assert.AreEqual(260, landblock1.Buildings[0].Portals[0].OtherCellId);
            Assert.AreEqual(1, landblock1.Buildings[0].Portals[0].OtherPortalId);

            CollectionAssert.AreEqual(new List<ushort>() { 256, 257, 258, 259, 260 }, landblock1.Buildings[0].Portals[0].StabList);
            Assert.AreEqual(0, landblock1.RestrictionTable.Count);


            res = dat.TryReadFile<LandBlockInfo>(0x5262FFFE, out var landblock2);
            Assert.IsTrue(res);
            Assert.IsNotNull(landblock2);
            Assert.AreEqual(74u, landblock2.NumCells);
            Assert.AreEqual(0, landblock2.Buildings.Count);
            Assert.AreEqual(0, landblock2.Objects.Count);

            Assert.AreEqual(1, landblock2.RestrictionTable.Count);
            Assert.AreEqual(0x7526200Cu, landblock2.RestrictionTable[0]);

            dat.Dispose();
        }
    }
}
