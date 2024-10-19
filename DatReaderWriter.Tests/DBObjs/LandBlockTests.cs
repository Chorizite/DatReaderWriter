using DatReaderWriter.Tests.Lib;
using ACClientLib.DatReaderWriter;
using ACClientLib.DatReaderWriter.Options;
using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.DBObjs;
using ACClientLib.DatReaderWriter.Types;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class LandBlockTests {
        [TestMethod]
        public void CanInsertAndReadLandblocks() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabaseReader(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatDatabaseType.Cell, 0);

            var landblock1 = new LandBlock() {
                Id = 0x0001FFFF,
                HasObjects = false,
                Height = Enumerable.Range(0, 81).Select(v => (byte)v).ToArray(),
                Terrain = Enumerable.Range(0, 81).Select(v => new TerrainInfo() {
                    Road = 1,
                    Type = TerrainType.LushGrass,
                    Scenery = 15
                }).ToArray()
            };
            var landblock2 = new LandBlock() {
                Id = 0x0002FFFF,
                HasObjects = true,
                Height = Enumerable.Range(0, 81).Select(v => (byte)0xAB).ToArray(),
                Terrain = Enumerable.Range(0, 81).Select(v => new TerrainInfo() {
                    Road = (byte)(v > 60 ? 0 : 1),
                    Type = v > 40 ? TerrainType.ObsidianPlain : TerrainType.MudRichDirt,
                    Scenery = (ushort)(v > 10 ? 5 : 20)
                }).ToArray()
            };

            var res = dat.TryWriteFile(landblock1);
            Assert.IsTrue(res);

            res = dat.TryWriteFile(landblock2);
            Assert.IsTrue(res);

            res = dat.TryReadFile<LandBlock>(0x0001FFFF, out var readLandblock1);
            Assert.IsTrue(res);
            Assert.IsNotNull(readLandblock1);

            Assert.AreEqual(0x0001FFFFu, readLandblock1.Id);
            Assert.AreEqual(false, readLandblock1.HasObjects);
            CollectionAssert.AreEqual(Enumerable.Range(0, 81).Select(v => (byte)v).ToArray(), readLandblock1.Height);

            var expected1 = Enumerable.Range(0, 81).Select(v => new TerrainInfo() {
                Road = 1,
                Type = TerrainType.LushGrass,
                Scenery = 15
            }).ToArray();
            for (var i = 0; i < expected1.Length; i++) {
                var expected = expected1[i];
                Assert.AreEqual(expected.Scenery, readLandblock1.Terrain[i].Scenery, $"Scenery failed: {i}");
                Assert.AreEqual(expected.Type, readLandblock1.Terrain[i].Type, $"Type failed: {i}");
                Assert.AreEqual(expected.Road, readLandblock1.Terrain[i].Road, $"Road failed: {i}");
            }

            res = dat.TryReadFile<LandBlock>(0x0002FFFF, out var readLandblock2);
            Assert.IsTrue(res);
            Assert.IsNotNull(readLandblock2);

            Assert.AreEqual(0x0002FFFFu, readLandblock2.Id);
            Assert.AreEqual(true, readLandblock2.HasObjects);
            CollectionAssert.AreEqual(Enumerable.Range(0, 81).Select(v => (byte)0xAB).ToArray(), readLandblock2.Height);

            var expected2 = Enumerable.Range(0, 81).Select(v => new TerrainInfo() {
                Road = (byte)(v > 60 ? 0 : 1),
                Type = v > 40 ? TerrainType.ObsidianPlain : TerrainType.MudRichDirt,
                Scenery = (ushort)(v > 10 ? 5 : 20)
            }).ToArray();
            for (var i = 0; i < expected2.Length; i++) {
                var expected = expected2[i];
                Assert.AreEqual(expected.Scenery, readLandblock2.Terrain[i].Scenery, $"Scenery failed: {i}");
                Assert.AreEqual(expected.Type, readLandblock2.Terrain[i].Type, $"Type failed: {i}");
                Assert.AreEqual(expected.Road, readLandblock2.Terrain[i].Road, $"Road failed: {i}");
            }

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORLandblocks() {
            using var dat = new DatDatabaseReader(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_cell_1.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });


            var res = dat.TryReadFile<LandBlock>(0x9AB1FFFF, out var landblock1);
            Assert.IsTrue(res);
            Assert.IsNotNull(landblock1);

            Assert.AreEqual(false, landblock1.HasObjects);
            Assert.AreEqual(81, landblock1.Height.Length);
            Assert.AreEqual(81, landblock1.Terrain.Length);

            Assert.AreEqual((byte)0, landblock1.Terrain[0].Road);
            Assert.AreEqual(TerrainType.SedimentaryRock, landblock1.Terrain[0].Type);
            Assert.AreEqual((ushort)9, landblock1.Terrain[0].Scenery);

            Assert.AreEqual((byte)0, landblock1.Terrain[1].Road);
            Assert.AreEqual(TerrainType.PatchyGrassland, landblock1.Terrain[1].Type);
            Assert.AreEqual((ushort)13, landblock1.Terrain[1].Scenery);

            Assert.AreEqual((byte)0, landblock1.Terrain[80].Road);
            Assert.AreEqual(TerrainType.SedimentaryRock, landblock1.Terrain[80].Type);
            Assert.AreEqual((ushort)11, landblock1.Terrain[80].Scenery);

            Assert.AreEqual((ushort)120, landblock1.Height[0]);
            Assert.AreEqual((ushort)139, landblock1.Height[1]);
            Assert.AreEqual((ushort)93, landblock1.Height[40]);
            Assert.AreEqual((ushort)93, landblock1.Height[79]);
            Assert.AreEqual((ushort)110, landblock1.Height[80]);

            dat.Dispose();
        }
    }
}
