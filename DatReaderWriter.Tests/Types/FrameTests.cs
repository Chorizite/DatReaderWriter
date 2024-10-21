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
    public class FrameTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new Frame() {
                Origin = new Vector3(1, 2, 3),
                Orientation = new Quaternion(0, 0, 0, 1),
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new Frame();
            var reader = new DatFileReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Origin, readObj.Origin);
            Assert.AreEqual(writeObj.Orientation, readObj.Orientation);
        }
    }
}
