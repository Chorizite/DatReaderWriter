using DatReaderWriter.Enums;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Types;
using DatReaderWriter.Tests.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests.Types {
    [TestClass]
    public class SpellBaseTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
#if NET8_0_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            var writeObj = new SpellBase() {
                Name = "Test",
                Description = "This is a description",
                School = MagicSchool.ItemEnchantment,
                Icon = 0x12345678,
                Category = SpellCategory.StrengthRaising,
                Bitfield = SpellIndex.Resistable,
                BaseMana = 103,
                BaseRangeConstant = 1.1f,
                BaseRangeMod = 0.5f,
                Power = 500,
                SpellEconomyMod = 0.3f,
                FormulaVersion = 1,
                ComponentLoss = 0.2f,
                MetaSpellType = SpellType.Enchantment,
                MetaSpellId = 0x87654321,
                RecoveryAmount = 1,
                CasterEffect = PlayScript.Launch,
                DegradeLimit = 1,
                DegradeModifier = 0.5f,
                DisplayOrder = 123,
                Duration = 2.2,
                FizzleEffect = PlayScript.Fizzle,
                ManaMod = 5, 
                NonComponentTargetType = ItemType.Armor,
                PortalLifetime = 0,
                Components = [1, 2, 3, 4, 5, 6, 7, 8],
                RecoveryInterval = 0.1,
                TargetEffect = PlayScript.Explode
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new SpellBase();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Name, readObj.Name);
            Assert.AreEqual(writeObj.Description, readObj.Description);
            Assert.AreEqual(writeObj.School, readObj.School);
            Assert.AreEqual(writeObj.Icon, readObj.Icon);
            Assert.AreEqual(writeObj.Category, readObj.Category);
            Assert.AreEqual(writeObj.Bitfield, readObj.Bitfield);
            Assert.AreEqual(writeObj.BaseMana, readObj.BaseMana);
            Assert.AreEqual(writeObj.BaseRangeConstant, readObj.BaseRangeConstant);
            Assert.AreEqual(writeObj.BaseRangeMod, readObj.BaseRangeMod);
            Assert.AreEqual(writeObj.Power, readObj.Power);
            Assert.AreEqual(writeObj.SpellEconomyMod, readObj.SpellEconomyMod);
            Assert.AreEqual(writeObj.FormulaVersion, readObj.FormulaVersion);
            Assert.AreEqual(writeObj.ComponentLoss, readObj.ComponentLoss);
            Assert.AreEqual(writeObj.MetaSpellType, readObj.MetaSpellType);
            Assert.AreEqual(writeObj.MetaSpellId, readObj.MetaSpellId);
            Assert.AreEqual(writeObj.RecoveryAmount, readObj.RecoveryAmount);
            Assert.AreEqual(writeObj.CasterEffect, readObj.CasterEffect);
            Assert.AreEqual(writeObj.DegradeLimit, readObj.DegradeLimit);
            Assert.AreEqual(writeObj.DegradeModifier, readObj.DegradeModifier);
            Assert.AreEqual(writeObj.DisplayOrder, readObj.DisplayOrder);
            Assert.AreEqual(writeObj.Duration, readObj.Duration);
            Assert.AreEqual(writeObj.FizzleEffect, readObj.FizzleEffect);
            Assert.AreEqual(writeObj.ManaMod, readObj.ManaMod);
            Assert.AreEqual(writeObj.NonComponentTargetType, readObj.NonComponentTargetType);
            Assert.AreEqual(writeObj.PortalLifetime, readObj.PortalLifetime);
            CollectionAssert.AreEqual(writeObj.Components, readObj.Components);
            Assert.AreEqual(writeObj.RecoveryInterval, readObj.RecoveryInterval);
            Assert.AreEqual(writeObj.TargetEffect, readObj.TargetEffect);
        }
    }
}
