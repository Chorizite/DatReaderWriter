using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class SkillTableTests {
        [TestMethod]
        public void CanInsertAndReadAnimations() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeAnim = new SkillTable() {
                Id = 0xE000004,
                Skills = new() {
                    { SkillId.Alchemy, new SkillBase(){
                        Name = "test",
                        Description = "testing 123",
                        Category = SkillCategory.Magic,
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
                    }  }
                }
            };

            var res = dat.TryWriteFile(writeAnim);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<SkillTable>(0xE000004, out var readSkillTable);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readSkillTable);

            Assert.AreEqual(0xE000004u, readSkillTable.Id);

            Assert.AreEqual(1, readSkillTable.Skills.Count);
            Assert.AreEqual("test", readSkillTable.Skills[SkillId.Alchemy].Name);
            Assert.AreEqual("testing 123", readSkillTable.Skills[SkillId.Alchemy].Description);
            Assert.AreEqual(SkillCategory.Magic, readSkillTable.Skills[SkillId.Alchemy].Category);
            Assert.IsTrue(readSkillTable.Skills[SkillId.Alchemy].ChargenUse);
            Assert.AreEqual(1.1f, readSkillTable.Skills[SkillId.Alchemy].LearnMod);
            Assert.AreEqual(1, readSkillTable.Skills[SkillId.Alchemy].LowerBound);
            Assert.AreEqual(2, readSkillTable.Skills[SkillId.Alchemy].UpperBound);
            Assert.AreEqual(3u, readSkillTable.Skills[SkillId.Alchemy].MinLevel);
            Assert.AreEqual(4, readSkillTable.Skills[SkillId.Alchemy].SpecializedCost);
            Assert.AreEqual(5, readSkillTable.Skills[SkillId.Alchemy].TrainedCost);


            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORSkillTable() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });


            var res = dat.TryReadFile<SkillTable>(0xE000004, out var skillTable);
            Assert.IsTrue(res);
            Assert.IsNotNull(skillTable);
            Assert.AreEqual(0xE000004u, skillTable.Id);

            Assert.AreEqual(38, skillTable.Skills.Count);

            Assert.AreEqual("Melee Defense", skillTable.Skills[SkillId.MeleeDefense].Name);
            Assert.AreEqual("Helps you evade melee (hand-to-hand) attacks.", skillTable.Skills[SkillId.MeleeDefense].Description);
            Assert.AreEqual(SkillCategory.Combat, skillTable.Skills[SkillId.MeleeDefense].Category);
            Assert.IsTrue(skillTable.Skills[SkillId.MeleeDefense].ChargenUse);
            Assert.AreEqual(0x06000165u, skillTable.Skills[SkillId.MeleeDefense].IconId);
            Assert.AreEqual(1f, skillTable.Skills[SkillId.MeleeDefense].LearnMod);
            Assert.AreEqual(120, skillTable.Skills[SkillId.MeleeDefense].LowerBound);
            Assert.AreEqual(900, skillTable.Skills[SkillId.MeleeDefense].UpperBound);
            Assert.AreEqual(1u, skillTable.Skills[SkillId.MeleeDefense].MinLevel);
            Assert.AreEqual(20, skillTable.Skills[SkillId.MeleeDefense].SpecializedCost);
            Assert.AreEqual(10, skillTable.Skills[SkillId.MeleeDefense].TrainedCost);
            Assert.AreEqual(1, skillTable.Skills[SkillId.MeleeDefense].Formula.Attribute1Multiplier);
            Assert.AreEqual(1, skillTable.Skills[SkillId.MeleeDefense].Formula.Attribute2Multiplier);
            Assert.AreEqual(3, skillTable.Skills[SkillId.MeleeDefense].Formula.Divisor);
            Assert.AreEqual(AttributeId.Quickness, skillTable.Skills[SkillId.MeleeDefense].Formula.Attribute1);
            Assert.AreEqual(AttributeId.Coordination, skillTable.Skills[SkillId.MeleeDefense].Formula.Attribute2);

            Assert.AreEqual("Summoning", skillTable.Skills[SkillId.Summoning].Name);
            Assert.AreEqual("Allows you to summon creatures to attack your foes.", skillTable.Skills[SkillId.Summoning].Description);
            Assert.AreEqual(SkillCategory.Combat, skillTable.Skills[SkillId.Summoning].Category);
            Assert.IsTrue(skillTable.Skills[SkillId.Summoning].ChargenUse);
            Assert.AreEqual(0x0600740Cu, skillTable.Skills[SkillId.Summoning].IconId);
            Assert.AreEqual(1f, skillTable.Skills[SkillId.Summoning].LearnMod);
            Assert.AreEqual(120, skillTable.Skills[SkillId.Summoning].LowerBound);
            Assert.AreEqual(900, skillTable.Skills[SkillId.Summoning].UpperBound);
            Assert.AreEqual(2u, skillTable.Skills[SkillId.Summoning].MinLevel);
            Assert.AreEqual(12, skillTable.Skills[SkillId.Summoning].SpecializedCost);
            Assert.AreEqual(8, skillTable.Skills[SkillId.Summoning].TrainedCost);
            Assert.AreEqual(1, skillTable.Skills[SkillId.Summoning].Formula.Attribute1Multiplier);
            Assert.AreEqual(1, skillTable.Skills[SkillId.Summoning].Formula.Attribute2Multiplier);
            Assert.AreEqual(3, skillTable.Skills[SkillId.Summoning].Formula.Divisor);
            Assert.AreEqual(AttributeId.Endurance, skillTable.Skills[SkillId.Summoning].Formula.Attribute1);
            Assert.AreEqual(AttributeId.Self, skillTable.Skills[SkillId.Summoning].Formula.Attribute2);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<SkillTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0xE000004u);
        }
    }
}
