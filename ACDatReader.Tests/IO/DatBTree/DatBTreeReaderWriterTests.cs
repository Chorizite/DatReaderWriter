using ACDatReader.IO.BlockAllocators;
using ACDatReader.IO.DatBTree;
using ACDatReader.Tests.Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACDatReader.Tests.IO.DatBTree {
    [TestClass]
    public class DatBTreeReaderWriterTests {
        [TestMethod]
        [CombinatorialData]
        public void CanAddLotsOfFileEntries([DataValues(1, 10, 100, 1000)] int entryCount) {
            var datFilePath = Path.GetTempFileName();
            var allocator = new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = datFilePath,
                AccessType = Options.DatAccessType.ReadWrite
            });

            allocator.InitNew(DatDatabaseType.Portal, 0);

            var tree = new DatBTreeReaderWriter(allocator);

            Assert.AreEqual(0, allocator.Header.RootBlock);

            var files = new List<DatBTreeFile>();
            for (var i = 0; i < entryCount; i++) {
                files.Add(new DatBTreeFile() {
                    Date = i,
                    Flags = 0,
                    Iteration = i,
                    Size = (uint)i * 2,
                    Id = (uint)i + 1
                });
            }

            files.Shuffle();

            foreach (var file in files) {
                tree.Insert(file);
            }

            for (var i = 0; i < entryCount; i++) {
                var result = tree.TryGetFile((uint)i + 1, out var retrievedFile);

                Assert.IsTrue(result, $"Result {i + 1} was false");
                Assert.IsNotNull(retrievedFile);
                Assert.AreEqual(i, retrievedFile.Date);
                Assert.AreEqual(0u, retrievedFile.Flags);
                Assert.AreEqual((uint)i + 1, retrievedFile.Id);
                Assert.AreEqual(i, retrievedFile.Iteration);
                Assert.AreEqual((uint)i * 2, retrievedFile.Size);
            }

            tree.Dispose();

            File.Delete(datFilePath);
        }

        [TestMethod]
        public void CanAddFileEntryWithNoExistingRoot() {
            var file = Path.GetTempFileName();
            var allocator = new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = file,
                AccessType = Options.DatAccessType.ReadWrite
            });

            allocator.InitNew(DatDatabaseType.Portal, 0);

            var tree = new DatBTreeReaderWriter(allocator);

            Assert.AreEqual(0, allocator.Header.RootBlock);

            var fileEntry = new DatBTreeFile() {
                Date = 12345,
                Flags = 0,
                Iteration = 1,
                Size = 56789,
                Id = 0x12341234
            };

            var existing = tree.Insert(fileEntry);

            Assert.IsNull(existing);

            var result = tree.TryGetFile(0x12341234, out var retrievedFile);

            Assert.AreNotEqual(0, allocator.Header.RootBlock);

            Assert.IsTrue(result);
            Assert.IsNotNull(retrievedFile);
            Assert.AreEqual(12345, retrievedFile.Date);
            Assert.AreEqual(0u, retrievedFile.Flags);
            Assert.AreEqual(0x12341234u, retrievedFile.Id);
            Assert.AreEqual(1, retrievedFile.Iteration);
            Assert.AreEqual(56789u, retrievedFile.Size);

            tree.Dispose();

            File.Delete(file);
        }

        #region EOR Tests
        [TestMethod]
        [TestCategory("EOR")]
        [CombinatorialData]
        public void CanReadEORRootNodes(
            [DataValues(EORDBType.Portal, EORDBType.Cell, EORDBType.Language, EORDBType.HighRes)] EORDBType dbType
            ) {

            using var tree = new DatBTreeReaderWriter(new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = EORCommonData.GetDatPath(dbType)
            }));

            Assert.IsNotNull(tree.Root);
            Assert.IsFalse(tree.Root.IsLeaf);

            EORCommonData.AssertGoodRootNode(tree);
        }

        [TestMethod]
        [TestCategory("EOR")]
        [CombinatorialData]
        public void HasKnownEORPortalFiles(
            [DataValues(0x010022A8u, 0x06004A07u, 0x08001947u, 0x0A0005ACu, 0x33000421u)] uint fileId
            ) {
            using var tree = new DatBTreeReaderWriter(new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = EORCommonData.GetDatPath(EORDBType.Portal)
            }));

            var result = tree.TryGetFile(fileId, out var file);

            Assert.IsTrue(result);
            Assert.IsNotNull(file);
            Assert.AreEqual(fileId, file.Id);
        }

        [TestMethod]
        [TestCategory("EOR")]
        [CombinatorialData]
        public void HasKnownEORCellFiles(
            [DataValues(0x040FFFFFu, 0xFEFBFFFFu, 0x0001FFFEu, 0x9682FFFEu, 0x00010100u, 0x5644012Bu)] uint fileId
            ) {
            using var tree = new DatBTreeReaderWriter(new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = EORCommonData.GetDatPath(EORDBType.Cell)
            }));

            var result = tree.TryGetFile(fileId, out var file);

            Assert.IsTrue(result);
            Assert.IsNotNull(file);
            Assert.AreEqual(fileId, file.Id);
        }

        [TestMethod]
        [TestCategory("EOR")]
        [CombinatorialData]
        public void DoesntFindInvalidPortalEntries(
            [DataValues(0x06001C4Cu, 0x04001737u, 0x0A000999u)] uint fileId
            ) {
            using var tree = new DatBTreeReaderWriter(new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = EORCommonData.GetDatPath(EORDBType.Portal)
            }));

            var result = tree.TryGetFile(fileId, out var file);

            Assert.IsFalse(result);
            Assert.IsNull(file);
        }

        [TestMethod]
        [TestCategory("EOR")]
        [CombinatorialData]
        public void DoesntFindInvalidCellEntries(
            [DataValues(0xFFFFFFFFu, 0xFCFCFFFEu, 0x00010088u)] uint fileId
            ) {
            using var tree = new DatBTreeReaderWriter(new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = EORCommonData.GetDatPath(EORDBType.Cell)
            }));

            var result = tree.TryGetFile(fileId, out var file);

            Assert.IsFalse(result);
            Assert.IsNull(file);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadFullCellFileEntry() {
            using var tree = new DatBTreeReaderWriter(new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = EORCommonData.GetDatPath(EORDBType.Cell)
            }));

            var result = tree.TryGetFile(0xFB21FFFF, out var file);

            Assert.IsTrue(result);
            Assert.IsNotNull(file);
            Assert.AreEqual(0xFB21FFFFu, file.Id);
            Assert.AreEqual(0xF18E000, file.Offset);
            Assert.AreEqual(1117236872, file.Date);
            Assert.AreEqual(252u, file.Size);
        }
        #endregion
    }
}
