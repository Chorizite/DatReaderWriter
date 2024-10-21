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
    public class StabTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new Stab() {
                Frame = new Frame() {
                    Origin = new Vector3(1, 2, 3),
                    Orientation = new Quaternion(1, 2, 3, 4),
                },
                Id = 151235,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new Stab();
            var reader = new DatFileReader(buffer);
            readObj.Unpack(reader);

            Assert.IsNotNull(readObj);
            Assert.AreEqual(writeObj.Frame.Origin, readObj.Frame.Origin);
            Assert.AreEqual(writeObj.Frame.Orientation, readObj.Frame.Orientation);
            Assert.AreEqual(writeObj.Id, readObj.Id);
        }
    }
}
