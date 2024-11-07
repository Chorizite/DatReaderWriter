using DatReaderWriter.Options;
using DatReaderWriter.Tests.Lib;
using System;
using System.Text;
using System.Threading.Tasks;
using DatReaderWriter;
using DatReaderWriter.Enums;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Lib.IO.BlockAllocators;

namespace DatReaderWriter.Tests.IO.BlockAllocators {
    public enum BlockAllocatorType {
        MemoryMapped,
        Stream
    }

    [TestClass]
    public class BlockAllocatorTests {
        private static Random _rnd = new Random();

        public static IDatBlockAllocator GetBlockAllocator(BlockAllocatorType type, string filename) {
            switch (type) {
                case BlockAllocatorType.MemoryMapped:
                    return new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
                        FilePath = filename,
                        AccessType = DatAccessType.ReadWrite
                    });
                case BlockAllocatorType.Stream:
                    return new StreamBlockAllocator(new DatDatabaseOptions() {
                        FilePath = filename,
                        AccessType = DatAccessType.ReadWrite
                    });
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        [TestMethod]
        [CombinatorialData]
        public void CanCreateNewEmptyDatAndLoadIt(
            [DataValues(256, 1024)] int blockSize,
            [DataValues(1, 2, 1_000, 10_000)] int numBlocksToAllocate,
            [DataValues(BlockAllocatorType.MemoryMapped, BlockAllocatorType.Stream)] BlockAllocatorType allocatorType
            ) {
            var file = Path.GetTempFileName();
            var allocator = GetBlockAllocator(allocatorType, file);

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatFileType.Portal, 0, blockSize, numBlocksToAllocate);

            var expectedFirstBlockOffset = (int)Math.Ceiling((double)DatHeader.SIZE / blockSize) * blockSize;
            var expectedLastBlockOffset = expectedFirstBlockOffset + ((numBlocksToAllocate - 1) * blockSize);

            Assert.IsTrue(allocator.HasHeaderData);
            Assert.AreEqual(numBlocksToAllocate, allocator.Header.FreeBlockCount);
            Assert.AreEqual(expectedFirstBlockOffset, allocator.Header.FirstFreeBlock);
            Assert.AreEqual(expectedLastBlockOffset, allocator.Header.LastFreeBlock);

            allocator.Dispose();

            var actualFileSize = new FileInfo(file).Length;

            allocator = GetBlockAllocator(allocatorType, file);
            Assert.IsTrue(allocator.HasHeaderData);
            Assert.AreEqual(actualFileSize, allocator.Header.FileSize);
            Assert.AreEqual(numBlocksToAllocate, allocator.Header.FreeBlockCount);
            Assert.AreEqual(expectedFirstBlockOffset, allocator.Header.FirstFreeBlock);
            Assert.AreEqual(expectedLastBlockOffset, allocator.Header.LastFreeBlock);

            allocator.Dispose();

            File.Delete(file);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanCreateNewDatAndSetVersion([DataValues(256, 1024)] int blockSize,
            [DataValues(BlockAllocatorType.MemoryMapped, BlockAllocatorType.Stream)] BlockAllocatorType allocatorType) {
            var file = Path.GetTempFileName();
            var allocator = GetBlockAllocator(allocatorType, file);

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatFileType.Portal, 0, blockSize, 1);

            var versionGuid = Guid.NewGuid();
            allocator.SetVersion("Testing", 123, 456, versionGuid, 789u);

            Assert.AreEqual("Testing", allocator.Header.Version);
            Assert.AreEqual(123, allocator.Header.EngineVersion);
            Assert.AreEqual(456, allocator.Header.GameVersion);
            Assert.AreEqual(versionGuid, allocator.Header.MajorVersion);
            Assert.AreEqual(789u, allocator.Header.MinorVersion);

            allocator.Dispose();
            
            allocator = GetBlockAllocator(allocatorType, file);

            Assert.AreEqual("Testing", allocator.Header.Version);
            Assert.AreEqual(123, allocator.Header.EngineVersion);
            Assert.AreEqual(456, allocator.Header.GameVersion);
            Assert.AreEqual(versionGuid, allocator.Header.MajorVersion);
            Assert.AreEqual(789u, allocator.Header.MinorVersion);

            allocator.Dispose();

            File.Delete(file);
        }

        [TestMethod]
        [CombinatorialData]
        public void HeaderHasProperFileSizeAfterInit(
            [DataValues(256, 1024)] int blockSize,
            [DataValues(0, 1, 100, 1000, 1234)] int numBlocks,
            [DataValues(BlockAllocatorType.MemoryMapped, BlockAllocatorType.Stream)] BlockAllocatorType allocatorType
            ) {
            var file = Path.GetTempFileName();
            var allocator = GetBlockAllocator(allocatorType, file);

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatFileType.Portal, 0, blockSize, numBlocks);

            Assert.AreEqual((int)Math.Ceiling(((double)DatHeader.SIZE / blockSize) + numBlocks) * blockSize, allocator.Header.FileSize);

            allocator.Dispose();

            File.Delete(file);
        }

        [TestMethod]
        [CombinatorialData]
        public void HeaderHasProperBlockIndicesAfterInit(
            [DataValues(256, 1024)] int blockSize,
            [DataValues(0, 1, 100, 1000, 1234)] int numBlocks,
            [DataValues(BlockAllocatorType.MemoryMapped, BlockAllocatorType.Stream)] BlockAllocatorType allocatorType
            ) {
            var file = Path.GetTempFileName();
            var allocator = GetBlockAllocator(allocatorType, file);

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatFileType.Portal, 0, blockSize, numBlocks);
            var headerBlockCount = (int)Math.Ceiling(((double)DatHeader.SIZE / blockSize));

            Assert.AreEqual((headerBlockCount + numBlocks) * blockSize, allocator.Header.FileSize);
            if (numBlocks > 0) {
                Assert.AreEqual(headerBlockCount * blockSize, allocator.Header.FirstFreeBlock);
                Assert.AreEqual((headerBlockCount + numBlocks - 1) * blockSize, allocator.Header.LastFreeBlock);
            }
            else {
                Assert.AreEqual(0, allocator.Header.FirstFreeBlock);
                Assert.AreEqual(0, allocator.Header.LastFreeBlock);
            }
            Assert.AreEqual(numBlocks, allocator.Header.FreeBlockCount);

            allocator.Dispose();

            File.Delete(file);
        }

        [TestMethod]
        [CombinatorialData]
        public void HeaderHasProperFileSizeAfterAllocations(
            [DataValues(256, 1024)] int blockSize,
            [DataValues(0, 1, 100, 1000, 1234)] int numBlocks,
            [DataValues(BlockAllocatorType.MemoryMapped, BlockAllocatorType.Stream)] BlockAllocatorType allocatorType
            ) {
            var file = Path.GetTempFileName();
            var allocator = GetBlockAllocator(allocatorType, file);

            Assert.IsFalse(allocator.HasHeaderData);

            allocator.InitNew(DatFileType.Portal, 0, blockSize, numBlocks);

            Assert.AreEqual((int)Math.Ceiling(((double)DatHeader.SIZE / blockSize) + numBlocks) * blockSize, allocator.Header.FileSize);

            allocator.AllocateEmptyBlocks(100);
            numBlocks += 100;

            Assert.AreEqual((int)Math.Ceiling(((double)DatHeader.SIZE / blockSize) + numBlocks) * blockSize, allocator.Header.FileSize);

            allocator.Dispose();

            File.Delete(file);
        }

        [TestMethod]
        [CombinatorialData]
        public void HeaderHasProperBlockIndicesAfterAllocations(
            [DataValues(256, 1024)] int blockSize,
            [DataValues(0, 1, 100, 1000, 1234)] int numBlocks,
            [DataValues(BlockAllocatorType.MemoryMapped, BlockAllocatorType.Stream)] BlockAllocatorType allocatorType
            ) {
            var file = Path.GetTempFileName();
            var allocator = GetBlockAllocator(allocatorType, file);

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatFileType.Portal, 0, blockSize, 0);
            var headerBlockCount = (int)Math.Ceiling(((double)DatHeader.SIZE / blockSize));

            allocator.AllocateEmptyBlocks(numBlocks);

            Assert.AreEqual((headerBlockCount + numBlocks) * blockSize, allocator.Header.FileSize);
            if (numBlocks > 0) {
                Assert.AreEqual(headerBlockCount * blockSize, allocator.Header.FirstFreeBlock);
                Assert.AreEqual((headerBlockCount + numBlocks - 1) * blockSize, allocator.Header.LastFreeBlock);
            }
            else {
                Assert.AreEqual(0, allocator.Header.FirstFreeBlock);
                Assert.AreEqual(0, allocator.Header.LastFreeBlock);
            }
            Assert.AreEqual(numBlocks, allocator.Header.FreeBlockCount);

            allocator.Dispose();

            File.Delete(file);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanReadWriteRawBytesToDat([DataValues(256, 1024)] int blockSize,
            [DataValues(BlockAllocatorType.MemoryMapped, BlockAllocatorType.Stream)] BlockAllocatorType allocatorType) {
            var file = Path.GetTempFileName();
            var allocator = GetBlockAllocator(allocatorType, file);

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatFileType.Portal, 0, blockSize, 1);

            var bytes = Encoding.ASCII.GetBytes("Hello World");
            allocator.WriteBytes(bytes, 0, bytes.Length);

            var readBuffer = new byte[bytes.Length];
            allocator.ReadBytes(readBuffer, 0, 0, readBuffer.Length);
            CollectionAssert.AreEqual(bytes, readBuffer);

            allocator.Dispose();

            allocator = new StreamBlockAllocator(new DatDatabaseOptions() {
                FilePath = file,
                AccessType = DatAccessType.Read
            });

            allocator.ReadBytes(readBuffer, 0, 0, readBuffer.Length);
            CollectionAssert.AreEqual(bytes, readBuffer);

            allocator.Dispose();

            File.Delete(file);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanAllocateEmptyBlocks(
            [DataValues(256, 1024)] int blockSize,
            [DataValues(1, 2, 1_000, 10_000)] int numBlocksToAllocate,
            [DataValues(BlockAllocatorType.MemoryMapped, BlockAllocatorType.Stream)] BlockAllocatorType allocatorType
            ) {
            var file = Path.GetTempFileName();
            var allocator = GetBlockAllocator(allocatorType, file);

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatFileType.Portal, 0, blockSize, 0);

            Assert.AreEqual(0, allocator.Header.FreeBlockCount);

            allocator.AllocateEmptyBlocks(numBlocksToAllocate);

            Assert.AreEqual(numBlocksToAllocate, allocator.Header.FreeBlockCount);

            allocator.Dispose();

            allocator = new StreamBlockAllocator(new DatDatabaseOptions() {
                FilePath = file,
                AccessType = DatAccessType.Read
            });

            Assert.AreEqual(numBlocksToAllocate, allocator.Header.FreeBlockCount);

            allocator.Dispose();

            File.Delete(file);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanWriteToSingleUnusedDatBlock([DataValues(256, 1024)] int blockSize,
            [DataValues(BlockAllocatorType.MemoryMapped, BlockAllocatorType.Stream)] BlockAllocatorType allocatorType) {
            var file = Path.GetTempFileName();
            var allocator = GetBlockAllocator(allocatorType, file);

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatFileType.Portal, 0, blockSize, 10);

            var fileBytes = new byte[blockSize - 4];
            _rnd.NextBytes(fileBytes);

            var blockOffset = allocator.WriteBlock(fileBytes, fileBytes.Length);

            var readBuffer = new byte[fileBytes.Length];
            allocator.ReadBlock(readBuffer, blockOffset);
            CollectionAssert.AreEqual(fileBytes, readBuffer);

            allocator.Dispose();

            allocator = new StreamBlockAllocator(new DatDatabaseOptions() {
                FilePath = file,
                AccessType = DatAccessType.Read
            });

            allocator.ReadBlock(readBuffer, blockOffset);
            CollectionAssert.AreEqual(fileBytes, readBuffer);

            allocator.Dispose();

            File.Delete(file);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanWriteToMultipleUnusedDatBlocks([DataValues(256, 1024)] int blockSize,
            [DataValues(BlockAllocatorType.MemoryMapped, BlockAllocatorType.Stream)] BlockAllocatorType allocatorType) {
            var file = Path.GetTempFileName();
            var allocator = GetBlockAllocator(allocatorType, file);

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatFileType.Portal, 0, blockSize, 10);

            var fileBytes = new byte[blockSize * 5];
            _rnd.NextBytes(fileBytes);

            var blockOffset = allocator.WriteBlock(fileBytes, fileBytes.Length);

            var readBuffer = new byte[fileBytes.Length];
            allocator.ReadBlock(readBuffer, blockOffset);

            CollectionAssert.AreEqual(fileBytes, readBuffer);

            allocator.Dispose();

            allocator = GetBlockAllocator(allocatorType, file);

            allocator.ReadBlock(readBuffer, blockOffset);
            CollectionAssert.AreEqual(fileBytes, readBuffer);

            allocator.Dispose();

            File.Delete(file);
        }

        [TestMethod]
        [CombinatorialData]
        public void UpdatingBlockContentsReusesAllocatedBlocks([DataValues(256, 1024)] int blockSize,
            [DataValues(BlockAllocatorType.MemoryMapped, BlockAllocatorType.Stream)] BlockAllocatorType allocatorType) {
            var file = Path.GetTempFileName();
            var allocator = GetBlockAllocator(allocatorType, file);

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatFileType.Portal, 0, blockSize, 10);

            var fileBytes = new byte[(blockSize - 4) * 4];
            _rnd.NextBytes(fileBytes);

            var blockOffset = allocator.WriteBlock(fileBytes, fileBytes.Length);
            var freeBlocks = allocator.Header.FreeBlockCount;

            // rewriting the block should use the same allocated blocks
            var newOffset = allocator.WriteBlock(fileBytes, fileBytes.Length, blockOffset);

            Assert.AreEqual(blockOffset, newOffset);
            Assert.AreEqual(freeBlocks, allocator.Header.FreeBlockCount);

            allocator.Dispose();

            File.Delete(file);
        }

        [TestMethod]
        [CombinatorialData]
        public void HeaderIsUpdatedAfterWritingBlocks(
            [DataValues(256, 1024)] int blockSize,
            [DataValues(1, 2, 1_000, 10_000)] int numBlocks,
            [DataValues(BlockAllocatorType.MemoryMapped, BlockAllocatorType.Stream)] BlockAllocatorType allocatorType
            ) {
            var file = Path.GetTempFileName();
            var allocator = GetBlockAllocator(allocatorType, file);

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatFileType.Portal, 0, blockSize, 20_000);

            var fileBytes = new byte[(blockSize - 4) * numBlocks];

            var blockOffset = allocator.WriteBlock(fileBytes, fileBytes.Length);

            Assert.AreEqual(20_000 - numBlocks, allocator.Header.FreeBlockCount);

            allocator.Dispose();

            allocator = GetBlockAllocator(allocatorType, file);

            var headerBlockCount = (int)Math.Ceiling(((double)DatHeader.SIZE / blockSize));

            Assert.AreEqual((headerBlockCount + 20_000) * blockSize, allocator.Header.FileSize);
            Assert.AreEqual((headerBlockCount + numBlocks) * blockSize, allocator.Header.FirstFreeBlock);
            Assert.AreEqual((headerBlockCount + 20_000 - 1) * blockSize, allocator.Header.LastFreeBlock);

            allocator.Dispose();

            File.Delete(file);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanExpandDatToWriteBlocks([DataValues(256, 1024)] int blockSize,
            [DataValues(BlockAllocatorType.MemoryMapped, BlockAllocatorType.Stream)] BlockAllocatorType allocatorType) {
            var file = Path.GetTempFileName();
            var allocator = GetBlockAllocator(allocatorType, file);

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatFileType.Portal, 0, blockSize, 1);

            var fileBytes = new byte[blockSize * 5];
            _rnd.NextBytes(fileBytes);

            var blockOffset = allocator.WriteBlock(fileBytes, fileBytes.Length);

            var readBuffer = new byte[fileBytes.Length];
            allocator.ReadBlock(readBuffer, blockOffset);

            CollectionAssert.AreEqual(fileBytes, readBuffer);

            allocator.Dispose();

            allocator = GetBlockAllocator(allocatorType, file);

            allocator.ReadBlock(readBuffer, blockOffset);
            CollectionAssert.AreEqual(fileBytes, readBuffer);

            allocator.Dispose();

            File.Delete(file);
        }

        [TestMethod]
        [CombinatorialData]
        public void UsesCorrectNumberOfBlocksWhenWritingFiles(
            [DataValues(256, 1024)] int blockSize,
            [DataValues(252, 1020, 200, 800, 1000, 10_000, 10_000_000)] int fileSize,
            [DataValues(BlockAllocatorType.MemoryMapped, BlockAllocatorType.Stream)] BlockAllocatorType allocatorType
            ) {
            var file = Path.GetTempFileName();
            var allocator = GetBlockAllocator(allocatorType, file);

            var allocatedBlockCount = (fileSize + (blockSize * 100)) / (blockSize - 4);

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatFileType.Portal, 0, blockSize, allocatedBlockCount);

            Assert.AreEqual(allocatedBlockCount, allocator.Header.FreeBlockCount);

            var startingFileSize = allocator.Header.FileSize;

            var fileBytes = new byte[fileSize];
            allocator.WriteBlock(fileBytes, fileBytes.Length);

            var expectedBlockUsage = Math.Ceiling(fileSize / (float)(blockSize - 4));

            Assert.AreEqual(startingFileSize, allocator.Header.FileSize);
            Assert.AreEqual(allocatedBlockCount - expectedBlockUsage, allocator.Header.FreeBlockCount);

            allocator.Dispose();

            File.Delete(file);
        }

        [TestMethod]
        [CombinatorialData]
        public void ReservingBlocksAllocatesBlocksAndUpdatesHeader(
            [DataValues(256, 1024)] int blockSize,
            [DataValues(1, 10, 100, 1000)] int blocksToReserve,
            [DataValues(BlockAllocatorType.MemoryMapped, BlockAllocatorType.Stream)] BlockAllocatorType allocatorType
            ) {
            var file = Path.GetTempFileName();
            var allocator = GetBlockAllocator(allocatorType, file);

            var allocatedBlockCount = 50;

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatFileType.Portal, 0, blockSize, allocatedBlockCount);

            Assert.AreEqual(allocatedBlockCount, allocator.Header.FreeBlockCount);

            var firstFree = allocator.Header.FirstFreeBlock;
            for (var i = 0; i < blocksToReserve; i++) {
                allocator.ReserveBlock();

                var exp = firstFree + ((i + 1) * allocator.Header.BlockSize);
                var cFree = allocator.Header.FirstFreeBlock;
                Assert.AreEqual(exp, cFree, $"Expected {exp:X8} to be same as free: {cFree:X8}");
            }

            allocator.Dispose();

            File.Delete(file);
        }
    }
}
