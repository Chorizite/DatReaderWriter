using ACDatReader.IO;
using ACDatReader.IO.BlockAllocators;
using ACDatReader.Tests.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ACDatReader.Tests.IO.BlockAllocators {
    [TestClass]
    public class MemoryMappedBlockAllocatorTests {
        [TestMethod]
        [CombinatorialData]
        public void CanCreateNewEmptyDatAndLoadIt(
            [DataValues(256, 1024)] int blockSize,
            [DataValues(1, 2, 1_000, 10_000)] int numBlocksToAllocate
            ) {
            var file = Path.GetTempFileName();
            var allocator = new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = file,
                AccessType = Options.DatAccessType.ReadWrite
            });

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatDatabaseType.Portal, 0, blockSize, numBlocksToAllocate);

            var expectedFirstBlockOffset = (int)Math.Ceiling((double)DatHeader.SIZE / blockSize) * blockSize;
            var expectedLastBlockOffset = expectedFirstBlockOffset + ((numBlocksToAllocate - 1) * blockSize);

            Assert.IsTrue(allocator.HasHeaderData);
            Assert.AreEqual(numBlocksToAllocate, allocator.Header.FreeBlockCount);
            Assert.AreEqual(expectedFirstBlockOffset, allocator.Header.FirstFreeBlock);
            Assert.AreEqual(expectedLastBlockOffset, allocator.Header.LastFreeBlock);

            allocator.Dispose();

            var actualFileSize = new FileInfo(file).Length;

            allocator = new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = file,
                AccessType = Options.DatAccessType.Read
            });
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
        public void CanCreateNewDatAndSetVersion([DataValues(256, 1024)] int blockSize) {
            var file = Path.GetTempFileName();
            var allocator = new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = file,
                AccessType = Options.DatAccessType.ReadWrite
            });

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatDatabaseType.Portal, 0, blockSize, 1);

            var versionGuid = Guid.NewGuid();
            allocator.SetVersion("Testing", 123, 456, versionGuid, 789u);

            Assert.AreEqual("Testing", allocator.Header.Version);
            Assert.AreEqual(123, allocator.Header.EngineVersion);
            Assert.AreEqual(456, allocator.Header.GameVersion);
            Assert.AreEqual(versionGuid, allocator.Header.MajorVersion);
            Assert.AreEqual(789u, allocator.Header.MinorVersion);

            allocator.Dispose();
            
            allocator = new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = file,
                AccessType = Options.DatAccessType.Read
            });

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
        public void CanReadWriteRawBytesToDat([DataValues(256, 1024)] int blockSize) {
            var file = Path.GetTempFileName();
            var allocator = new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = file,
                AccessType = Options.DatAccessType.ReadWrite
            });

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatDatabaseType.Portal, 0, blockSize, 1);

            var bytes = Encoding.ASCII.GetBytes("Hello World");
            allocator.WriteBytes(bytes, 0, bytes.Length);

            var readBuffer = new byte[bytes.Length];
            allocator.ReadBytes(readBuffer, 0, readBuffer.Length);
            CollectionAssert.AreEqual(bytes, readBuffer);

            allocator.Dispose();

            allocator = new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = file,
                AccessType = Options.DatAccessType.Read
            });

            allocator.ReadBytes(readBuffer, 0, readBuffer.Length);
            CollectionAssert.AreEqual(bytes, readBuffer);

            allocator.Dispose();

            File.Delete(file);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanWriteToSingleUnusedDatBlock([DataValues(256, 1024)] int blockSize) {
            var file = Path.GetTempFileName();
            var allocator = new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = file,
                AccessType = Options.DatAccessType.ReadWrite
            });

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatDatabaseType.Portal, 0, blockSize, 10);

            var fileBytes = new byte[blockSize - 4];
            Random.Shared.NextBytes(fileBytes.AsSpan());

            var blockOffset = allocator.WriteBlock(fileBytes, fileBytes.Length);

            var readBuffer = new byte[fileBytes.Length];
            allocator.ReadBlock(readBuffer, blockOffset);
            CollectionAssert.AreEqual(fileBytes, readBuffer);

            allocator.Dispose();

            allocator = new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = file,
                AccessType = Options.DatAccessType.Read
            });

            allocator.ReadBlock(readBuffer, blockOffset);
            CollectionAssert.AreEqual(fileBytes, readBuffer);

            allocator.Dispose();

            File.Delete(file);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanWriteToMultipleUnusedDatBlocks([DataValues(256, 1024)] int blockSize) {
            var file = Path.GetTempFileName();
            var allocator = new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = file,
                AccessType = Options.DatAccessType.ReadWrite
            });

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatDatabaseType.Portal, 0, blockSize, 10);

            var fileBytes = new byte[blockSize * 5];
            Random.Shared.NextBytes(fileBytes.AsSpan());

            var blockOffset = allocator.WriteBlock(fileBytes, fileBytes.Length);

            var readBuffer = new byte[fileBytes.Length];
            allocator.ReadBlock(readBuffer, blockOffset);

            CollectionAssert.AreEqual(fileBytes, readBuffer);

            allocator.Dispose();

            allocator = new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = file,
                AccessType = Options.DatAccessType.Read
            });

            allocator.ReadBlock(readBuffer, blockOffset);
            CollectionAssert.AreEqual(fileBytes, readBuffer);

            allocator.Dispose();

            File.Delete(file);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanExpandDatToWriteBlocks([DataValues(256, 1024)] int blockSize) {
            var file = Path.GetTempFileName();
            var allocator = new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = file,
                AccessType = Options.DatAccessType.ReadWrite
            });

            Assert.IsFalse(allocator.HasHeaderData);
            allocator.InitNew(DatDatabaseType.Portal, 0, blockSize, 1);

            var fileBytes = new byte[blockSize * 5];
            Random.Shared.NextBytes(fileBytes.AsSpan());

            var blockOffset = allocator.WriteBlock(fileBytes, fileBytes.Length);

            var readBuffer = new byte[fileBytes.Length];
            allocator.ReadBlock(readBuffer, blockOffset);

            CollectionAssert.AreEqual(fileBytes, readBuffer);

            allocator.Dispose();

            allocator = new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = file,
                AccessType = Options.DatAccessType.Read
            });

            allocator.ReadBlock(readBuffer, blockOffset);
            CollectionAssert.AreEqual(fileBytes, readBuffer);

            allocator.Dispose();

            File.Delete(file);
        }
    }
}
