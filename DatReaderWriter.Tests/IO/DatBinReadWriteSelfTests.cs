using DatReaderWriter.Lib.IO;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Tests.Lib;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests.IO {
    [TestClass]
    public class DatBinReadWriteSelfTests {
        private static Random _rnd = new Random();

        [TestMethod]
        public void CanWriteReadMultipleValues() {
            var bytes = new byte[12];

            var writer = new DatBinWriter(bytes);

            var randomBytes = new byte[4];
            _rnd.NextBytes(randomBytes);

            writer.WriteUInt32(1);
            writer.WriteInt32(-1);
            writer.WriteBytes(randomBytes, 4);

            var reader = new DatBinReader(bytes);

            Assert.AreEqual(1u, reader.ReadUInt32());
            Assert.AreEqual(-1, reader.ReadInt32());
            CollectionAssert.AreEqual(randomBytes, reader.ReadBytes(4));
        }

        [TestMethod]
        public void CanSkipAndWriteRead() {
            var bytes = new byte[12];

            var writer = new DatBinWriter(bytes);
            writer.Skip(4);
            writer.WriteUInt32(1);
            writer.WriteInt32(-1);

            var reader = new DatBinReader(bytes);
            reader.Skip(4);

            Assert.AreEqual(1u, reader.ReadUInt32());
            Assert.AreEqual(-1, reader.ReadInt32());
        }

        [TestMethod]
        [CombinatorialData]
        public void CanReadWriteInt16([DataValues((short)-1234, (short)5678, (short)0, (short)1, short.MinValue, short.MaxValue)] short number) {
            var bytes = new byte[sizeof(short)];
            var writer = new DatBinWriter(bytes);
            writer.WriteInt16(number);

            var reader = new DatBinReader(bytes);
            Assert.AreEqual(number, reader.ReadInt16());
            Assert.AreEqual(sizeof(short), reader.Offset);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanReadWriteUInt16([DataValues((ushort)1234u, (ushort)5678u, (ushort)0u, (ushort)1u, ushort.MaxValue, ushort.MinValue)] ushort number) {
            var bytes = new byte[sizeof(ushort)];
            var writer = new DatBinWriter(bytes);
            writer.WriteUInt16(number);

            var reader = new DatBinReader(bytes);
            Assert.AreEqual(number, reader.ReadUInt16());
            Assert.AreEqual(sizeof(ushort), reader.Offset);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanWriteReadUInt32([DataValues(1234u, 5678u, 0u, 1u, 0xFFFFFFFFu)] uint number) {
            var bytes = new byte[sizeof(uint)];
            var writer = new DatBinWriter(bytes);
            writer.WriteUInt32(number);

            var reader = new DatBinReader(bytes);
            Assert.AreEqual(number, reader.ReadUInt32());
            Assert.AreEqual(sizeof(uint), reader.Offset);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanWriteReadInt32([DataValues(-1234, 5678, 0, 1, unchecked((int)0xFFFFFFFF))] int number) {
            var bytes = new byte[sizeof(int)];
            var writer = new DatBinWriter(bytes);
            writer.WriteInt32(number);

            var reader = new DatBinReader(bytes);
            Assert.AreEqual(number, reader.ReadInt32());
            Assert.AreEqual(sizeof(int), reader.Offset);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanWritereadSingle([DataValues(0f, 1f, -1f, 0.123f, 12f, float.MaxValue, float.MinValue)] float number) {
            var bytes = new byte[sizeof(float)];
            var writer = new DatBinWriter(bytes);
            writer.WriteSingle(number);

            var reader = new DatBinReader(bytes);
            Assert.AreEqual(number, reader.ReadSingle());
            Assert.AreEqual(sizeof(float), reader.Offset);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanWritereadSingle([DataValues(0.0, 1.0, -1.0, 0.123, (double)12, double.MaxValue, double.MinValue)] double number) {
            var bytes = new byte[sizeof(double)];
            var writer = new DatBinWriter(bytes);
            writer.WriteDouble(number);

            var reader = new DatBinReader(bytes);
            Assert.AreEqual(number, reader.ReadDouble());
            Assert.AreEqual(sizeof(double), reader.Offset);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanWriteCompressedUInt([DataValues(1234u, 5678u, 0u, 1u)] uint number) {
            var bytes = new byte[sizeof(uint)];
            var writer = new DatBinWriter(bytes);
            writer.WriteCompressedUInt(number);

            var reader = new DatBinReader(bytes);
            Assert.AreEqual(number, reader.ReadCompressedUInt());
        }

        [TestMethod]
        [CombinatorialData]
        public void CanWriteReadDataIdOfKnownType([DataValues(1234u, 0x7FFEu, 0x8001u, 0u, 1u)] uint number) {
            var bytes = new byte[6];
            var writer = new DatBinWriter(bytes);

            writer.WriteDataIdOfKnownType(number + 0x01000000u, 0x01000000u);

            var reader = new DatBinReader(bytes);
            var v = reader.ReadDataIdOfKnownType(0x01000000u);
            Assert.AreEqual(number + 0x01000000u, v, $"""Expected {number:X8} but got {v:X8}""");
        }

        [TestMethod]
        public void CanWriteReadVector3() {
            var v = new Vector3(1, 2, 3);

            var bytes = new byte[12];
            var writer = new DatBinWriter(bytes);
            writer.WriteVector3(v);

            var reader = new DatBinReader(bytes);
            Assert.AreEqual(v, reader.ReadVector3());
            Assert.AreEqual(12, reader.Offset);
        }

        [TestMethod]
        public void CanWriteReadQuaternion() {
            var q = new Quaternion(1, 2, 3, 4);

            var bytes = new byte[16];
            var writer = new DatBinWriter(bytes);
            writer.WriteQuaternion(q);

            var reader = new DatBinReader(bytes);
            Assert.AreEqual(q, reader.ReadQuaternion());
            Assert.AreEqual(16, reader.Offset);
        }

        [TestMethod]
        public void CanWriteReadPlane() {
            var p = new Plane(1, 2, 3, 4);

            var bytes = new byte[16];
            var writer = new DatBinWriter(bytes);
            writer.WritePlane(p);

            var reader = new DatBinReader(bytes);
            Assert.AreEqual(p, reader.ReadPlane());
            Assert.AreEqual(16, reader.Offset);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanWriteReadTrueBoolSizes([DataValues(1, 2, 4, 8)] int size) {
            var bytes = new byte[size];
            bytes.AsSpan().Fill(1);

            var reader = new DatBinReader(bytes);
            Assert.IsTrue(reader.ReadBool(size));
        }

        [TestMethod]
        [CombinatorialData]
        public void CanWriteReadFalseBoolSizes([DataValues(1, 2, 4, 8)] int size) {
            var bytes = new byte[size];
            bytes.AsSpan().Fill(0);

            var reader = new DatBinReader(bytes);
            Assert.IsFalse(reader.ReadBool(size));
        }

        [TestMethod]
        public void CanWriteReadBytes() {
            var bytes = new byte[100];
            var randomBytes = new byte[100];
            _rnd.NextBytes(randomBytes);

            var writer = new DatBinWriter(randomBytes);

            writer.WriteBytes(bytes, 100);

            var reader = new DatBinReader(bytes);

            CollectionAssert.AreEqual(randomBytes, reader.ReadBytes(100));
        }
    }
}
