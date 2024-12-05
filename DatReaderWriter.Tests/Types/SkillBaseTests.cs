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
    public class SkillBaseTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new SkillBase() {
                Name = "test",
                Description = "testing 123",
                Category = SkillCategory.Other,
                ChargenUse = true,
                Formula = new SkillFormula() {

                },
                IconId = 124,
                LearnMod = 1.1f,
                LowerBound = 1,
                UpperBound = 2,
                MinLevel = 3,
                SpecializedCost = 4,
                TrainedCost = 5
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new SkillBase();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.IsNotNull(readObj);
            Assert.AreEqual(writeObj.Name, readObj.Name);
            Assert.AreEqual(writeObj.Description, readObj.Description);
            Assert.AreEqual(writeObj.Category, readObj.Category);
            Assert.AreEqual(writeObj.ChargenUse, readObj.ChargenUse);
            Assert.AreEqual(writeObj.IconId, readObj.IconId);
            Assert.AreEqual(writeObj.LearnMod, readObj.LearnMod);
            Assert.AreEqual(writeObj.LowerBound, readObj.LowerBound);
            Assert.AreEqual(writeObj.UpperBound, readObj.UpperBound);
            Assert.AreEqual(writeObj.MinLevel, readObj.MinLevel);
            Assert.AreEqual(writeObj.SpecializedCost, readObj.SpecializedCost);
            Assert.AreEqual(writeObj.TrainedCost, readObj.TrainedCost);
        }
    }
}
