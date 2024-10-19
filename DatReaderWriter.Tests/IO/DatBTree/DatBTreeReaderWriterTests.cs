using ACClientLib.DatReaderWriter;
using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.IO;
using ACClientLib.DatReaderWriter.IO.BlockAllocators;
using ACClientLib.DatReaderWriter.IO.DatBTree;
using ACClientLib.DatReaderWriter.Options;
using DatReaderWriter.Tests.Lib;

namespace DatReaderWriter.Tests.IO.DatBTree {
    [TestClass]
    public class DatBTreeReaderWriterTests {
        [TestMethod]
        [CombinatorialData]
        public void CanInsertFileEntries([DataValues(1, 10, 100, 1000)] int entryCount) {
            var datFilePath = Path.GetTempFileName();
            var allocator = new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
                FilePath = datFilePath,
                AccessType = DatAccessType.ReadWrite
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
                    Id = (uint)(i + 1) * 3
                });
            }

            files.Shuffle();

            foreach (var file in files) {
                tree.Insert(file);
            }

            var enumeratedFileIds = new List<uint>();
            foreach (var fileEntry in tree) {
                enumeratedFileIds.Add(fileEntry.Id);
            }

            var sortedInsertedFiles = files.Select(f => f.Id).ToList();
            sortedInsertedFiles.Sort((a, b) => a.CompareTo(b));

            CollectionAssert.AreEqual(sortedInsertedFiles, enumeratedFileIds);

            for (var i = 0; i < entryCount; i++) {
                var result = tree.TryGetFile((uint)(i + 1) * 3, out var retrievedFile);

                Assert.IsTrue(result, $"Result {(i + 1) * 3} was false");
                Assert.IsNotNull(retrievedFile);
                Assert.AreEqual(i, retrievedFile.Date);
                Assert.AreEqual(0u, retrievedFile.Flags);
                Assert.AreEqual((uint)(i + 1) * 3, retrievedFile.Id);
                Assert.AreEqual(i, retrievedFile.Iteration);
                Assert.AreEqual((uint)i * 2, retrievedFile.Size);
            }

            tree.Dispose();

            File.Delete(datFilePath);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanDeleteFileEntries([DataValues(1, 10, 100, 1000)] int entryCount) {
            var datFilePath = Path.GetTempFileName();
            var allocator = new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
                FilePath = datFilePath,
                AccessType = DatAccessType.ReadWrite
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
                    Id = (uint)(i + 1) * 3
                });
            }

            files.Shuffle();

            foreach (var file in files) {
                tree.Insert(file);
            }

            var enumeratedFileIds = new List<uint>();
            foreach (var fileEntry in tree) {
                enumeratedFileIds.Add(fileEntry.Id);
            }

            var sortedInsertedFiles = files.Select(f => f.Id).ToList();
            sortedInsertedFiles.Sort((a, b) => a.CompareTo(b));

            CollectionAssert.AreEqual(sortedInsertedFiles, enumeratedFileIds);

            for (var i = 0; i < entryCount; i++) {
                var id = (uint)(i + 1) * 3;
                var result = tree.TryDelete(id, out var deletedFile);

                Assert.IsTrue(result, $"Result {id} was false");
                Assert.IsNotNull(deletedFile);
                Assert.AreEqual(i, deletedFile.Date);
                Assert.AreEqual(0u, deletedFile.Flags);
                Assert.AreEqual((uint)(i + 1) * 3, deletedFile.Id);
                Assert.AreEqual(i, deletedFile.Iteration);
                Assert.AreEqual((uint)i * 2, deletedFile.Size);

                Assert.IsFalse(tree.HasFile(id));
            }

            var newFileCount = 0;
            foreach (var file in tree) {
                newFileCount++;
            }

            Assert.AreEqual(0, newFileCount);

            tree.Dispose();

            File.Delete(datFilePath);
        }

        [TestMethod]
        public void CanAddFileEntryWithNoExistingRoot() {
            var file = Path.GetTempFileName();
            var allocator = new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
                FilePath = file,
                AccessType = DatAccessType.ReadWrite
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

            using var tree = new DatBTreeReaderWriter(new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
                FilePath = EORCommonData.GetDatPath(dbType)
            }));

            Assert.IsNotNull(tree.Root);
            Assert.IsFalse(tree.Root.IsLeaf);

            EORCommonData.AssertGoodRootNode(tree);
        }

        [TestMethod]
        [TestCategory("EOR")]
        [CombinatorialData]
        public void CanLinqOverEORDats([DataValues(EORDBType.Portal)] EORDBType dbType) {

            using var tree = new DatBTreeReaderWriter(new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
                FilePath = EORCommonData.GetDatPath(dbType)
            }));

            var textureCount = tree.Where(f => f.Id >> 24 == 0x05).Select(f => f.Id).Count();
            Assert.AreEqual(7221, textureCount);
        }

        [TestMethod]
        [TestCategory("EOR")]
        [CombinatorialData]
        public void HasKnownEORPortalFiles(
            [DataValues(0x010022A8u, 0x06004A07u, 0x08001947u, 0x0A0005ACu, 0x33000421u)] uint fileId
            ) {
            using var tree = new DatBTreeReaderWriter(new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
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
            using var tree = new DatBTreeReaderWriter(new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
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
            using var tree = new DatBTreeReaderWriter(new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
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
            using var tree = new DatBTreeReaderWriter(new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
                FilePath = EORCommonData.GetDatPath(EORDBType.Cell)
            }));

            var result = tree.TryGetFile(fileId, out var file);

            Assert.IsFalse(result);
            Assert.IsNull(file);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadFullCellFileEntry() {
            using var tree = new DatBTreeReaderWriter(new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
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

        [TestMethod]
        [TestCategory("EOR")]
        [CombinatorialData]
        public void CanIterateOverEORDatsInSortedFileOrder(
            [DataValues(EORDBType.Portal, EORDBType.Cell, EORDBType.Language, EORDBType.HighRes)] EORDBType dbType
            ) {

            using var tree = new DatBTreeReaderWriter(new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
                FilePath = EORCommonData.GetDatPath(dbType)
            }));

            var fileCount = 0;
            var lastFile = 0u;
            foreach (var fileEntry in tree) {
                Assert.IsTrue(lastFile < fileEntry.Id, $"Current file {fileEntry.Id:X8} should be less than previous file {lastFile:X8}");

                lastFile = fileEntry.Id;
                fileCount++;
            }

            EORCommonData.AssertGoodFileEntryCount(dbType, fileCount);
        }

        [TestMethod]
        [TestCategory("EOR")]
        [CombinatorialData]
        public void CanIterateOverRetailDatsWithoutDoubleEntryReading(
            [DataValues(EORDBType.Portal, EORDBType.Cell, EORDBType.Language, EORDBType.HighRes)] EORDBType dbType
            ) {

            using var tree = new DatBTreeReaderWriter(new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
                FilePath = EORCommonData.GetDatPath(dbType)
            }));

            var knownFiles = new Dictionary<uint, bool>();
            foreach (var fileEntry in tree) {
                Assert.IsFalse(knownFiles.ContainsKey(fileEntry.Id), $"Current file {fileEntry.Id:X8} was already iterated over");

                knownFiles.Add(fileEntry.Id, true);
            }
        }
        #endregion
    }
}
