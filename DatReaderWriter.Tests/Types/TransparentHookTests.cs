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
    public class TransparentHookTests {
        [TestMethod]
        public void HasCorrectHookType() {
            var obj = new TransparentHook();

            Assert.AreEqual(AnimationHookType.Transparent, obj.HookType);
        }

        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new TransparentHook() {
                Direction = AnimationHookDir.Both,
                Start = 0.5f,
                End = 0.4f,
                Time = 0.3f,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new TransparentHook();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Direction, readObj.Direction);
            Assert.AreEqual(writeObj.Start, readObj.Start);
            Assert.AreEqual(writeObj.End, readObj.End);
            Assert.AreEqual(writeObj.Time, readObj.Time);
        }

        [TestMethod]
        public void CanBeCreatedByAbstractUnpack() {
            var writeHook = new TransparentHook() {
                Direction = AnimationHookDir.Forward,
                Start = 0.5f,
                End = 0.4f,
                Time = 0.3f,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeHook.Pack(writer);

            var reader = new DatBinReader(buffer);
            var readObj = AnimationHook.Unpack(reader, writeHook.HookType);

            Assert.IsNotNull(readObj);
            Assert.IsInstanceOfType(readObj, typeof(TransparentHook));

            var readHook = readObj as TransparentHook;
            Assert.IsNotNull(readHook);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.Start, readHook.Start);
            Assert.AreEqual(writeHook.End, readHook.End);
            Assert.AreEqual(writeHook.Time, readHook.Time);
        }
    }
}
