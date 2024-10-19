using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.IO;
using ACClientLib.DatReaderWriter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests.Types {
    [TestClass]
    public class CreateBlockingParticleHookTests {
        [TestMethod]
        public void HasCorrectHookType() {
            var obj = new CreateBlockingParticleHook();

            Assert.AreEqual(AnimationHookType.CreateBlockingParticle, obj.HookType);
        }

        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeHook = new CreateBlockingParticleHook() {
                Direction = AnimationHookDir.Forward
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeHook.Pack(writer);

            var readHook = new CreateBlockingParticleHook();
            var reader = new DatFileReader(buffer);
            readHook.Unpack(reader);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
        }

        [TestMethod]
        public void CanBeCreatedByAbstractUnpack() {
            var writeHook = new CreateBlockingParticleHook() {
                Direction = AnimationHookDir.Forward
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeHook.Pack(writer);

            var reader = new DatFileReader(buffer);
            var readHook = AnimationHook.Unpack(reader, writeHook.HookType);

            Assert.IsNotNull(readHook);
            Assert.IsInstanceOfType(readHook, typeof(CreateBlockingParticleHook));
        }
    }
}
