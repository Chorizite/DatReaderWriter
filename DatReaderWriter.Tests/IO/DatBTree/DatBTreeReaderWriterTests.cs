using DatReaderWriter;
using DatReaderWriter.Enums;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Options;
using DatReaderWriter.Lib.IO.BlockAllocators;
using DatReaderWriter.Lib.IO.DatBTree;
using DatReaderWriter.Tests.Lib;
using Moq;
using System.Data;

namespace DatReaderWriter.Tests.IO.DatBTree {
    [TestClass]
    public class DatBTreeReaderWriterTests {
        private DatBTreeReaderWriter _btree;
        private string _datFilePath;
        private MemoryMappedBlockAllocator _allocator;

        [TestInitialize]
        public void SetUp() {
            _datFilePath = Path.GetTempFileName();
            _allocator = new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
                FilePath = _datFilePath,
                AccessType = DatAccessType.ReadWrite
            });

            _allocator.InitNew(DatFileType.Portal, 0);

            _btree = new DatBTreeReaderWriter(_allocator);
        }

        [TestCleanup]
        public void TearDown() {
            _btree.Dispose();
            _allocator.Dispose();
            File.Delete(_datFilePath);
        }



        [TestMethod]
        public void Insert_ShouldAddRootNode_WhenTreeIsEmpty() {
            // Arrange
            var file = new DatBTreeFile { Id = 1 };

            // Act
            _btree.Insert(file);

            // Assert
            Assert.IsNotNull(_btree.Root);
            Assert.AreEqual(1, _btree.Root.Files.Count);
            Assert.AreEqual(file, _btree.Root.Files[0]);
        }

        [TestMethod]
        public void Insert_ShouldSplitRoot_WhenRootIsFull() {
            // Arrange
            for (int i = 1; i <= _btree.MaxItems + 1; i++) {
                _btree.Insert(new DatBTreeFile { Id = (uint)i });
            }

            // Act
            // Check if root was split
            Assert.IsNotNull(_btree.Root);
            Assert.AreEqual(1, _btree.Root.Files.Count);
            Assert.IsTrue(_btree.Root.Branches.Count > 0);
        }

        [TestMethod]
        public void TryGetFile_ShouldReturnTrue_WhenFileExists() {
            // Arrange
            var file = new DatBTreeFile { Id = 100 };
            _btree.Insert(file);

            // Act
            var result = _btree.TryGetFile(file.Id, out var retrievedFile);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TryGetFile_ShouldReturnFalse_WhenFileDoesNotExist() {
            // Act
            var result = _btree.TryGetFile(999, out var retrievedFile);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(retrievedFile);
        }

        [TestMethod]
        public void Delete_ShouldRemoveFile_WhenFileExists() {
            // Arrange
            var file = new DatBTreeFile { Id = 50 };
            _btree.Insert(file);

            // Act
            var deleteResult = _btree.TryDelete(file.Id, out var deletedFile);

            // Assert
            Assert.IsTrue(deleteResult);

            // Verify it’s no longer in the tree
            var findResult = _btree.TryGetFile(file.Id, out var _);
            Assert.IsFalse(findResult);
        }

        [TestMethod]
        public void Delete_ShouldReturnFalse_WhenFileDoesNotExist() {
            // Act
            var deleteResult = _btree.TryDelete(1000, out var deletedFile);

            // Assert
            Assert.IsFalse(deleteResult);
            Assert.IsNull(deletedFile);
        }

        [TestMethod]
        public void Insert_ShouldHandleMultipleInsertionsCorrectly() {
            // Arrange
            var files = new[] {
                new DatBTreeFile { Id = 1 },
                new DatBTreeFile { Id = 50 },
                new DatBTreeFile { Id = 100 },
                new DatBTreeFile { Id = 75 }
            };

            foreach (var file in files) {
                _btree.Insert(file);
            }

            // Act & Assert - Check that each file is accessible
            foreach (var file in files) {
                var result = _btree.TryGetFile(file.Id, out var retrievedFile);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void Insert_ShouldSplitNodesRecursively_WhenNecessary() {
            // Arrange - Insert enough items to trigger multiple splits
            for (int i = 1; i <= (_btree.Degree * 3); i++) {
                _btree.Insert(new DatBTreeFile { Id = (uint)i });
            }

            // Act - Check if root and branches exist
            Assert.IsNotNull(_btree.Root);
            Assert.IsTrue(_btree.Root.Files.Count > 1);
            Assert.IsTrue(_btree.Root.Branches.Count > 0);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanInsertFileEntries([DataValues(1, 10, 100, 1000)] int entryCount) {
            var datFilePath = Path.GetTempFileName();
            var allocator = new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
                FilePath = datFilePath,
                AccessType = DatAccessType.ReadWrite
            });

            allocator.InitNew(DatFileType.Portal, 0);

            var tree = new DatBTreeReaderWriter(allocator);

            Assert.AreEqual(0, allocator.Header.RootBlock);
            var now = DateTime.UtcNow;

            var files = new List<DatBTreeFile>();
            for (var i = 0; i < entryCount; i++) {
                files.Add(new DatBTreeFile() {
                    Date = now,
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
                //Assert.AreEqual(now, retrievedFile.Date);
                Assert.AreEqual(0x20000u, retrievedFile.Flags);
                Assert.AreEqual((uint)(i + 1) * 3, retrievedFile.Id);
                Assert.AreEqual(i, retrievedFile.Iteration);
                Assert.AreEqual((uint)i * 2, retrievedFile.Size);
            }

            tree.Dispose();
            allocator.Dispose();

            File.Delete(datFilePath);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanIterateRangedFileEntries([DataValues(1, 10, 60, 61, 62, 100, 1000)] int entryCount) {
            var datFilePath = Path.GetTempFileName();
            var allocator = new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
                FilePath = datFilePath,
                AccessType = DatAccessType.ReadWrite
            });

            allocator.InitNew(DatFileType.Portal, 0);

            var tree = new DatBTreeReaderWriter(allocator);

            Assert.AreEqual(0, allocator.Header.RootBlock);
            var now = DateTime.UtcNow;

            var files = new List<DatBTreeFile>();
            for (var i = 0; i < entryCount; i++) {
                files.Add(new DatBTreeFile() {
                    Id = (uint)(i + 1)
                });
            }

            // insert ids that will be higher than our range request to
            // ensure they are not included in the request
            for (var i = 0; i < entryCount; i++) {
                files.Add(new DatBTreeFile() {
                    Id = (uint)(10 * entryCount) + (uint)(i + 1)
                });
            }

            foreach (var file in files) {
                tree.Insert(file);
            }

            Assert.AreEqual(entryCount * 2, tree.Count());
            Assert.AreEqual(entryCount, tree.GetFilesInRange(1, (uint)entryCount).Count());

            var min = Math.Max((uint)entryCount / 2, entryCount);
            Assert.AreEqual(min, tree.GetFilesInRange(1, (uint)min).Count());

            tree.Dispose();
            allocator.Dispose();

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

            allocator.InitNew(DatFileType.Portal, 0);

            var tree = new DatBTreeReaderWriter(allocator);

            Assert.AreEqual(0, allocator.Header.RootBlock);
            var now = DateTime.UtcNow;
            var files = new List<DatBTreeFile>();
            for (var i = 0; i < entryCount; i++) {
                files.Add(new DatBTreeFile() {
                    Date = now,
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
                //Assert.AreEqual(now, deletedFile.Date);
                Assert.AreEqual(0x20000u, deletedFile.Flags);
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
            allocator.Dispose();

            File.Delete(datFilePath);
        }

        [TestMethod]
        public void CanAddFileEntryWithNoExistingRoot() {
            var file = Path.GetTempFileName();
            var allocator = new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
                FilePath = file,
                AccessType = DatAccessType.ReadWrite
            });

            allocator.InitNew(DatFileType.Portal, 0);

            var tree = new DatBTreeReaderWriter(allocator);

            Assert.AreEqual(0, allocator.Header.RootBlock);
            var now = DateTime.UtcNow;
            var fileEntry = new DatBTreeFile() {
                Date = now,
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
            //Assert.AreEqual(now, retrievedFile.Date);
            Assert.AreEqual(0x20000u, retrievedFile.Flags);
            Assert.AreEqual(0x12341234u, retrievedFile.Id);
            Assert.AreEqual(1, retrievedFile.Iteration);
            Assert.AreEqual(56789u, retrievedFile.Size);

            tree.Dispose();
            allocator.Dispose();

            File.Delete(file);
        }

        #region EOR Tests
        [TestMethod]
        [TestCategory("EOR")]
        [CombinatorialData]
        public void CanReadEORRootNodes(
            [DataValues(EORDBType.Portal, EORDBType.Cell, EORDBType.Local, EORDBType.HighRes)] EORDBType dbType
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
        public void CanReadEORSurfaceTextureRanges() {
            using var tree = new DatBTreeReaderWriter(new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
                FilePath = EORCommonData.GetDatPath(EORDBType.Portal)
            }));
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var files = tree.GetFilesInRange(0x5000000, 0x5FFFFFF);
            Assert.AreEqual(7221, files.Count());
            sw.Stop();

            Console.WriteLine(sw.ElapsedMilliseconds);
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
            Assert.AreEqual(252u, file.Size);
        }

        [TestMethod]
        [TestCategory("EOR")]
        [CombinatorialData]
        public void CanIterateOverEORDatsInSortedFileOrder(
            [DataValues(EORDBType.Portal, EORDBType.Cell, EORDBType.Local, EORDBType.HighRes)] EORDBType dbType
            ) {
            return;

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
            [DataValues(EORDBType.Portal, EORDBType.Cell, EORDBType.Local, EORDBType.HighRes)] EORDBType dbType
            ) {
            return;

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
