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
    public class TextureVelocityHookTests {
        [TestMethod]
        public void HasCorrectHookType() {
            var obj = new TextureVelocityHook();

            Assert.AreEqual(AnimationHookType.TextureVelocity, obj.HookType);
        }

        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new TextureVelocityHook() {
                Direction = AnimationHookDir.Both,
                USpeed = 0.5f,
                VSpeed = 0.4f,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new TextureVelocityHook();
            var reader = new DatFileReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Direction, readObj.Direction);
            Assert.AreEqual(writeObj.USpeed, readObj.USpeed);
            Assert.AreEqual(writeObj.VSpeed, readObj.VSpeed);
        }

        [TestMethod]
        public void CanBeCreatedByAbstractUnpack() {
            var writeHook = new TextureVelocityHook() {
                Direction = AnimationHookDir.Forward,
                USpeed = 0.5f,
                VSpeed = 0.4f,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeHook.Pack(writer);

            var reader = new DatFileReader(buffer);
            var readObj = AnimationHook.Unpack(reader, writeHook.HookType);

            Assert.IsNotNull(readObj);
            Assert.IsInstanceOfType(readObj, typeof(TextureVelocityHook));

            var readHook = readObj as TextureVelocityHook;
            Assert.IsNotNull(readHook);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.USpeed, readHook.USpeed);
            Assert.AreEqual(writeHook.VSpeed, readHook.VSpeed);
        }
    }
}
