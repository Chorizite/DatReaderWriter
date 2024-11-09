using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;
using System.Text;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class PhysicsScriptTests {
        [TestMethod]
        public void CanInsertAndReadPhysicsScript() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new PhysicsScript() {
                Id = 0x33000001,
                ScriptData = [
                    new PhysicsScriptData(){
                        StartTime = 1.1,
                        Hook = new AnimationDoneHook() { Direction = AnimationHookDir.Forward },
                    }
                ]
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<PhysicsScript>(0x33000001, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x33000001u, readObj.Id);

            Assert.IsInstanceOfType(readObj.ScriptData[0].Hook, typeof(AnimationDoneHook));
            Assert.AreEqual(1.1, readObj.ScriptData[0].StartTime);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORPhysicsScriptData() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });


            var res = dat.TryReadFile<PhysicsScript>(0x33000007u, out var readObj);
            Assert.IsTrue(res);
            Assert.IsNotNull(readObj);
            Assert.AreEqual(0x33000007u, readObj.Id);

            Assert.IsInstanceOfType(readObj.ScriptData[0].Hook, typeof(CreateParticleHook));

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORClothingAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<PhysicsScript>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x33000007);
            TestHelpers.CanReadAndWriteIdentical<PhysicsScript>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x33000008);
            TestHelpers.CanReadAndWriteIdentical<PhysicsScript>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x33000009);
        }
    }
}
