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
                /*
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
                */
            }

            dat.Dispose();
        }
    }
}
