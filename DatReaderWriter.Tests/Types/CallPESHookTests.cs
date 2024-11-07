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
    public class CallPESHookTests {
        [TestMethod]
        public void HasCorrectHookType() {
            var obj = new CallPESHook();

            Assert.AreEqual(AnimationHookType.CallPES, obj.HookType);
        }

        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeHook = new CallPESHook() {
                Direction = AnimationHookDir.Forward,
                Pause = 0.123f,
                PES = 0x12345678,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeHook.Pack(writer);

            var readHook = new CallPESHook();
            var reader = new DatBinReader(buffer);
            readHook.Unpack(reader);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.Pause, readHook.Pause);
            Assert.AreEqual(writeHook.PES, readHook.PES);
        }

        [TestMethod]
        public void CanBeCreatedByAbstractUnpack() {
            var writeHook = new CallPESHook() {
                Direction = AnimationHookDir.Forward
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeHook.Pack(writer);

            var reader = new DatBinReader(buffer);
            var readHook = AnimationHook.Unpack(reader, writeHook.HookType);

            Assert.IsNotNull(readHook);
            Assert.IsInstanceOfType(readHook, typeof(CallPESHook));
        }
    }
}
