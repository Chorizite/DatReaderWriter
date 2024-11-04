using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.IO;
using ACClientLib.DatReaderWriter.Types;
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
                Bitfield = 0x01,
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
                CasterEffect = 2,
                DegradeLimit = 1,
                DegradeModifier = 0.5f,
                DisplayOrder = 123,
                Duration = 2.2,
                FizzleEffect = 4,
                ManaMod = 5, 
                NonComponentTargetType = 1234,
                PortalLifetime = 0,
                RawComponents = [1, 2, 3, 4, 5, 6, 7, 8],
                RecoveryInterval = 0.1,
                TargetEffect = 0x12345678
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new SpellBase();
            var reader = new DatFileReader(buffer);
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
            CollectionAssert.AreEqual(writeObj.RawComponents, readObj.RawComponents);
            Assert.AreEqual(writeObj.RecoveryInterval, readObj.RecoveryInterval);
            Assert.AreEqual(writeObj.TargetEffect, readObj.TargetEffect);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanEncryptAndDecryptComponents(
            [DataValues(
                new uint[8] { 1u, 2u, 3u, 4u, 5u, 6u, 7u, 8u },
                new uint [8] { 81u, 71u, 61u, 51u, 41u, 0u, 0u, 0u }
            )] uint[] components
            ) {

            var spellComponentBase = new SpellBase() {
                Name = "Test",
                Description = "Test text",
            };

            spellComponentBase.SetRawComponents(components);

            CollectionAssert.AreEquivalent(components, spellComponentBase.DecryptedComponents());
        }
    }
}
