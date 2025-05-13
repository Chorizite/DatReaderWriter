using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;
using DatReaderWriter.Lib.IO.DatBTree;
using DatReaderWriter.Lib.IO.BlockAllocators;
using DatReaderWriter.Tests.Lib;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class LanguageInfoTests {
        [TestMethod]
        public void CanInsertAndRead() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new LanguageInfo() {
                Id = 0x41000000u,
                AdditionalFlags = 0,
                AdditionalSettings = "hello world",
                Base = 1,
                CandColorBase = 2,
                CandColorBorder = 3,
                CandColorText = 4,
                CompColorConverted = 5,
                CompColorInput = 6,
                CompColorInputErr = 7,
                CompColorTargetConv = 8,
                CompColorTargetNotConv = 9,
                CompColorText = 10,
                CompTranslucence = 11,
                DecimalSeperator = ".",
                FemalePlayerLetters = "abc",
                MalePlayerLetters = "def",
                GroupingSeperator = ",",
                GroupingSize = 3,
                IMEEnabledSetting = 0,
                IsNegativeOneSingular = false,
                IsNegativeTwoOrLessSingular = false,
                IsOneSingular = true,
                IsTwoOrMoreSingular = true,
                IsZeroSingular = true,
                LeadingZero = false,
                NegativeNumberFormat = "-%s",
                NumDecimalDigits = 2,
                Numerals = "0123456789",
                OtherIME = 0,
                SymbolColor = 1,
                SymbolColorText = 2,
                SymbolHeight = 3,
                SymbolPlacement = 4,
                SymbolTranslucence = 5,
                TreasureMiddleLetters = "ghi",
                TreasurePrefixLetters = "jkl",
                TreasureSuffixLetters = "mno",
                Version = 1,
                WordWrapOnSpace = 1


            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryGet<LanguageInfo>(0x41000000u, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(writeObj.AdditionalFlags, readObj.AdditionalFlags);
            Assert.AreEqual(writeObj.AdditionalSettings, readObj.AdditionalSettings);
            Assert.AreEqual(writeObj.Base, readObj.Base);
            Assert.AreEqual(writeObj.CandColorBase, readObj.CandColorBase);
            Assert.AreEqual(writeObj.CandColorBorder, readObj.CandColorBorder);
            Assert.AreEqual(writeObj.CandColorText, readObj.CandColorText);
            Assert.AreEqual(writeObj.CompColorConverted, readObj.CompColorConverted);
            Assert.AreEqual(writeObj.CompColorInput, readObj.CompColorInput);
            Assert.AreEqual(writeObj.CompColorInputErr, readObj.CompColorInputErr);
            Assert.AreEqual(writeObj.CompColorTargetConv, readObj.CompColorTargetConv);
            Assert.AreEqual(writeObj.CompColorTargetNotConv, readObj.CompColorTargetNotConv);
            Assert.AreEqual(writeObj.CompColorText, readObj.CompColorText);
            Assert.AreEqual(writeObj.CompTranslucence, readObj.CompTranslucence);
            Assert.AreEqual(writeObj.DecimalSeperator, readObj.DecimalSeperator);
            Assert.AreEqual(writeObj.FemalePlayerLetters, readObj.FemalePlayerLetters);
            Assert.AreEqual(writeObj.MalePlayerLetters, readObj.MalePlayerLetters);
            Assert.AreEqual(writeObj.GroupingSeperator, readObj.GroupingSeperator);
            Assert.AreEqual(writeObj.GroupingSize, readObj.GroupingSize);
            Assert.AreEqual(writeObj.IMEEnabledSetting, readObj.IMEEnabledSetting);
            Assert.AreEqual(writeObj.IsNegativeOneSingular, readObj.IsNegativeOneSingular);
            Assert.AreEqual(writeObj.IsNegativeTwoOrLessSingular, readObj.IsNegativeTwoOrLessSingular);
            Assert.AreEqual(writeObj.IsOneSingular, readObj.IsOneSingular);
            Assert.AreEqual(writeObj.IsTwoOrMoreSingular, readObj.IsTwoOrMoreSingular);
            Assert.AreEqual(writeObj.IsZeroSingular, readObj.IsZeroSingular);
            Assert.AreEqual(writeObj.LeadingZero, readObj.LeadingZero);
            Assert.AreEqual(writeObj.NegativeNumberFormat, readObj.NegativeNumberFormat);
            Assert.AreEqual(writeObj.NumDecimalDigits, readObj.NumDecimalDigits);
            Assert.AreEqual(writeObj.Numerals, readObj.Numerals);
            Assert.AreEqual(writeObj.OtherIME, readObj.OtherIME);
            Assert.AreEqual(writeObj.SymbolColor, readObj.SymbolColor);
            Assert.AreEqual(writeObj.SymbolColorText, readObj.SymbolColorText);
            Assert.AreEqual(writeObj.SymbolHeight, readObj.SymbolHeight);
            Assert.AreEqual(writeObj.SymbolPlacement, readObj.SymbolPlacement);
            Assert.AreEqual(writeObj.SymbolTranslucence, readObj.SymbolTranslucence);
            Assert.AreEqual(writeObj.TreasureMiddleLetters, readObj.TreasureMiddleLetters);
            Assert.AreEqual(writeObj.TreasurePrefixLetters, readObj.TreasurePrefixLetters);
            Assert.AreEqual(writeObj.TreasureSuffixLetters, readObj.TreasureSuffixLetters);
            Assert.AreEqual(writeObj.Version, readObj.Version);
            Assert.AreEqual(writeObj.WordWrapOnSpace, readObj.WordWrapOnSpace);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEOR() {
            using var dat = new LocalDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_local_English.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });

            var res = dat.TryGet<LanguageInfo>(0x41000000u, out var rt1);
            Assert.IsTrue(res);
            Assert.IsNotNull(rt1);

            Assert.AreEqual(0xCDCDCDCDu, rt1.AdditionalFlags);
            Assert.AreEqual("", rt1.AdditionalSettings);
            Assert.AreEqual(10u, rt1.Base);
            Assert.AreEqual(0xFFFFFFFEu, rt1.CandColorBase);
            Assert.AreEqual(0xFF000001u, rt1.CandColorBorder);
            Assert.AreEqual(0u, rt1.CandColorText);
            Assert.AreEqual(0xFF00u, rt1.CompColorConverted);
            Assert.AreEqual(0xFFFF00u, rt1.CompColorInput);
            Assert.AreEqual(0x00FF0000u, rt1.CompColorInputErr);
            Assert.AreEqual(0xFFu, rt1.CompColorTargetConv);
            Assert.AreEqual(0x00FF0000u, rt1.CompColorTargetNotConv);
            Assert.AreEqual(0x00000001u, rt1.CompColorText);
            Assert.AreEqual(0x00000080u, rt1.CompTranslucence);
            Assert.AreEqual(".", rt1.DecimalSeperator);
            Assert.AreEqual("fn", rt1.FemalePlayerLetters);
            Assert.AreEqual("mn", rt1.MalePlayerLetters);
            Assert.AreEqual(",", rt1.GroupingSeperator);
            Assert.AreEqual(3u, rt1.GroupingSize);


            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<LanguageInfo>(Path.Combine(EORCommonData.DatDirectory, $"client_local_English.dat"), 0x41000000u);
        }
    }
}
