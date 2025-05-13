using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class MasterInputMapTests {
        [TestMethod]
        public void CanInsertAndRead() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeObj = new MasterInputMap() {
                Id = 0x14000000u,
                Devices = [
                    new DeviceKeyMapEntry() { Type = DeviceType.Keyboard, Guid = new Guid("FEEDBEEF-1234-5678-90AB-CDEF12345678") }
                ],
                GuidMap = new Guid("FEEDBEEF-1234-5678-90AB-CDEF12345678"),
                InputMaps = {
                    { 1, new CInputMap() { Mappings = { new QualifiedControl() { Activation = 0x01u, Key = new ControlSpecification() { Key = 0x02u, Modifier = 0x03u }},  } } }
                },
                MetaKeys = [
                    new ControlSpecification() { Key = 0x01u, Modifier = 0x02u }
                ],
                Name = "Test Name",
            };

            var res = dat.TryWriteFile(writeObj);
            Assert.IsTrue(res);

            var res2 = dat.TryGet<MasterInputMap>(0x14000000u, out var readObj);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readObj);

            Assert.AreEqual(0x14000000u, readObj.Id);

            Assert.AreEqual(1, readObj.Devices.Count);
            Assert.AreEqual(DeviceType.Keyboard, readObj.Devices[0].Type);
            Assert.AreEqual(new Guid("FEEDBEEF-1234-5678-90AB-CDEF12345678"), readObj.Devices[0].Guid);

            Assert.AreEqual(1, readObj.InputMaps.Count);
            Assert.AreEqual(1, readObj.InputMaps[1].Mappings.Count);
            Assert.AreEqual(0x01u, readObj.InputMaps[1].Mappings[0].Activation);
            Assert.AreEqual(0x02u, readObj.InputMaps[1].Mappings[0].Key.Key);
            Assert.AreEqual(0x03u, readObj.InputMaps[1].Mappings[0].Key.Modifier);

            Assert.AreEqual(1, readObj.MetaKeys.Count);
            Assert.AreEqual(0x01u, readObj.MetaKeys[0].Key);
            Assert.AreEqual(0x02u, readObj.MetaKeys[0].Modifier);

            Assert.AreEqual("Test Name", readObj.Name);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORKeymaps() {
            using var dat = new PortalDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });

            dat.Tree.TryGetFile(0x14000000u, out var readObj);
            Console.WriteLine($"{readObj.Id:X8}  (bytes: {readObj.Size})");

            dat.TryGetFileBytes(readObj.Id, out var keymapBytes);
            var inputMap = new MasterInputMap();
            var reader = new DatBinReader(keymapBytes);
            inputMap.Unpack(reader);

            Console.WriteLine($"Name: {inputMap.Name}");
            Console.WriteLine($"GUID: {inputMap.GuidMap}");
            Console.WriteLine($"Device Count: {inputMap.Devices.Count}");
            foreach (var device in inputMap.Devices) {
                Console.WriteLine($"\tDevice: {device.Guid} {device.Type}");
            }
            Console.WriteLine($"Meta Key Count: {inputMap.MetaKeys.Count}");
            foreach (var metaKey in inputMap.MetaKeys) {
                Console.WriteLine($"\tMeta Key: {metaKey.Key} {metaKey.Modifier:X8}");
            }
            Console.WriteLine($"Section Count: {inputMap.InputMaps.Count}");

            foreach (var section in inputMap.InputMaps) {
                Console.WriteLine($"\tSection: {section.Key:X8}");
                Console.WriteLine($"\t\t m_eInputMapID: {section.Value.Mappings.Count}");
            }

            Console.WriteLine($"Read {reader.Offset}/{reader.Length} bytes.");

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<MasterInputMap>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x14000000u);
            TestHelpers.CanReadAndWriteIdentical<MasterInputMap>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x14000002u);
        }
    }
}
