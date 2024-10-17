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
    public class DatFileReaderTests {
        private static Random _rnd = new Random();

        [TestMethod]
        public void CanReadMultipleValues() {
            var bytes = new byte[12];
            var bSpan = new Span<byte>(bytes);
            BinaryPrimitives.WriteUInt32LittleEndian(bSpan.Slice(0), 1);
            BinaryPrimitives.WriteInt32LittleEndian(bSpan.Slice(4), -1);
            BinaryPrimitives.WriteUInt32LittleEndian(bSpan.Slice(8), 0);

            var reader = new DatFileReader(bytes);

            Assert.AreEqual(0, reader.Offset);

            Assert.AreEqual(1u, reader.ReadUInt32());
            Assert.AreEqual(4, reader.Offset);

            Assert.AreEqual(-1, reader.ReadInt32());
            Assert.AreEqual(8, reader.Offset);

            Assert.AreEqual(0u, reader.ReadUInt32());
            Assert.AreEqual(12, reader.Offset);
        }

        [TestMethod]
        public void CanSkipAndRead() {
            var bytes = new byte[12];
            var bSpan = new Span<byte>(bytes);
            BinaryPrimitives.WriteUInt32LittleEndian(bSpan.Slice(0), 1);
            BinaryPrimitives.WriteInt32LittleEndian(bSpan.Slice(4), -1);
            BinaryPrimitives.WriteUInt32LittleEndian(bSpan.Slice(8), 0);

            var reader = new DatFileReader(bytes);

            Assert.AreEqual(0, reader.Offset);

            reader.Skip(4);
            Assert.AreEqual(4, reader.Offset);

            Assert.AreEqual(-1, reader.ReadInt32());
            Assert.AreEqual(8, reader.Offset);

            Assert.AreEqual(0u, reader.ReadUInt32());
            Assert.AreEqual(12, reader.Offset);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanReadUInt32([DataValues(1234u, 5678u, 0u, 1u, 0xFFFFFFFFu)] uint number) {
            var bytes = new byte[4];
            BinaryPrimitives.WriteUInt32LittleEndian(bytes, number);

            var reader = new DatFileReader(bytes);
            Assert.AreEqual(0, reader.Offset);

            Assert.AreEqual(number, reader.ReadUInt32());
            Assert.AreEqual(4, reader.Offset);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanReadInt32([DataValues(-1234, 5678, 0, 1, unchecked((int)0xFFFFFFFF))] int number) {
            var bytes = new byte[4];
            BinaryPrimitives.WriteInt32LittleEndian(bytes, number);

            var reader = new DatFileReader(bytes);
            Assert.AreEqual(0, reader.Offset);

            Assert.AreEqual(number, reader.ReadInt32());
            Assert.AreEqual(4, reader.Offset);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanReadInt64([DataValues((Int64)(-1234), (Int64)5678, (Int64)0, (Int64)1, Int64.MaxValue, Int64.MinValue)] long number) {
            var bytes = new byte[8];
            BinaryPrimitives.WriteInt64LittleEndian(bytes, number);

            var reader = new DatFileReader(bytes);
            Assert.AreEqual(0, reader.Offset);

            Assert.AreEqual(number, reader.ReadInt64());
            Assert.AreEqual(8, reader.Offset);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanReadUInt64([DataValues((UInt64)5678, (UInt64)0, (UInt64)1, UInt64.MaxValue, UInt64.MinValue)] ulong number) {
            var bytes = new byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(bytes, number);

            var reader = new DatFileReader(bytes);
            Assert.AreEqual(0, reader.Offset);

            Assert.AreEqual(number, reader.ReadUInt64());
            Assert.AreEqual(8, reader.Offset);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanReadBool([DataValues(0u, 1u, 1000u)] uint number) {
            var bytes = new byte[4];
            BinaryPrimitives.WriteUInt32LittleEndian(bytes, number);

            var reader = new DatFileReader(bytes);
            Assert.AreEqual(number != 0, reader.ReadBool());
        }

        [TestMethod]
        public void CanAlignToBoundary() {
            var bytes = new byte[100];
            var reader = new DatFileReader(bytes);
            Assert.AreEqual(0, reader.Offset);

            reader.Align(4);
            Assert.AreEqual(0, reader.Offset); // already aligned, shouldnt move

            reader.Skip(3);
            Assert.AreEqual(3, reader.Offset);
            reader.Align(4);
            Assert.AreEqual(4, reader.Offset);

            reader.Skip(3);
            Assert.AreEqual(7, reader.Offset);
            reader.Align(4);
            Assert.AreEqual(8, reader.Offset);
        }

        [TestMethod]
        public void CanReadByte() {
            var bytes = new byte[100];
            _rnd.NextBytes(bytes);

            var reader = new DatFileReader(bytes);
            Assert.AreEqual(0, reader.Offset);

            var readBytes = new byte[100];
            for (var i = 0; i < 100; i++) {
                readBytes[i] = reader.ReadByte();
            }

            CollectionAssert.AreEqual(bytes, readBytes);
        }

        [TestMethod]
        public void CanReadBytes() {
            var bytes = new byte[100];
            _rnd.NextBytes(bytes);

            var reader = new DatFileReader(bytes);
            Assert.AreEqual(0, reader.Offset);

            var readBytes = reader.ReadBytes(bytes.Length);
            Assert.AreEqual(bytes.Length, reader.Offset);

            CollectionAssert.AreEqual(bytes, readBytes);
        }
    }
}
