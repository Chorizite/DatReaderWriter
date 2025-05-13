using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class ObjectHierarchyTests {
        [TestMethod]
        public void CanInsertAndRead() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new ObjectHierarchy() {
                Id = 0xE00000Du,
                RootNode = new ObjHierarchyNode() {
                    MenuName = "",
                    WCID = 0,
                    Children = [
                        new ObjHierarchyNode() { MenuName = "asdf", WCID = 1 },
                        new ObjHierarchyNode() { MenuName = "qwer", WCID = 2 }
                    ]
                }
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryGet<ObjectHierarchy>(0xE00000Du, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0xE00000Du, readObj.Id);

            Assert.AreEqual(writeObj.RootNode.MenuName, readObj.RootNode.MenuName);
            Assert.AreEqual(writeObj.RootNode.WCID, readObj.RootNode.WCID);
            Assert.AreEqual(2, readObj.RootNode.Children.Count);
            Assert.AreEqual(writeObj.RootNode.Children[0].MenuName, readObj.RootNode.Children[0].MenuName);
            Assert.AreEqual(writeObj.RootNode.Children[0].WCID, readObj.RootNode.Children[0].WCID);
            Assert.AreEqual(writeObj.RootNode.Children[1].MenuName, readObj.RootNode.Children[1].MenuName);
            Assert.AreEqual(writeObj.RootNode.Children[1].WCID, readObj.RootNode.Children[1].WCID);

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


            var res = dat.TryGet<ObjectHierarchy>(0xE00000D, out var objHierarchy);
            Assert.IsTrue(res);
            Assert.IsNotNull(objHierarchy);
            Assert.AreEqual(0xE00000Du, objHierarchy.Id);

            Assert.AreEqual("", objHierarchy.RootNode.MenuName);
            Assert.AreEqual(0u, objHierarchy.RootNode.WCID);

            Assert.AreEqual(2, objHierarchy.RootNode.Children.Count);

            Assert.AreEqual("PlayDay", objHierarchy.RootNode.Children[0].MenuName);
            Assert.AreEqual(0u, objHierarchy.RootNode.Children[0].WCID);
            Assert.AreEqual(3, objHierarchy.RootNode.Children[0].Children.Count);

            Assert.AreEqual("WeenieObjects", objHierarchy.RootNode.Children[1].MenuName);
            Assert.AreEqual(0u, objHierarchy.RootNode.Children[1].WCID);
            Assert.AreEqual(3, objHierarchy.RootNode.Children[0].Children.Count);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<ObjectHierarchy>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0xE00000Du);
        }
    }
}
