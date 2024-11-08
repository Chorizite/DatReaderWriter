using DatReaderWriter.Enums;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests.Types {
    [TestClass]
    public class ObjectDescTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new ObjectDesc() {
                ObjectId = 1234,
                BaseLoc = new Frame() {
                    Origin = new Vector3(1, 2, 3),
                    Orientation = new Quaternion(1, 2, 3, 4),
                },
                Frequency = 1f,
                DisplaceX = 1.1f,
                DisplaceY = 2.2f,
                MinScale = 1f,
                MaxScale = 2f,
                MaxRotation = 360f,
                MinSlope = 0.1f,
                MaxSlope = 0.2f,
                Align = 2,
                Orient = 3,
                WeenieObj = 0x12345678
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new ObjectDesc();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.ObjectId, readObj.ObjectId);
            Assert.AreEqual(writeObj.BaseLoc.Origin, readObj.BaseLoc.Origin);
            Assert.AreEqual(writeObj.BaseLoc.Orientation, readObj.BaseLoc.Orientation);
            Assert.AreEqual(writeObj.Frequency, readObj.Frequency);
            Assert.AreEqual(writeObj.DisplaceX, readObj.DisplaceX);
            Assert.AreEqual(writeObj.DisplaceY, readObj.DisplaceY);
            Assert.AreEqual(writeObj.MinScale, readObj.MinScale);
            Assert.AreEqual(writeObj.MaxScale, readObj.MaxScale);
            Assert.AreEqual(writeObj.MaxRotation, readObj.MaxRotation);
            Assert.AreEqual(writeObj.MinSlope, readObj.MinSlope);
            Assert.AreEqual(writeObj.MaxSlope, readObj.MaxSlope);
            Assert.AreEqual(writeObj.Align, readObj.Align);
            Assert.AreEqual(writeObj.Orient, readObj.Orient);
            Assert.AreEqual(writeObj.WeenieObj, readObj.WeenieObj);
        }
    }
}
