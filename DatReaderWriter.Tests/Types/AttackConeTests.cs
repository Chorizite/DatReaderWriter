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
    public class AttackConeTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new AttackCone() {
                PartIndex = 123,
                Height = 456,
                LeftX = 789,
                LeftY = 987,
                Radius = 0.12f,
                RightX = 654,
                RightY = 321
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new AttackCone();
            var reader = new DatFileReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.PartIndex, readObj.PartIndex);
            Assert.AreEqual(writeObj.Height, readObj.Height);
            Assert.AreEqual(writeObj.LeftX, readObj.LeftX);
            Assert.AreEqual(writeObj.LeftY, readObj.LeftY);
            Assert.AreEqual(writeObj.Radius, readObj.Radius);
            Assert.AreEqual(writeObj.RightX, readObj.RightX);
            Assert.AreEqual(writeObj.RightY, readObj.RightY);
        }
    }
}
