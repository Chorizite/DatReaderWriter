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
    public class TransparentPartHookTests {
        [TestMethod]
        public void HasCorrectHookType() {
            var obj = new TransparentPartHook();

            Assert.AreEqual(AnimationHookType.TransparentPart, obj.HookType);
        }

        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new TransparentPartHook() {
                Direction = AnimationHookDir.Both,
                Start = 0.5f,
                End = 0.4f,
                Time = 0.3f,
                PartIndex = 3
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new TransparentPartHook();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Direction, readObj.Direction);
            Assert.AreEqual(writeObj.Start, readObj.Start);
            Assert.AreEqual(writeObj.End, readObj.End);
            Assert.AreEqual(writeObj.Time, readObj.Time);
            Assert.AreEqual(writeObj.PartIndex, readObj.PartIndex);
        }

        [TestMethod]
        public void CanBeCreatedByAbstractUnpack() {
            var writeHook = new TransparentPartHook() {
                Direction = AnimationHookDir.Forward,
                Start = 0.5f,
                End = 0.4f,
                Time = 0.3f,
                PartIndex = 3
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeHook.Pack(writer);

            var reader = new DatBinReader(buffer);
            var readObj = AnimationHook.Unpack(reader, writeHook.HookType);

            Assert.IsNotNull(readObj);
            Assert.IsInstanceOfType(readObj, typeof(TransparentPartHook));

            var readHook = readObj as TransparentPartHook;
            Assert.IsNotNull(readHook);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.Start, readHook.Start);
            Assert.AreEqual(writeHook.End, readHook.End);
            Assert.AreEqual(writeHook.Time, readHook.Time);
            Assert.AreEqual(writeHook.PartIndex, readHook.PartIndex);
        }
    }
}
