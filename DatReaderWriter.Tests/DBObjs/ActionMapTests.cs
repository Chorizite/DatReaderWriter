using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class ActionMapTests {
        [TestMethod]
        public void CanInsertAndReadActionMaps() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var actionMap = new ActionMap() {
                Id = 0x26000000u,
                StringTableId = 0x23000005u,
                InputMaps = new() {
                    { 0, new() {
                        { 0, new ActionMapValue(){ Magic = 1 }}
                    } }
                },
                ConflictingMaps = new() {
                    { 0, new InputsConflictsValue() {InputMap = 1, ConflictingInputMaps = [1, 2]}}
                }
            };

            var res =  dat.TryWriteFile(actionMap);

            var res2 = dat.TryGet<ActionMap>(0x26000000u, out var readActionMap);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readActionMap);

            Assert.AreEqual(0x23000005u, readActionMap.StringTableId);
            Assert.AreEqual(1, readActionMap.InputMaps.Count);
            Assert.AreEqual(1, readActionMap.InputMaps[0].Count);
            Assert.AreEqual(1u, readActionMap.InputMaps[0][0].Magic);
            Assert.AreEqual(1, readActionMap.ConflictingMaps.Count);
            Assert.AreEqual(1u, readActionMap.ConflictingMaps[0].InputMap);
            CollectionAssert.AreEqual(new List<int> { 1, 2 }, readActionMap.ConflictingMaps[0].ConflictingInputMaps);

            dat.Dispose();
            File.Delete(datFilePath);
        }
        
        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEOR() {
            using var dat = new DatCollection(EORCommonData.DatDirectory);

            var res = dat.TryGet<ActionMap>(0x26000000u, out var actionMap);
            Assert.IsTrue(res);
            Assert.IsNotNull(actionMap);
            Assert.AreEqual(0x26000000u, actionMap.Id);

            Assert.AreEqual(0x23000005u, actionMap.StringTableId);
            Assert.AreEqual(27, actionMap.InputMaps.Count);
            Assert.AreEqual(16, actionMap.ConflictingMaps.Count);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<ActionMap>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x26000000u);
        }
    }
}
