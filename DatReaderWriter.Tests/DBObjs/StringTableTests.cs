using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class StringTableTests {
        [TestMethod]
        public void CanInsertAndRead() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);
/*
            var writeObj = new StringTable() {
                Id = 0x23000001u,
                Language = 1,
                Strings = {
                    { 0,  new StringTableString() {
                        VarNames = ["test", "asdf"],
                        Vars = ["test2", "asdf2"],
                        Comments = [1, 2],
                        Strings = ["foo", "bar"],
                    } }
                }
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryGet<StringTable>(0x23000001u, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x23000001u, readObj.Id);

            Assert.AreEqual(1, readObj.Strings.Count);
            Assert.AreEqual(2, readObj.Strings[0].VarNames.Count);
            Assert.AreEqual("test", readObj.Strings[0].VarNames[0]);
            Assert.AreEqual("asdf", readObj.Strings[0].VarNames[1]);
            Assert.AreEqual(2, readObj.Strings[0].Vars.Count);
            Assert.AreEqual("test2", readObj.Strings[0].Vars[0]);
            Assert.AreEqual("asdf2", readObj.Strings[0].Vars[1]);
            Assert.AreEqual(2, readObj.Strings[0].Comments.Count);
            Assert.AreEqual(1u, readObj.Strings[0].Comments[0]);
            Assert.AreEqual(2u, readObj.Strings[0].Comments[1]);
            Assert.AreEqual(2, readObj.Strings[0].Strings.Count);
            Assert.AreEqual("foo", readObj.Strings[0].Strings[0]);
            Assert.AreEqual("bar", readObj.Strings[0].Strings[1]);
*/
            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEOR() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_local_English.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });


            var res = dat.TryGet<StringTable>(0x23000001u, out var strTbl);
            Assert.IsTrue(res);
            Assert.IsNotNull(strTbl);
            Assert.AreEqual(0x23000001u, strTbl.Id);

            Assert.AreEqual(1u, strTbl.Language);

            Assert.AreEqual(502, strTbl.Strings.Count);

            var first = strTbl.Strings[0x52BA517];
            Assert.AreEqual(0u, first.DataId);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<StringTable>(Path.Combine(EORCommonData.DatDirectory, $"client_local_English.dat"), 0x23000001u);
            TestHelpers.CanReadAndWriteIdentical<StringTable>(Path.Combine(EORCommonData.DatDirectory, $"client_local_English.dat"), 0x23000002u);
            TestHelpers.CanReadAndWriteIdentical<StringTable>(Path.Combine(EORCommonData.DatDirectory, $"client_local_English.dat"), 0x23000003u);
        }
    }
}
