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
    public class SoundTweakedHookTests {
        [TestMethod]
        public void HasCorrectHookType() {
            var obj = new SoundTweakedHook();

            Assert.AreEqual(AnimationHookType.SoundTweaked, obj.HookType);
        }

        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new SoundTweakedHook() {
                Direction = AnimationHookDir.Both,
                Priority = 0.5f,
                Probability = 0.4f,
                SoundID = 1234,
                Volume = 0.2f,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new SoundTweakedHook();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Direction, readObj.Direction);
            Assert.AreEqual(writeObj.Priority, readObj.Priority);
            Assert.AreEqual(writeObj.Probability, readObj.Probability);
            Assert.AreEqual(writeObj.SoundID, readObj.SoundID);
            Assert.AreEqual(writeObj.Volume, readObj.Volume);
        }

        [TestMethod]
        public void CanBeCreatedByAbstractUnpack() {
            var writeHook = new SoundTweakedHook() {
                Direction = AnimationHookDir.Forward,
                Priority = 0.5f,
                Probability = 0.4f,
                SoundID = 1234,
                Volume = 0.2f,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeHook.Pack(writer);

            var reader = new DatBinReader(buffer);
            var readObj = AnimationHook.Unpack(reader, writeHook.HookType);

            Assert.IsNotNull(readObj);
            Assert.IsInstanceOfType(readObj, typeof(SoundTweakedHook));

            var readHook = readObj as SoundTweakedHook;
            Assert.IsNotNull(readHook);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.Priority, readHook.Priority);
            Assert.AreEqual(writeHook.Probability, readHook.Probability);
            Assert.AreEqual(writeHook.SoundID, readHook.SoundID);
            Assert.AreEqual(writeHook.Volume, readHook.Volume);
        }
    }
}
