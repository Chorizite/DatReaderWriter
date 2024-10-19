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
    public class LightInfoTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new LightInfo() {
                ConeAngle = 0.5f,
                Color = new ColorARGB() { Alpha = 123, Blue = 255, Green = 0, Red = 55 },
                Falloff = 0.5f,
                Intensity = 0.5f,
                ViewSpaceLocation = new Frame() {
                    Orientation = Quaternion.Identity,
                    Origin = Vector3.One
                }
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new LightInfo();
            var reader = new DatFileReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.ConeAngle, readObj.ConeAngle);
            Assert.AreEqual(writeObj.Color.Alpha, readObj.Color.Alpha);
            Assert.AreEqual(writeObj.Color.Blue, readObj.Color.Blue);
            Assert.AreEqual(writeObj.Color.Green, readObj.Color.Green);
            Assert.AreEqual(writeObj.Color.Red, readObj.Color.Red);
            Assert.AreEqual(writeObj.Falloff, readObj.Falloff);
            Assert.AreEqual(writeObj.Intensity, readObj.Intensity);
            Assert.AreEqual(writeObj.ViewSpaceLocation.Origin, readObj.ViewSpaceLocation.Origin);
            Assert.AreEqual(writeObj.ViewSpaceLocation.Orientation, readObj.ViewSpaceLocation.Orientation);
        }
    }
}
