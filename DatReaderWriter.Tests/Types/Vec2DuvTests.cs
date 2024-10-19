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
    public class Vec2DuvTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new Vec2Duv() {
                U = 0.5f,
                V = 0.1f
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new Vec2Duv();
            var reader = new DatFileReader(buffer);
            readObj.Unpack(reader);

            Assert.IsNotNull(readObj);
            Assert.AreEqual(writeObj.U, readObj.U);
            Assert.AreEqual(writeObj.V, readObj.V);
        }
    }
}
