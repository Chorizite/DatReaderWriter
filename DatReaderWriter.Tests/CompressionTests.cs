using DatReaderWriter.Enums;
using DatReaderWriter.Options;
using DatReaderWriter.Lib.IO.DatBTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests {
    [TestClass]
    public class CompressionTests {
        private string _tempDatPath = null!;

        [TestInitialize]
        public void Initialize() {
            _tempDatPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}.dat");
        }

        [TestCleanup]
        public void Cleanup() {
            if (File.Exists(_tempDatPath)) {
                File.Delete(_tempDatPath);
            }
        }

        private DatDatabase CreateTempDatabase() {
            var db = new DatDatabase(o => {
                o.FilePath = _tempDatPath;
                o.AccessType = DatAccessType.ReadWrite;
            });
            db.BlockAllocator.InitNew(DatFileType.Portal, 0);
            return db;
        }

        [TestMethod]
        public void TestCompression() {
            var id = 0x01000001u;
            var data = new byte[1024];
            // Highly compressible data
            for (int i = 0; i < data.Length; i++) data[i] = (byte)(i % 10);

            using (var db = CreateTempDatabase()) {
                var writeResult = db.TryWriteCompressedBytes(id, data, data.Length, 1);
                Assert.IsTrue(writeResult.Success, $"Write failed: {writeResult.Error}");

                Assert.IsTrue(db.Tree.TryGetFile(id, out var fileEntry));
                Assert.IsTrue(fileEntry.Flags.HasFlag(DatBTreeFileFlags.IsCompressed), "IsCompressed flag not set");
                Assert.IsTrue(fileEntry.Size < data.Length,
                    $"Compressed size ({fileEntry.Size}) not smaller than original ({data.Length})");

                Assert.IsTrue(db.TryGetFileBytes(id, out var readBytes));
                Assert.AreEqual(data.Length, readBytes.Length, "Length mismatch");
                for (int i = 0; i < data.Length; i++) {
                    if (data[i] != readBytes[i]) {
                        var msg = $"Mismatch at index {i}: expected {data[i]}, got {readBytes[i]}\n";
                        msg += $"Context: {string.Join(", ", readBytes.Skip(Math.Max(0, i - 5)).Take(10))}";
                        Assert.Fail(msg);
                    }
                }
            }
        }

        [TestMethod]
        public async Task TestCompressionAsync() {
            var id = 0x01000001u;
            var data = new byte[1024];
            for (int i = 0; i < data.Length; i++) data[i] = (byte)(i % 10);

            using (var db = CreateTempDatabase()) {
                var writeResult = await db.TryWriteCompressedBytesAsync(id, data, data.Length, 1);
                Assert.IsTrue(writeResult.Success, $"Write failed: {writeResult.Error}");

                var (found, fileEntry) = await db.Tree.TryGetFileAsync(id);
                Assert.IsTrue(found);
                Assert.IsTrue(fileEntry.Flags.HasFlag(DatBTreeFileFlags.IsCompressed));

                var (readSuccess, readBytes) = await db.TryGetFileBytesAsync(id);
                Assert.IsTrue(readSuccess);
                CollectionAssert.AreEqual(data, readBytes);
            }
        }

        [TestMethod]
        public void TestNoCompressionOnIncompressibleData() {
            var id = 0x01000001u;
            var data = new byte[1024];
            new Random(123).NextBytes(data); // Use seed for predictability

            using (var db = CreateTempDatabase()) {
                var writeResult = db.TryWriteCompressedBytes(id, data, data.Length, 1);
                Assert.IsTrue(writeResult.Success);

                Assert.IsTrue(db.Tree.TryGetFile(id, out var fileEntry));
                Assert.IsFalse(fileEntry.Flags.HasFlag(DatBTreeFileFlags.IsCompressed),
                    $"Should not be compressed. Size: {fileEntry.Size}");
                Assert.AreEqual((uint)data.Length, fileEntry.Size);

                Assert.IsTrue(db.TryGetFileBytes(id, out var readBytes));
                CollectionAssert.AreEqual(data, readBytes);
            }
        }
    }
}
