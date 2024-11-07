using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class MaterialModifierTests {
        [TestMethod]
        public void CanInsertAndReadMaterialModifiers() {
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORMaterialModifiers() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });

            foreach (var f in dat.Tree) {
                if (f.Id >= 0x17000000 && f.Id <= 0x17FFFFFF) {
                    Console.WriteLine($"{f.Id:X8}  (bytes: {f.Size})");
                    var materialInstance = dat.TryReadFile<MaterialModifier>(f.Id, out var mm);
                    Console.WriteLine($"MaterialModifier: 0x{mm.Id:X8}");
                    foreach (var prop in mm.MaterialProperties) {
                        Console.WriteLine($"\t Prop:");
                        Console.WriteLine($"\t\t NameId: {prop.NameId}");
                        Console.WriteLine($"\t\t DataType: {prop.DataType}");
                        Console.WriteLine($"\t\t DataLength: {prop.DataLength:X8}");
                        Console.WriteLine($"\t\t DataLength2: {prop.DataLength2:X8}");
                        Console.WriteLine($"\t\t DataLength3: {prop.DataLength3:X4}");
                        Console.WriteLine($"\t\t DataLength4: {prop.DataLength3:X2}");
                    }
                }
            }

            return;

            var res = dat.TryReadFile<Animation>(0x03000514, out var anim);
            Assert.IsTrue(res);
            Assert.IsNotNull(anim);
            Assert.AreEqual(0x03000514u, anim.Id);

            Assert.AreEqual(17u, anim.NumParts);
            Assert.AreEqual(44, anim.PartFrames.Count);
            Assert.AreEqual(1, anim.PartFrames[0].Hooks.Count);

            Assert.IsInstanceOfType(anim.PartFrames[0].Hooks[0], typeof(SoundTableHook));
            var soundTableHook = (SoundTableHook)anim.PartFrames[0].Hooks[0];
            Assert.IsNotNull(soundTableHook);
            Assert.AreEqual(AnimationHookDir.Forward, soundTableHook.Direction);
            Assert.AreEqual(Sound.Death1, soundTableHook.SoundType);

            dat.Dispose();
        }
    }
}
