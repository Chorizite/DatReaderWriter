using DatReaderWriter.Enums;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests.Types {
    [TestClass]
    public class LuminousPartHookTests {
        [TestMethod]
        public void HasCorrectHookType() {
            var obj = new LuminousPartHook();

            Assert.AreEqual(AnimationHookType.LuminousPart, obj.HookType);
        }

        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeHook = new LuminousPartHook() {
                Direction = AnimationHookDir.Forward,
                Start = 0f,
                End = 1.1f,
                Time = 2.2f,
                PartIndex = 3
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeHook.Pack(writer);

            var readHook = new LuminousPartHook();
            var reader = new DatBinReader(buffer);
            readHook.Unpack(reader);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.Start, readHook.Start);
            Assert.AreEqual(writeHook.End, readHook.End);
            Assert.AreEqual(writeHook.Time, readHook.Time);
            Assert.AreEqual(writeHook.PartIndex, readHook.PartIndex);
        }

        [TestMethod]
        public void CanBeCreatedByAbstractUnpack() {
            var writeHook = new LuminousPartHook() {
                Direction = AnimationHookDir.Forward,
                Start = 0f,
                End = 1.1f,
                Time = 2.2f,
                PartIndex = 3
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeHook.Pack(writer);

            var reader = new DatBinReader(buffer);
            var readObj = AnimationHook.Unpack(reader, writeHook.HookType);

            Assert.IsNotNull(readObj);
            Assert.IsInstanceOfType(readObj, typeof(LuminousPartHook));

            var readHook = readObj as LuminousPartHook;
            Assert.IsNotNull(readHook);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.Start, readHook.Start);
            Assert.AreEqual(writeHook.End, readHook.End);
            Assert.AreEqual(writeHook.Time, readHook.Time);
            Assert.AreEqual(writeHook.PartIndex, readHook.PartIndex);
        }
    }
}
