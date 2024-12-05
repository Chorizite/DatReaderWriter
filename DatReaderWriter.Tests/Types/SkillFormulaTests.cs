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
    public class SkillFormulaTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new SkillFormula() {
                Attribute1 = AttributeId.Strength,
                Attribute2 = AttributeId.Coordination,
                HasSecondAttribute = true,
                Divisor = 2,
                UseFormula = true
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new SkillFormula();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Attribute1, readObj.Attribute1);
            Assert.AreEqual(writeObj.Attribute2, readObj.Attribute2);
            Assert.AreEqual(writeObj.HasSecondAttribute, readObj.HasSecondAttribute);
            Assert.AreEqual(writeObj.Divisor, readObj.Divisor);
            Assert.AreEqual(writeObj.UseFormula, readObj.UseFormula);
        }
    }
}
