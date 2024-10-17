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
    public class DatFileReadWriteSelfTests {
        private static Random _rnd = new Random();

        [TestMethod]
        public void CanWriteReadMultipleValues() {
            var bytes = new byte[12];

            var writer = new DatFileWriter(bytes);

            var randomBytes = new byte[4];
            _rnd.NextBytes(randomBytes);

            writer.WriteUInt32(1);
            writer.WriteInt32(-1);
            writer.WriteBytes(randomBytes, 4);

            var reader = new DatFileReader(bytes);

            Assert.AreEqual(1u, reader.ReadUInt32());
            Assert.AreEqual(-1, reader.ReadInt32());
            CollectionAssert.AreEqual(randomBytes, reader.ReadBytes(4));
        }

        [TestMethod]
        public void CanSkipAndWriteRead() {
            var bytes = new byte[12];

            var writer = new DatFileWriter(bytes);
            writer.Skip(4);
            writer.WriteUInt32(1);
            writer.WriteInt32(-1);

            var reader = new DatFileReader(bytes);
            reader.Skip(4);

            Assert.AreEqual(1u, reader.ReadUInt32());
            Assert.AreEqual(-1, reader.ReadInt32());
        }

        [TestMethod]
        [CombinatorialData]
        public void CanWriteReadUInt32([DataValues(1234u, 5678u, 0u, 1u, 0xFFFFFFFFu)] uint number) {
            var bytes = new byte[4];
            var writer = new DatFileWriter(bytes);
            writer.WriteUInt32(number);

            var reader = new DatFileReader(bytes);
            Assert.AreEqual(number, reader.ReadUInt32());
        }

        [TestMethod]
        [CombinatorialData]
        public void CanWriteReadInt32([DataValues(-1234, 5678, 0, 1, unchecked((int)0xFFFFFFFF))] int number) {
            var bytes = new byte[4];
            var writer = new DatFileWriter(bytes);
            writer.WriteInt32(number);

            var reader = new DatFileReader(bytes);
            Assert.AreEqual(number, reader.ReadInt32());
        }

        [TestMethod]
        public void CanWriteReadBytes() {
            var bytes = new byte[100];
            var randomBytes = new byte[100];
            _rnd.NextBytes(randomBytes);

            var writer = new DatFileWriter(randomBytes);

            writer.WriteBytes(bytes, 100);

            var reader = new DatFileReader(bytes);

            CollectionAssert.AreEqual(randomBytes, reader.ReadBytes(100));
        }
    }
}
