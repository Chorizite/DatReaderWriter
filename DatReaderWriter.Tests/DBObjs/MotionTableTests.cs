using DatReaderWriter.Tests.Lib;
using ACClientLib.DatReaderWriter;
using ACClientLib.DatReaderWriter.Options;
using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.DBObjs;
using ACClientLib.DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class MotionTableTests {
        [TestMethod]
        public void CanInsertAndReadMotionTables() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabaseReader(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatDatabaseType.Portal, 0);

            var writeAnim = new MotionTable() {
                Id = 0x09000001,
                DefaultStyle = MotionCommand.Cancel,
                Cycles = new Dictionary<int, MotionData>() {
                    { 1, new MotionData(){
                        Anims = [
                            new AnimData(){ AnimId = 1, Framerate = 30, HighFrame = -1, LowFrame = 0 } 
                        ],
                        Flags = MotionDataFlags.HasVelocity,
                        Velocity = Vector3.UnitZ
                    }  }
                }
            };

            var res = dat.TryWriteFile(writeAnim);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<MotionTable>(0x09000001, out var readMTable);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readMTable);

            Assert.AreEqual(0x09000001u, readMTable.Id);

            Assert.AreEqual(MotionCommand.Cancel, readMTable.DefaultStyle);
            Assert.AreEqual(1, readMTable.Cycles.Count);
            Assert.AreEqual(1, readMTable.Cycles.First().Key);
            Assert.AreEqual(MotionDataFlags.HasVelocity, readMTable.Cycles.First().Value.Flags);
            Assert.AreEqual(Vector3.UnitZ, readMTable.Cycles.First().Value.Velocity);


            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORMotionTables() {
            using var dat = new DatDatabaseReader(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });


            var res = dat.TryReadFile<MotionTable>(0x09000202, out var mTable);
            Assert.IsTrue(res);
            Assert.IsNotNull(mTable);
            Assert.AreEqual(0x09000202u, mTable.Id);

            Assert.AreEqual(MotionCommand.NonCombat, mTable.DefaultStyle);

            Assert.AreEqual(1, mTable.StyleDefaults.Count);
            Assert.AreEqual(MotionCommand.NonCombat, mTable.StyleDefaults.First().Key);
            Assert.AreEqual(MotionCommand.Off, mTable.StyleDefaults.First().Value);

            dat.Dispose();
        }
    }
}
