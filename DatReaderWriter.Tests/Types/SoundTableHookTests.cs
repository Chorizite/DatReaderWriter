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
    public class SoundTableHookTests {
        [TestMethod]
        public void HasCorrectHookType() {
            var obj = new SoundTableHook();

            Assert.AreEqual(AnimationHookType.SoundTable, obj.HookType);
        }

        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new SoundTableHook() {
                Direction = AnimationHookDir.Both,
                SoundType = Sound.Dance2,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new SoundTableHook();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Direction, readObj.Direction);
            Assert.AreEqual(writeObj.SoundType, readObj.SoundType);
        }

        [TestMethod]
        public void CanBeCreatedByAbstractUnpack() {
            var writeHook = new SoundTableHook() {
                Direction = AnimationHookDir.Forward,
                SoundType = Sound.Fizzle,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeHook.Pack(writer);

            var reader = new DatBinReader(buffer);
            var readObj = AnimationHook.Unpack(reader, writeHook.HookType);

            Assert.IsNotNull(readObj);
            Assert.IsInstanceOfType(readObj, typeof(SoundTableHook));

            var readHook = readObj as SoundTableHook;
            Assert.IsNotNull(readHook);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.SoundType, readHook.SoundType);
        }
    }
}
