using DatReaderWriter.Enums;
using DatReaderWriter.Options;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Lib.IO.DatBTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace DatReaderWriter.Tests {
    [TestClass]
    public class DatDatabaseOptimizationTests {
        private string _tempDatPath = null!;

        [TestInitialize]
        public void Initialize() {
            _tempDatPath = Path.Combine(Path.GetTempPath(), $"test_opt_{Guid.NewGuid():N}.dat");
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
        public void TestTryGetFileBytesSpan_Uncompressed() {
            var id = 0x01000001u;
            var data = new byte[1024];
            for (int i = 0; i < data.Length; i++) data[i] = (byte)(i % 255);

            using (var db = CreateTempDatabase()) {
                db.TryWriteFileBytes(id, data, data.Length, 1);

                Span<byte> buffer = new byte[1024];
                bool result = db.TryGetFileBytes(id, buffer, out int bytesRead);

                Assert.IsTrue(result, "TryGetFileBytes failed");
                Assert.AreEqual(data.Length, bytesRead, "Bytes read mismatch");
                Assert.IsTrue(data.AsSpan().SequenceEqual(buffer), "Data mismatch");
            }
        }

        [TestMethod]
        public void TestTryGetFileBytesSpan_Compressed() {
            var id = 0x01000002u;
            var data = new byte[1024];
            for (int i = 0; i < data.Length; i++) data[i] = (byte)(i % 10); // Highly compressible

            using (var db = CreateTempDatabase()) {
                db.TryWriteCompressedBytes(id, data, data.Length, 1);

                Span<byte> buffer = new byte[1024];
                bool result = db.TryGetFileBytes(id, buffer, out int bytesRead);

                Assert.IsTrue(result, "TryGetFileBytes failed");
                Assert.AreEqual(data.Length, bytesRead, "Bytes read mismatch");
                Assert.IsTrue(data.AsSpan().SequenceEqual(buffer), "Data mismatch");
            }
        }

        [TestMethod]
        public void TestTryGetFileBytesSpan_SmallBuffer() {
            var id = 0x01000003u;
            var data = new byte[1024];
            for (int i = 0; i < data.Length; i++) data[i] = (byte)(i % 255);

            using (var db = CreateTempDatabase()) {
                db.TryWriteFileBytes(id, data, data.Length, 1);

                // Buffer smaller than data
                Span<byte> buffer = new byte[512];
                // Checks if it throws or returns partial
                // Current implementation might return true but fewer bytes, or fail?
                // Let's observe behavior. Ideally it should fill what it can or fail.
                // Standard Stream.Read behavior is to fill what it can.

                bool result = db.TryGetFileBytes(id, buffer, out int bytesRead);

                // Assertions depend on desired behavior.
                // Assuming it works like Read (success, generic bytesRead).
                Assert.IsTrue(result);
                Assert.AreEqual(512, bytesRead);
                Assert.IsTrue(data.AsSpan(0, 512).SequenceEqual(buffer));
            }
        }
    }
}
