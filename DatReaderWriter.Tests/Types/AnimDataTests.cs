using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.IO;
using ACClientLib.DatReaderWriter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests.Types {
    [TestClass]
    public class AnimDataTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new AnimData() {
                AnimId = 0x12345678,
                HighFrame = 0x1234,
                LowFrame = 0x5678,
                Framerate = 0.12f
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new AnimData();
            var reader = new DatFileReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.AnimId, readObj.AnimId);
            Assert.AreEqual(writeObj.HighFrame, readObj.HighFrame);
            Assert.AreEqual(writeObj.LowFrame, readObj.LowFrame);
            Assert.AreEqual(writeObj.Framerate, readObj.Framerate);
        }
    }
}
