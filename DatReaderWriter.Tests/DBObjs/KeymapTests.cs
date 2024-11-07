using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class KeymapTests {
        [TestMethod]
        public void CanInsertAndReadKeymaps() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var writeAnim = new Animation() {
                Id = 0x03000001,
                NumParts = 1,
                PartFrames = [
                  new AnimationFrame(1) {
                      Frames = [
                        new Frame() {
                          Origin = new Vector3(1, 2, 3),
                          Orientation = new Quaternion(0, 0, 0, 1)
                        }
                      ],
                      Hooks = [
                        new AnimationDoneHook() { Direction = AnimationHookDir.Backward }
                      ]
                  }
                ]
            };

            var res = dat.TryWriteFile(writeAnim);
            Assert.IsTrue(res);

            var res2 = dat.TryReadFile<Animation>(0x03000001, out var readAnim);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readAnim);

            Assert.AreEqual(0x03000001u, readAnim.Id);

            Assert.AreEqual(1u, readAnim.NumParts);
            Assert.AreEqual(1, readAnim.PartFrames.Count);
            Assert.AreEqual(1, readAnim.PartFrames[0].Frames.Count);
            Assert.AreEqual(1, readAnim.PartFrames[0].Hooks.Count);

            Assert.AreEqual(new Vector3(1, 2, 3), readAnim.PartFrames[0].Frames[0].Origin);
            Assert.AreEqual(new Quaternion(0, 0, 0, 1), readAnim.PartFrames[0].Frames[0].Orientation);
            Assert.AreEqual(AnimationHookDir.Backward, readAnim.PartFrames[0].Hooks[0].Direction);

            Assert.IsInstanceOfType(readAnim.PartFrames[0].Hooks[0], typeof(AnimationDoneHook));

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

            foreach (var keymap in dat.Keymaps) {
                Console.WriteLine($"{keymap.Id:X8}");
                Console.WriteLine($"\t Name: {keymap.Name}");
                Console.WriteLine($"\t GuidMap: {keymap.GuidMap}");

                Console.WriteLine($"\t DeviceKeyMapEntries:");
                var i = 0;
                foreach (var entry in keymap.DeviceKeyMapEntries) {
                    Console.WriteLine($"\t\t DeviceKeyMapEntry[{i++}]");
                    Console.WriteLine($"\t\t\t Type: {entry.Type}");
                    Console.WriteLine($"\t\t\t Guid: {entry.Guid}");
                }

                Console.WriteLine($"\t MetaKeys:");
                i = 0;
                foreach (var entry in keymap.MetaKeys) {
                    Console.WriteLine($"\t\t MetaKey[{i++}]");
                    Console.WriteLine($"\t\t\t DeviceIdx: {entry.DeviceIndex}");
                    Console.WriteLine($"\t\t\t SubControl: {entry.SubControl}");
                    Console.WriteLine($"\t\t\t Key: {entry.Key:X4}");
                    Console.WriteLine($"\t\t\t Modifiers: {entry.Modifiers:X8}");
                }

                Console.WriteLine($"\t UserBindings:");
                i = 0;
                foreach (var entry in keymap.UserBindings) {
                    Console.WriteLine($"\t\t UserBinding[{i++}]");
                    Console.WriteLine($"\t\t\t\t ActionClass: {entry.ActionClass}");
                    Console.WriteLine($"\t\t\t\t ActionName: {entry.ActionName}");
                    Console.WriteLine($"\t\t\t\t ControlSpec:");
                    Console.WriteLine($"\t\t\t\t\t DeviceIdx: {entry.ControlSpec.DeviceIndex}");
                    Console.WriteLine($"\t\t\t\t\t SubControl: {entry.ControlSpec.SubControl}");
                    Console.WriteLine($"\t\t\t\t\t Key: {entry.ControlSpec.Key:X4}");
                    Console.WriteLine($"\t\t\t\t\t Modifiers: {entry.ControlSpec.Modifiers:X8}");
                }
            }

            dat.Dispose();
        }
    }
}
