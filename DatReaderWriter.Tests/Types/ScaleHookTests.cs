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
    public class ScaleHookTests {
        [TestMethod]
        public void HasCorrectHookType() {
            var obj = new ScaleHook();

            Assert.AreEqual(AnimationHookType.Scale, obj.HookType);
        }

        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new ScaleHook() {
                Direction = AnimationHookDir.Both,
                Time = 2.2f,
                End = 1.1f,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new ScaleHook();
            var reader = new DatFileReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Direction, readObj.Direction);
            Assert.AreEqual(writeObj.Time, readObj.Time);
            Assert.AreEqual(writeObj.End, readObj.End);
        }

        [TestMethod]
        public void CanBeCreatedByAbstractUnpack() {
            var writeHook = new ScaleHook() {
                Direction = AnimationHookDir.Forward,
                Time = 2.2f,
                End = 1.1f,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeHook.Pack(writer);

            var reader = new DatFileReader(buffer);
            var readObj = AnimationHook.Unpack(reader, writeHook.HookType);

            Assert.IsNotNull(readObj);
            Assert.IsInstanceOfType(readObj, typeof(ScaleHook));

            var readHook = readObj as ScaleHook;
            Assert.IsNotNull(readHook);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.Time, readHook.Time);
            Assert.AreEqual(writeHook.End, readHook.End);
        }
    }
}
