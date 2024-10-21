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
    public class TerrainInfoTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new TerrainInfo() {
                Road = 2,
                Scenery = 3,
                Type = TerrainType.MarshSparseSwamp
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new TerrainInfo();
            var reader = new DatFileReader(buffer);
            readObj.Unpack(reader);

            Assert.IsNotNull(readObj);
            Assert.AreEqual(writeObj.Road, readObj.Road);
            Assert.AreEqual(writeObj.Scenery, readObj.Scenery);
            Assert.AreEqual(writeObj.Type, readObj.Type);
        }
    }
}
