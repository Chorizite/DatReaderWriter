using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class AnimationTests {
        [TestMethod]
        public void CanInsertAndReadAnimations() {
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

            var res2 = dat.TryGet<Animation>(0x03000001, out var readAnim);
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
        public void CanReadEORAnimations() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.Never;
            });

            var res = dat.TryGet<Animation>(0x03000514, out var anim);
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

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            var dats = new DatCollection(EORCommonData.DatDirectory);
            var allFiles = dats.Portal.Tree.GetFilesInRange(0x16000000, 0x16FFFFFF);

            foreach (var file in allFiles) {
                dats.Portal.TryGetFileBytes(file.Id, out var bytes);
                Console.WriteLine($"0x{file.Id:X8}  {file.Size}");
                Console.WriteLine(string.Join(" ", bytes.Select(b => $"{b:X2}")));
            }

            TestHelpers.CanReadAndWriteIdentical<Animation>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x03000DD8);
            TestHelpers.CanReadAndWriteIdentical<Animation>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x03000514);
            TestHelpers.CanReadAndWriteIdentical<Animation>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x03000968);
        }
    }
}
