using System;
using DatReaderWriter.Enums;
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
    public class CylSphereTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new CylSphere() {
                Height = 1.1f,
                Origin = new Vector3(1, 2, 3),
                Radius = 4.5f,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new CylSphere();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.IsNotNull(readObj);
            Assert.AreEqual(writeObj.Height, readObj.Height);
            Assert.AreEqual(writeObj.Origin, readObj.Origin);
            Assert.AreEqual(writeObj.Radius, readObj.Radius);
        }
    }
}
