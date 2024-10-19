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
        public void CanReadSingle([DataValues(0.123f, -0.123f, 0f, 1f, float.MaxValue, float.MinValue)] float number) {
            var bytes = new byte[sizeof(float)];
#if (NETFRAMEWORK || NETSTANDARD2_0)
            BitConverter.GetBytes(number).CopyTo(bytes.AsSpan());
#else
            BinaryPrimitives.WriteSingleLittleEndian(bytes, number);
#endif
            var reader = new DatFileReader(bytes);
            Assert.AreEqual(0, reader.Offset);

            Assert.AreEqual(number, reader.ReadSingle());
            Assert.AreEqual(sizeof(float), reader.Offset);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanReadDouble([DataValues(0.123, -0.123, 0, 1, double.MaxValue, double.MinValue)] double number) {
            var bytes = new byte[sizeof(double)];
#if (NETFRAMEWORK || NETSTANDARD2_0)
            BitConverter.GetBytes(number).CopyTo(bytes.AsSpan());
#else
            BinaryPrimitives.WriteDoubleLittleEndian(bytes, number);
#endif
            var reader = new DatFileReader(bytes);
            Assert.AreEqual(0, reader.Offset);

            Assert.AreEqual(number, reader.ReadDouble());
            Assert.AreEqual(sizeof(double), reader.Offset);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanReadUInt16([DataValues((ushort)1234u, (ushort)5678u, (ushort)0u, (ushort)1u, ushort.MaxValue, ushort.MinValue)] ushort number) {
            var bytes = new byte[sizeof(ushort)];
            BinaryPrimitives.WriteUInt16LittleEndian(bytes, number);

            var reader = new DatFileReader(bytes);
            Assert.AreEqual(0, reader.Offset);

            Assert.AreEqual(number, reader.ReadUInt16());
            Assert.AreEqual(sizeof(ushort), reader.Offset);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanReadInt16([DataValues((short)-1234, (short)5678, (short)0, (short)1, short.MinValue, short.MaxValue)] short number) {
            var bytes = new byte[sizeof(short)];
            BinaryPrimitives.WriteInt16LittleEndian(bytes, number);

            var reader = new DatFileReader(bytes);
            Assert.AreEqual(0, reader.Offset);

            Assert.AreEqual(number, reader.ReadInt16());
            Assert.AreEqual(sizeof(short), reader.Offset);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanReadUInt32([DataValues(1234u, 5678u, 0u, 1u, 0xFFFFFFFFu)] uint number) {
            var bytes = new byte[sizeof(uint)];
            BinaryPrimitives.WriteUInt32LittleEndian(bytes, number);

            var reader = new DatFileReader(bytes);
            Assert.AreEqual(0, reader.Offset);

            Assert.AreEqual(number, reader.ReadUInt32());
            Assert.AreEqual(sizeof(uint), reader.Offset);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanReadInt32([DataValues(-1234, 5678, 0, 1, unchecked((int)0xFFFFFFFF))] int number) {
            var bytes = new byte[sizeof(int)];
            BinaryPrimitives.WriteInt32LittleEndian(bytes, number);

            var reader = new DatFileReader(bytes);
            Assert.AreEqual(0, reader.Offset);

            Assert.AreEqual(number, reader.ReadInt32());
            Assert.AreEqual(sizeof(int), reader.Offset);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanReadInt64([DataValues((Int64)(-1234), (Int64)5678, (Int64)0, (Int64)1, Int64.MaxValue, Int64.MinValue)] long number) {
            var bytes = new byte[sizeof(long)];
            BinaryPrimitives.WriteInt64LittleEndian(bytes, number);

            var reader = new DatFileReader(bytes);
            Assert.AreEqual(0, reader.Offset);

            Assert.AreEqual(number, reader.ReadInt64());
            Assert.AreEqual(sizeof(long), reader.Offset);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanReadUInt64([DataValues((UInt64)5678, (UInt64)0, (UInt64)1, UInt64.MaxValue, UInt64.MinValue)] ulong number) {
            var bytes = new byte[sizeof(ulong)];
            BinaryPrimitives.WriteUInt64LittleEndian(bytes, number);

            var reader = new DatFileReader(bytes);
            Assert.AreEqual(0, reader.Offset);

            Assert.AreEqual(number, reader.ReadUInt64());
            Assert.AreEqual(sizeof(ulong), reader.Offset);
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
        [CombinatorialData]
        public void CanReadTrueBoolSizes([DataValues(1, 2, 4, 8)] int size) {
            var bytes = new byte[size];
            switch (size) {
                case 1:
                    bytes[0] = 1;
                    break;
                case 2:
                    BinaryPrimitives.WriteUInt16LittleEndian(bytes, 1);
                    break;
                case 4:
                    BinaryPrimitives.WriteUInt32LittleEndian(bytes, 1);
                    break;
                case 8:
                    BinaryPrimitives.WriteUInt64LittleEndian(bytes, 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(size), size, null);
            }

            var reader = new DatFileReader(bytes);
            Assert.IsTrue(reader.ReadBool(size));
        }

        [TestMethod]
        [CombinatorialData]
        public void CanReadFalseBoolSizes([DataValues(1, 2, 4, 8)] int size) {
            var bytes = new byte[size];
            bytes.AsSpan().Fill(0);

            var reader = new DatFileReader(bytes);
            Assert.IsFalse(reader.ReadBool(size));
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
