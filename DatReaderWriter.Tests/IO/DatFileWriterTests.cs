using ACClientLib.DatReaderWriter.IO;
using ACClientLib.DatReaderWriter.Tests.Lib;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACClientLib.DatReaderWriter.Tests.IO {
    [TestClass]
    public class DatFileWriterTests {
        private static Random _rnd = new Random();

        [TestMethod]
        public void CanWriteMultipleValues() {
            var bytes = new byte[12];

            var writer = new DatFileWriter(bytes);

            var randomBytes = new byte[4];
            _rnd.NextBytes(randomBytes);

            Assert.AreEqual(0, writer.Offset);

            writer.WriteUInt32(1);
            Assert.AreEqual(4, writer.Offset);

            writer.WriteInt32(-1);
            Assert.AreEqual(8, writer.Offset);

            writer.WriteBytes(randomBytes, 4);
            Assert.AreEqual(12, writer.Offset);

            var bSpan = new Span<byte>(bytes);

            Assert.AreEqual(1u, BinaryPrimitives.ReadUInt32LittleEndian(bSpan.Slice(0)));
            Assert.AreEqual(-1, BinaryPrimitives.ReadInt32LittleEndian(bSpan.Slice(4)));
            CollectionAssert.AreEqual(randomBytes, bSpan.Slice(8, 4).ToArray());
        }

        [TestMethod]
        public void CanSkipAndWrite() {
            var bytes = new byte[12];

            var writer = new DatFileWriter(bytes);
            Assert.AreEqual(0, writer.Offset);

            writer.Skip(4);
            Assert.AreEqual(4, writer.Offset);

            writer.WriteUInt32(1);
            Assert.AreEqual(8, writer.Offset);

            writer.WriteInt32(-1);
            Assert.AreEqual(12, writer.Offset);

            var bSpan = new Span<byte>(bytes);

            Assert.AreEqual(1u, BinaryPrimitives.ReadUInt32LittleEndian(bSpan.Slice(4)));
            Assert.AreEqual(-1, BinaryPrimitives.ReadInt32LittleEndian(bSpan.Slice(8)));
        }

        [TestMethod]
        [CombinatorialData]
        public void CanWriteUInt32([DataValues(1234u, 5678u, 0u, 1u, 0xFFFFFFFFu)] uint number) {
            var bytes = new byte[4];
            var writer = new DatFileWriter(bytes);

            Assert.AreEqual(0, writer.Offset);
            writer.WriteUInt32(number);
            Assert.AreEqual(4, writer.Offset);

            Assert.AreEqual(number, BinaryPrimitives.ReadUInt32LittleEndian(bytes));
        }

        [TestMethod]
        [CombinatorialData]
        public void CanWriteInt32([DataValues(-1234, 5678, 0, 1, unchecked((int)0xFFFFFFFF))] int number) {
            var bytes = new byte[4];
            var writer = new DatFileWriter(bytes);

            Assert.AreEqual(0, writer.Offset);
            writer.WriteInt32(number);
            Assert.AreEqual(4, writer.Offset);

            Assert.AreEqual(number, BinaryPrimitives.ReadInt32LittleEndian(bytes));
        }

        [TestMethod]
        public void CanWriteBytes() {
            var bytes = new byte[100];
            var randomBytes = new byte[100];
            _rnd.NextBytes(randomBytes);

            var writer = new DatFileWriter(randomBytes);

            Assert.AreEqual(0, writer.Offset);
            writer.WriteBytes(bytes, randomBytes.Length);
            Assert.AreEqual(randomBytes.Length, writer.Offset);

            CollectionAssert.AreEqual(randomBytes, bytes);
        }
    }
}
