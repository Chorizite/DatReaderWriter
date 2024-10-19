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
    public class SWVertexTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new SWVertex() {
                Normal = new Vector3(1, 2, 3),
                Origin = new Vector3(4, 5, 6),
                UVs = [
                    new Vec2Duv() {
                        U = 7,
                        V = 8
                    },
                    new Vec2Duv() {
                        U = 9,
                        V = 10
                    }
                ]
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new SWVertex();
            var reader = new DatFileReader(buffer);
            readObj.Unpack(reader);

            Assert.IsNotNull(readObj);
            Assert.AreEqual(writeObj.Normal, readObj.Normal);
            Assert.AreEqual(writeObj.Origin, readObj.Origin);
            Assert.AreEqual(writeObj.UVs.Count, readObj.UVs.Count);
            Assert.AreEqual(writeObj.UVs[0].U, readObj.UVs[0].U);
            Assert.AreEqual(writeObj.UVs[0].V, readObj.UVs[0].V);
            Assert.AreEqual(writeObj.UVs[1].U, readObj.UVs[1].U);
            Assert.AreEqual(writeObj.UVs[1].V, readObj.UVs[1].V);
        }
    }
}
