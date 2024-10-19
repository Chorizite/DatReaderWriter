using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.IO;
using ACClientLib.DatReaderWriter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests.Types {
    [TestClass]
    public class CreateParticleHookTests {
        [TestMethod]
        public void HasCorrectHookType() {
            var obj = new CreateParticleHook();

            Assert.AreEqual(AnimationHookType.CreateParticle, obj.HookType);
        }

        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeHook = new CreateParticleHook() {
                Direction = AnimationHookDir.Forward,
                EmitterId = 123,
                EmitterInfoId = 456,
                Offset = new Frame() {
                    Origin = Vector3.UnitZ,
                    Orientation = Quaternion.Identity
                },
                PartIndex = 789
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeHook.Pack(writer);

            var readHook = new CreateParticleHook();
            var reader = new DatFileReader(buffer);
            readHook.Unpack(reader);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.EmitterId, readHook.EmitterId);
            Assert.AreEqual(writeHook.EmitterInfoId, readHook.EmitterInfoId);
            Assert.AreEqual(writeHook.PartIndex, readHook.PartIndex);
            Assert.AreEqual(writeHook.Offset.Origin, readHook.Offset.Origin);
            Assert.AreEqual(writeHook.Offset.Orientation, readHook.Offset.Orientation);
        }

        [TestMethod]
        public void CanBeCreatedByAbstractUnpack() {
            var writeHook = new CreateParticleHook() {
                Direction = AnimationHookDir.Forward,
                EmitterId = 123,
                EmitterInfoId = 456,
                Offset = new Frame() {
                    Origin = Vector3.UnitZ,
                    Orientation = Quaternion.Identity
                },
                PartIndex = 789
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeHook.Pack(writer);

            var reader = new DatFileReader(buffer);
            var readObj = AnimationHook.Unpack(reader, writeHook.HookType);

            Assert.IsNotNull(readObj);
            Assert.IsInstanceOfType(readObj, typeof(CreateParticleHook));

            var readHook = readObj as CreateParticleHook;
            Assert.IsNotNull(readHook);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.EmitterId, readHook.EmitterId);
            Assert.AreEqual(writeHook.EmitterInfoId, readHook.EmitterInfoId);
            Assert.AreEqual(writeHook.PartIndex, readHook.PartIndex);
            Assert.AreEqual(writeHook.Offset.Origin, readHook.Offset.Origin);
            Assert.AreEqual(writeHook.Offset.Orientation, readHook.Offset.Orientation);
        }
    }
}
