using DatReaderWriter.Options;
using DatReaderWriter.Tests.Lib;
using System;
using System.Buffers.Binary;
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
        public void AllocateEmptyBlocksWritesInBandPointers(
            [DataValues(256, 1024)] int blockSize,
            [DataValues(BlockAllocatorType.MemoryMapped, BlockAllocatorType.Stream)] BlockAllocatorType allocatorType
            ) {
            var file = Path.GetTempFileName();
            var allocator = GetBlockAllocator(allocatorType, file);

            allocator.InitNew(DatFileType.Portal, 0, blockSize, 3);

            var firstBlock = allocator.Header.FirstFreeBlock;
            var ptrBuf = new byte[4];

            // Block 0: should point to block 1 with high free-marker bit
            allocator.ReadBytes(ptrBuf, 0, firstBlock, 4);
            var ptr0 = BinaryPrimitives.ReadInt32LittleEndian(ptrBuf);
            Assert.AreEqual((firstBlock + blockSize) | unchecked((int)0x80000000), ptr0,
                "Block 0 in-band pointer should point to block 1 with high bit set");

            // Block 1: should point to block 2 with high bit
            allocator.ReadBytes(ptrBuf, 0, firstBlock + blockSize, 4);
            var ptr1 = BinaryPrimitives.ReadInt32LittleEndian(ptrBuf);
            Assert.AreEqual((firstBlock + 2 * blockSize) | unchecked((int)0x80000000), ptr1,
                "Block 1 in-band pointer should point to block 2 with high bit set");

            // Block 2 (last): end-of-chain marker
            allocator.ReadBytes(ptrBuf, 0, firstBlock + 2 * blockSize, 4);
            var ptr2 = BinaryPrimitives.ReadInt32LittleEndian(ptrBuf);
            Assert.AreEqual(unchecked((int)0x80000000), ptr2,
                "Last block in-band pointer should be end-of-chain marker 0x80000000");

            allocator.Dispose();
            File.Delete(file);
        }

        [TestMethod]
        [CombinatorialData]
        public void AllocateEmptyBlocksChainsToExistingFreeList(
            [DataValues(256, 1024)] int blockSize,
            [DataValues(BlockAllocatorType.MemoryMapped, BlockAllocatorType.Stream)] BlockAllocatorType allocatorType
            ) {
            var file = Path.GetTempFileName();
            var allocator = GetBlockAllocator(allocatorType, file);

            // Start with 2 blocks, then add 2 more — the old tail must chain into the new head.
            allocator.InitNew(DatFileType.Portal, 0, blockSize, 2);
            var block0 = allocator.Header.FirstFreeBlock;
            var block1 = block0 + blockSize;

            allocator.AllocateEmptyBlocks(2);
            var block2 = block1 + blockSize;
            var block3 = block2 + blockSize;

            Assert.AreEqual(4, allocator.Header.FreeBlockCount);
            Assert.AreEqual(block0, allocator.Header.FirstFreeBlock);
            Assert.AreEqual(block3, allocator.Header.LastFreeBlock);

            // Old tail (block1) should now point to block2
            var ptrBuf = new byte[4];
            allocator.ReadBytes(ptrBuf, 0, block1, 4);
            var ptr1 = BinaryPrimitives.ReadInt32LittleEndian(ptrBuf);
            Assert.AreEqual(block2 | unchecked((int)0x80000000), ptr1,
                "Old last block should chain to new chain head with high bit");

            // New tail (block3) should have end-of-chain marker
            allocator.ReadBytes(ptrBuf, 0, block3, 4);
            var ptr3 = BinaryPrimitives.ReadInt32LittleEndian(ptrBuf);
            Assert.AreEqual(unchecked((int)0x80000000), ptr3,
                "New last block should be end-of-chain marker");

            allocator.Dispose();
            File.Delete(file);
        }

        [TestMethod]
        [CombinatorialData]
        public void ReserveBlockFollowsFragmentedLinkedList(
            [DataValues(256, 1024)] int blockSize,
            [DataValues(BlockAllocatorType.MemoryMapped, BlockAllocatorType.Stream)] BlockAllocatorType allocatorType
            ) {
            var file = Path.GetTempFileName();
            var allocator = GetBlockAllocator(allocatorType, file);

            // Allocate 8 contiguous blocks to have enough file space.
            allocator.InitNew(DatFileType.Portal, 0, blockSize, 8);

            var blockA = allocator.Header.FirstFreeBlock;
            var blockB = blockA + 3 * blockSize; // non-contiguous gap
            var blockC = blockA + 7 * blockSize; // non-contiguous gap

            // Overwrite the in-band pointers to form a fragmented chain A → B → C → end.
            var ptrBuf = new byte[4];
            BinaryPrimitives.WriteInt32LittleEndian(ptrBuf, blockB | unchecked((int)0x80000000));
            allocator.WriteBytes(ptrBuf, blockA, 4);

            BinaryPrimitives.WriteInt32LittleEndian(ptrBuf, blockC | unchecked((int)0x80000000));
            allocator.WriteBytes(ptrBuf, blockB, 4);

            BinaryPrimitives.WriteInt32LittleEndian(ptrBuf, unchecked((int)0x80000000));
            allocator.WriteBytes(ptrBuf, blockC, 4);

            // Patch the in-memory header to reflect the fragmented free list.
            allocator.Header.FirstFreeBlock = blockA;
            allocator.Header.LastFreeBlock = blockC;
            allocator.Header.FreeBlockCount = 3;

            // Reserve 3 blocks — must follow the linked list, not a stride.
            var r1 = allocator.ReserveBlock();
            var r2 = allocator.ReserveBlock();
            var r3 = allocator.ReserveBlock();

            Assert.AreEqual(blockA, r1, "First reservation should return block A");
            Assert.AreEqual(blockB, r2, "Second reservation should return block B");
            Assert.AreEqual(blockC, r3, "Third reservation should return block C");
            Assert.AreEqual(0, allocator.Header.FreeBlockCount, "Free block count should be 0 after consuming all");
            Assert.AreEqual(0, allocator.Header.FirstFreeBlock, "FirstFreeBlock should be 0 after consuming all");

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
            var reservedOffsets = new List<int>(blocksToReserve);
            for (var i = 0; i < blocksToReserve; i++) {
                reservedOffsets.Add(allocator.ReserveBlock());
            }

            // Every returned offset must be unique — no block handed out twice.
            Assert.AreEqual(blocksToReserve, reservedOffsets.Distinct().Count(),
                "ReserveBlock must never return the same offset twice");

            // With a sequential free-list chain, offsets are issued in stride order.
            for (var i = 0; i < blocksToReserve; i++) {
                var exp = firstFree + i * blockSize;
                Assert.AreEqual(exp, reservedOffsets[i],
                    $"Expected block {i} offset {exp:X8} but got {reservedOffsets[i]:X8}");
            }

            allocator.Dispose();

            File.Delete(file);
        }
    }
}
