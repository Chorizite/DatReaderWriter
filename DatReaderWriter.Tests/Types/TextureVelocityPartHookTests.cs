using DatReaderWriter.Enums;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests.Types {
    [TestClass]
    public class TextureVelocityPartHookTests {
        [TestMethod]
        public void HasCorrectHookType() {
            var obj = new TextureVelocityPartHook();

            Assert.AreEqual(AnimationHookType.TextureVelocityPart, obj.HookType);
        }

        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new TextureVelocityPartHook() {
                Direction = AnimationHookDir.Both,
                USpeed = 0.5f,
                VSpeed = 0.4f,
                PartIndex = 3
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new TextureVelocityPartHook();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Direction, readObj.Direction);
            Assert.AreEqual(writeObj.USpeed, readObj.USpeed);
            Assert.AreEqual(writeObj.VSpeed, readObj.VSpeed);
            Assert.AreEqual(writeObj.PartIndex, readObj.PartIndex);
        }

        [TestMethod]
        public void CanBeCreatedByAbstractUnpack() {
            var writeHook = new TextureVelocityPartHook() {
                Direction = AnimationHookDir.Forward,
                USpeed = 0.5f,
                VSpeed = 0.4f,
                PartIndex = 3
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeHook.Pack(writer);

            var reader = new DatBinReader(buffer);
            var readObj = AnimationHook.Unpack(reader, writeHook.HookType);

            Assert.IsNotNull(readObj);
            Assert.IsInstanceOfType(readObj, typeof(TextureVelocityPartHook));

            var readHook = readObj as TextureVelocityPartHook;
            Assert.IsNotNull(readHook);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.USpeed, readHook.USpeed);
            Assert.AreEqual(writeHook.VSpeed, readHook.VSpeed);
            Assert.AreEqual(writeHook.PartIndex, readHook.PartIndex);
        }
    }
}
