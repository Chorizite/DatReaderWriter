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
    public class SetLightHookTests {
        [TestMethod]
        public void HasCorrectHookType() {
            var obj = new SetLightHook();

            Assert.AreEqual(AnimationHookType.SetLight, obj.HookType);
        }

        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new SetLightHook() {
                Direction = AnimationHookDir.Both,
                LightsOn = true,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new SetLightHook();
            var reader = new DatFileReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Direction, readObj.Direction);
            Assert.AreEqual(writeObj.LightsOn, readObj.LightsOn);
        }

        [TestMethod]
        public void CanBeCreatedByAbstractUnpack() {
            var writeHook = new SetLightHook() {
                Direction = AnimationHookDir.Forward,
                LightsOn = false
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeHook.Pack(writer);

            var reader = new DatFileReader(buffer);
            var readObj = AnimationHook.Unpack(reader, writeHook.HookType);

            Assert.IsNotNull(readObj);
            Assert.IsInstanceOfType(readObj, typeof(SetLightHook));

            var readHook = readObj as SetLightHook;
            Assert.IsNotNull(readHook);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.LightsOn, readHook.LightsOn);
        }
    }
}
