using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class DBPropertiesTests {
        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEOR() {
            using var dat = new DatCollection(EORCommonData.DatDirectory);

            var res = dat.TryGet<DBProperties>(0x78000000u, out var readObj);
            Assert.IsTrue(res);
            Assert.IsNotNull(readObj);
            Assert.AreEqual(0x78000000u, readObj.Id);

            Assert.AreEqual(1, readObj.Properties.Count);
            Assert.IsInstanceOfType(readObj.Properties[210], typeof(ArrayBaseProperty));
            var arrayProp = (ArrayBaseProperty)readObj.Properties[210];

            Assert.AreEqual(2, arrayProp.Value.Count);

            foreach (var prop in arrayProp.Value) {
                Console.WriteLine($"prop1 {prop}");
                var s = prop as StructBaseProperty;

                foreach (var kv in s.Value) {
                    Console.WriteLine($"\t{kv.Key}: {kv.Value}");
                }
            }

            var res2 = dat.TryGet<DBProperties>(0x78000001u, out var readObj2);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj2);
            Assert.AreEqual(0x78000001u, readObj2.Id);

            Assert.AreEqual(3, readObj2.Properties.Count);

            foreach (var prop in readObj2.Properties) {
                Console.WriteLine($"prop1 {prop.Key}: {prop.Value}");
            }

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            using var dat = new DatCollection(EORCommonData.DatDirectory);
            TestHelpers.CanReadAndWriteIdentical<DBProperties>("", 0x78000000, dat.Portal);
            TestHelpers.CanReadAndWriteIdentical<DBProperties>("", 0x78000001, dat.Portal);
        }
    }
}
