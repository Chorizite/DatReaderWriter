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
    public class LuminousHookTests {
        [TestMethod]
        public void HasCorrectHookType() {
            var obj = new LuminousHook();

            Assert.AreEqual(AnimationHookType.Luminous, obj.HookType);
        }

        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeHook = new LuminousHook() {
                Direction = AnimationHookDir.Forward,
                Start = 0f,
                End = 1.1f,
                Time = 2.2f
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeHook.Pack(writer);

            var readHook = new LuminousHook();
            var reader = new DatFileReader(buffer);
            readHook.Unpack(reader);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.Start, readHook.Start);
            Assert.AreEqual(writeHook.End, readHook.End);
            Assert.AreEqual(writeHook.Time, readHook.Time);
        }

        [TestMethod]
        public void CanBeCreatedByAbstractUnpack() {
            var writeHook = new LuminousHook() {
                Direction = AnimationHookDir.Forward,
                Start = 0f,
                End = 1.1f,
                Time = 2.2f
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeHook.Pack(writer);

            var reader = new DatFileReader(buffer);
            var readObj = AnimationHook.Unpack(reader, writeHook.HookType);

            Assert.IsNotNull(readObj);
            Assert.IsInstanceOfType(readObj, typeof(LuminousHook));

            var readHook = readObj as LuminousHook;
            Assert.IsNotNull(readHook);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.Start, readHook.Start);
            Assert.AreEqual(writeHook.End, readHook.End);
            Assert.AreEqual(writeHook.Time, readHook.Time);
        }
    }
}
