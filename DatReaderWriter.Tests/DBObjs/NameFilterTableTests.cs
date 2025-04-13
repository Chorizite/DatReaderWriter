using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class NameFilterTableTests {
        [TestMethod]
        public void CanInsertAndReadNameFilterTable() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new NameFilterTable() {
                Id = 0x0E000020u,
                LanguageData = new Dictionary<uint, NameFilterLanguageData> {
                    { 1, new NameFilterLanguageData() {
                        ExtraAllowedCharacters = "asdf",
                        FirstNCharactersMustHaveAVowel = 1,
                        MaximumSameCharactersInARow = 2,
                        MaximumVowelsInARow = 3,
                        VowelContainingSubstringLength = 4,
                        CompoundLetterGroups = ["th", "zs", "xy"],
                    } }
                }
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<NameFilterTable>(0x0E000020u, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x0E000020u, readObj.Id);

            Assert.AreEqual(1, readObj.LanguageData.Count);
            Assert.AreEqual("asdf", readObj.LanguageData[1].ExtraAllowedCharacters);
            Assert.AreEqual(1u, readObj.LanguageData[1].FirstNCharactersMustHaveAVowel);
            Assert.AreEqual(2u, readObj.LanguageData[1].MaximumSameCharactersInARow);
            Assert.AreEqual(3u, readObj.LanguageData[1].MaximumVowelsInARow);
            Assert.AreEqual(4u, readObj.LanguageData[1].VowelContainingSubstringLength);
            Assert.AreEqual(3, readObj.LanguageData[1].CompoundLetterGroups.Count);
            Assert.AreEqual("th", readObj.LanguageData[1].CompoundLetterGroups[0]);
            Assert.AreEqual("zs", readObj.LanguageData[1].CompoundLetterGroups[1]);
            Assert.AreEqual("xy", readObj.LanguageData[1].CompoundLetterGroups[2]);


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


            var res = dat.TryReadFile<NameFilterTable>(0x0E000020, out var nameFilterTable);
            Assert.IsTrue(res);
            Assert.IsNotNull(nameFilterTable);
            Assert.AreEqual(0x0E000020u, nameFilterTable.Id);

            Assert.AreEqual(1, nameFilterTable.LanguageData.Count);
            Assert.AreEqual("", nameFilterTable.LanguageData[1].ExtraAllowedCharacters);
            Assert.AreEqual(4u, nameFilterTable.LanguageData[1].FirstNCharactersMustHaveAVowel);
            Assert.AreEqual(2u, nameFilterTable.LanguageData[1].MaximumSameCharactersInARow);
            Assert.AreEqual(3u, nameFilterTable.LanguageData[1].MaximumVowelsInARow);
            Assert.AreEqual(5u, nameFilterTable.LanguageData[1].VowelContainingSubstringLength);
            Assert.AreEqual(3, nameFilterTable.LanguageData[1].CompoundLetterGroups.Count);
            Assert.AreEqual("th", nameFilterTable.LanguageData[1].CompoundLetterGroups[0]);
            Assert.AreEqual("ch", nameFilterTable.LanguageData[1].CompoundLetterGroups[1]);
            Assert.AreEqual("ph", nameFilterTable.LanguageData[1].CompoundLetterGroups[2]);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<NameFilterTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x0E000020u);
        }
    }
}
