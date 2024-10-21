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
    public class LocationTypeTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new LocationType() {
                PartId = 4124,
                Frame = new Frame() {
                    Origin = new Vector3(1, 2, 3),
                    Orientation = new Quaternion(0, 0, 0, 1),
                }
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new LocationType();
            var reader = new DatFileReader(buffer);
            readObj.Unpack(reader);

            Assert.IsNotNull(readObj);
            Assert.AreEqual(writeObj.PartId, readObj.PartId);
            Assert.AreEqual(writeObj.Frame.Origin, readObj.Frame.Origin);
            Assert.AreEqual(writeObj.Frame.Orientation, readObj.Frame.Orientation);
        }
    }
}
