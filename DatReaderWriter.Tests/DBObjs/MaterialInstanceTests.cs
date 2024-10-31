using DatReaderWriter.Tests.Lib;
using ACClientLib.DatReaderWriter;
using ACClientLib.DatReaderWriter.Options;
using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.DBObjs;
using ACClientLib.DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class MaterialInstanceTests {
        [TestMethod]
        public void CanInsertAndReadMaterialInstances() {
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORMaterialInstances() {
            using var dat = new DatDatabaseReader(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });

            foreach (var f in dat.Tree) {
                if (f.Id >= 0x18000000 && f.Id <= 0x18FFFFFF) {
                    Console.WriteLine($"{f.Id:X8}  (bytes: {f.Size})");
                    var materialInstance = dat.TryReadFile<MaterialInstance>(f.Id, out var mi);
                    Console.WriteLine($"MaterialInstance: 0x{mi.Id:X8}");
                    Console.WriteLine($"\t MaterialId: 0x{mi.MaterialId:X8}");
                    Console.WriteLine($"\t Type: {mi.MaterialType}");
                    Console.WriteLine($"\t ModifierRefs: [ {string.Join(",", mi.ModifierRefs.Select(v => $"0x{v:X8}"))} ]");
                    Console.WriteLine($"\t AllowStencelShadows: {mi.AllowStencilShadows}");
                    Console.WriteLine($"\t WantDiscardGeometry: {mi.WantDiscardGeometry}");
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
