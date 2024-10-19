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
    public class PolygonTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new Polygon() {
                NegSurface = 1,
                PosSurface = 2,
                NegUVIndices = [ 1, 2, 3 ],
                PosUVIndices = [ 4, 5, 6 ],
                SidesType = CullMode.Clockwise,
                Stippling = StipplingType.Both,
                VertexIds = [ 7, 8, 9 ]
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new Polygon();
            var reader = new DatFileReader(buffer);
            readObj.Unpack(reader);

            Assert.IsNotNull(readObj);
            Assert.AreEqual(writeObj.NegSurface, readObj.NegSurface);
            Assert.AreEqual(writeObj.PosSurface, readObj.PosSurface);
            Assert.AreEqual(writeObj.NegUVIndices.Count, readObj.NegUVIndices.Count);
            CollectionAssert.AreEqual(writeObj.NegUVIndices, readObj.NegUVIndices);
            Assert.AreEqual(writeObj.PosUVIndices.Count, readObj.PosUVIndices.Count);
            CollectionAssert.AreEqual(writeObj.PosUVIndices, readObj.PosUVIndices);
            Assert.AreEqual(writeObj.SidesType, readObj.SidesType);
            Assert.AreEqual(writeObj.Stippling, readObj.Stippling);
            Assert.AreEqual(writeObj.VertexIds.Count, readObj.VertexIds.Count);
            CollectionAssert.AreEqual(writeObj.VertexIds, readObj.VertexIds);
        }
    }
}
