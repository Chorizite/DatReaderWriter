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
    public class VertexArrayTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new VertexArray() {
                VertexType = VertexType.CSWVertexType,
                Vertices = new Dictionary<ushort, SWVertex>() {
                    { 0, new SWVertex() {
                        Normal = new Vector3(1, 2, 3),
                        Origin = new Vector3(4, 5, 6),
                        UVs = new List<Vec2Duv>() {
                            new Vec2Duv() {
                                U = 7,
                                V = 8
                            },
                            new Vec2Duv() {
                                U = 9,
                                V = 10
                            }
                        }
                    } }
                }
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new VertexArray();
            var reader = new DatFileReader(buffer);
            readObj.Unpack(reader);

            Assert.IsNotNull(readObj);
            Assert.AreEqual(writeObj.VertexType, readObj.VertexType);
            Assert.AreEqual(writeObj.Vertices.Count, readObj.Vertices.Count);
            Assert.AreEqual(writeObj.Vertices[0].Normal, readObj.Vertices[0].Normal);
            Assert.AreEqual(writeObj.Vertices[0].Origin, readObj.Vertices[0].Origin);
            Assert.AreEqual(writeObj.Vertices[0].UVs.Count, readObj.Vertices[0].UVs.Count);
            Assert.AreEqual(writeObj.Vertices[0].UVs[0].U, readObj.Vertices[0].UVs[0].U);
            Assert.AreEqual(writeObj.Vertices[0].UVs[0].V, readObj.Vertices[0].UVs[0].V);
        }
    }
}
