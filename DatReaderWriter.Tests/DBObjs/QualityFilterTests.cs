using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;
using DatReaderWriter.Lib.IO.DatBTree;
using DatReaderWriter.Lib.IO.BlockAllocators;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class QualityFilterTests {
        [TestMethod]
        public void CanInsertAndRead() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new QualityFilter() {
                Id = 0x0E010000u,
                Attribute2ndStatFilter = [0x00000001u, 0x00000002u],
                AttributeStatFilter = [0x00000003u, 0x00000004u],
                BoolStatFilter = [0x00000005u, 0x00000006u],
                DataIdStatFilter = [0x00000007u, 0x00000008u],
                FloatStatFilter = [0x00000009u, 0x0000000Au],
                InstanceIdStatFilter = [0x0000000Bu, 0x0000000Cu],
                Int64StatFilter = [0x0000000Du, 0x0000000Eu],
                IntStatFilter = [0x0000000Fu, 0x00000010u],
                PositionStatFilter = [0x00000011u, 0x00000012u],
                SkillStatFilter = [0x00000013u, 0x00000014u],
                StringStatFilter = [0x00000015u, 0x00000016u]
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<QualityFilter>(0x0E010000u, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x0E010000u, readObj.Id);

            Assert.AreEqual(2, readObj.Attribute2ndStatFilter.Count());
            Assert.AreEqual(0x00000001u, readObj.Attribute2ndStatFilter[0]);
            Assert.AreEqual(0x00000002u, readObj.Attribute2ndStatFilter[1]);

            Assert.AreEqual(2, readObj.AttributeStatFilter.Count());
            Assert.AreEqual(0x00000003u, readObj.AttributeStatFilter[0]);
            Assert.AreEqual(0x00000004u, readObj.AttributeStatFilter[1]);

            Assert.AreEqual(2, readObj.BoolStatFilter.Count());
            Assert.AreEqual(0x00000005u, readObj.BoolStatFilter[0]);
            Assert.AreEqual(0x00000006u, readObj.BoolStatFilter[1]);

            Assert.AreEqual(2, readObj.DataIdStatFilter.Count());
            Assert.AreEqual(0x00000007u, readObj.DataIdStatFilter[0]);
            Assert.AreEqual(0x00000008u, readObj.DataIdStatFilter[1]);

            Assert.AreEqual(2, readObj.FloatStatFilter.Count());
            Assert.AreEqual(0x00000009u, readObj.FloatStatFilter[0]);
            Assert.AreEqual(0x0000000Au, readObj.FloatStatFilter[1]);

            Assert.AreEqual(2, readObj.InstanceIdStatFilter.Count());
            Assert.AreEqual(0x0000000Bu, readObj.InstanceIdStatFilter[0]);
            Assert.AreEqual(0x0000000Cu, readObj.InstanceIdStatFilter[1]);

            Assert.AreEqual(2, readObj.Int64StatFilter.Count());
            Assert.AreEqual(0x0000000Du, readObj.Int64StatFilter[0]);
            Assert.AreEqual(0x0000000Eu, readObj.Int64StatFilter[1]);

            Assert.AreEqual(2, readObj.IntStatFilter.Count());
            Assert.AreEqual(0x0000000Fu, readObj.IntStatFilter[0]);
            Assert.AreEqual(0x00000010u, readObj.IntStatFilter[1]);

            Assert.AreEqual(2, readObj.PositionStatFilter.Count());
            Assert.AreEqual(0x00000011u, readObj.PositionStatFilter[0]);
            Assert.AreEqual(0x00000012u, readObj.PositionStatFilter[1]);

            Assert.AreEqual(2, readObj.SkillStatFilter.Count());
            Assert.AreEqual(0x00000013u, readObj.SkillStatFilter[0]);
            Assert.AreEqual(0x00000014u, readObj.SkillStatFilter[1]);

            Assert.AreEqual(2, readObj.StringStatFilter.Count());
            Assert.AreEqual(0x00000015u, readObj.StringStatFilter[0]);
            Assert.AreEqual(0x00000016u, readObj.StringStatFilter[1]);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEOR() {
            using var dat = new PortalDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });

            var res = dat.TryReadFile<QualityFilter>(0x0E010001u, out var rt1);
            Assert.IsTrue(res);
            Assert.IsNotNull(rt1);
            Assert.AreEqual(0x0E010001u, rt1.Id);

            Assert.AreEqual(3, rt1.Attribute2ndStatFilter.Count());
            Assert.AreEqual(0x00000001u, rt1.Attribute2ndStatFilter[0]);
            Assert.AreEqual(0x00000003u, rt1.Attribute2ndStatFilter[1]);
            Assert.AreEqual(0x00000005u, rt1.Attribute2ndStatFilter[2]);

            Assert.AreEqual(0, rt1.AttributeStatFilter.Count());
            Assert.AreEqual(0, rt1.StringStatFilter.Count());

            Assert.AreEqual(47, rt1.SkillStatFilter.Count());
            Assert.AreEqual(1u, rt1.SkillStatFilter[0]);
            Assert.AreEqual(2u, rt1.SkillStatFilter[1]);
            Assert.AreEqual(53u, rt1.SkillStatFilter[45]);
            Assert.AreEqual(54u, rt1.SkillStatFilter[46]);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<QualityFilter>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x0E010001u);
            TestHelpers.CanReadAndWriteIdentical<QualityFilter>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x0E010002u);
        }
    }
}
