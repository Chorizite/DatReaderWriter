using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class SoundTableTests {
        [TestMethod]
        public void CanInsertAndRead() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new SoundTable() {
                Id = 0x20000001u,
                HashKey = 123,
                Hashes = new () {
                    { 1, new SoundHashData() { Priority = 0f, Probability = 1f, Volume = 1f } }
                },
                Sounds = new Dictionary<Sound, SoundData>() {
                    { Sound.ArrowLand, new SoundData() { Entries = new List<SoundEntry>() { new SoundEntry() { Id = 1, Priority = 0f, Probability = 1f, Volume = 1f } } } }
                }
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryGet<SoundTable>(0x20000001u, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x20000001u, readObj.Id);
            
            Assert.AreEqual(123, readObj.HashKey);
            
            Assert.AreEqual(1, readObj.Hashes.Count);
            Assert.AreEqual(0f, readObj.Hashes[1u].Priority);
            Assert.AreEqual(1f, readObj.Hashes[1u].Probability);
            Assert.AreEqual(1f, readObj.Hashes[1u].Volume);

            Assert.AreEqual(1, readObj.Sounds.Count);
            Assert.AreEqual(1, readObj.Sounds[Sound.ArrowLand].Entries.Count);
            Assert.AreEqual(1u, readObj.Sounds[Sound.ArrowLand].Entries.First().Id);
            Assert.AreEqual(0f, readObj.Sounds[Sound.ArrowLand].Entries.First().Priority);
            Assert.AreEqual(1f, readObj.Sounds[Sound.ArrowLand].Entries.First().Probability);
            Assert.AreEqual(1f, readObj.Sounds[Sound.ArrowLand].Entries.First().Volume);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEOR() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });


            var res = dat.TryGet<SoundTable>(0x20000001u, out var readObj);
            Assert.IsTrue(res);
            Assert.IsNotNull(readObj);
            Assert.AreEqual(0x20000001u, readObj.Id);
            
            Assert.AreEqual(0, readObj.HashKey);

            Assert.AreEqual(1, readObj.Hashes.Count);
            Assert.AreEqual(0f, readObj.Hashes[0u].Priority);
            Assert.AreEqual(1f, readObj.Hashes[0u].Probability);
            Assert.AreEqual(1f, readObj.Hashes[0u].Volume);

            Assert.AreEqual(52, readObj.Sounds.Count);

            Assert.AreEqual(1, readObj.Sounds[Sound.ShieldUp].Entries.Count);
            Assert.AreEqual(0x0A000262u, readObj.Sounds[Sound.ShieldUp].Entries.First().Id);
            Assert.AreEqual(0.7f, readObj.Sounds[Sound.ShieldUp].Entries.First().Priority);
            Assert.AreEqual(1f, readObj.Sounds[Sound.ShieldUp].Entries.First().Probability);
            Assert.AreEqual(1f, readObj.Sounds[Sound.ShieldUp].Entries.First().Volume);

            Assert.AreEqual(1, readObj.Sounds[Sound.EnchantDown].Entries.Count);
            Assert.AreEqual(0x0A000274u, readObj.Sounds[Sound.EnchantDown].Entries.First().Id);
            Assert.AreEqual(0.7f, readObj.Sounds[Sound.EnchantDown].Entries.First().Priority);
            Assert.AreEqual(1f, readObj.Sounds[Sound.EnchantDown].Entries.First().Probability);
            Assert.AreEqual(0.8f, readObj.Sounds[Sound.EnchantDown].Entries.First().Volume);


            var res2 = dat.TryGet<SoundTable>(0x200000A8u, out var readObj2);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj2);
            Assert.AreEqual(0x200000A8u, readObj2.Id);
            
            Assert.AreEqual(0, readObj2.HashKey);

            Assert.AreEqual(1, readObj2.Hashes.Count);
            Assert.AreEqual(32, readObj2.Sounds.Count);

            Assert.IsTrue(readObj2.Sounds.ContainsKey(Sound.Swoosh2));
            Assert.AreEqual(2, readObj2.Sounds[Sound.Swoosh2].Entries.Count);
            Assert.AreEqual(0xA000519u, readObj2.Sounds[Sound.Swoosh2].Entries.First().Id);
            Assert.AreEqual(0xA00051Eu, readObj2.Sounds[Sound.Swoosh2].Entries.Last().Id);


            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<SoundTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x20000001u);
            TestHelpers.CanReadAndWriteIdentical<SoundTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x20000002u);
            TestHelpers.CanReadAndWriteIdentical<SoundTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x20000003u);
            TestHelpers.CanReadAndWriteIdentical<SoundTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x200000A8u);
        }
    }
}
