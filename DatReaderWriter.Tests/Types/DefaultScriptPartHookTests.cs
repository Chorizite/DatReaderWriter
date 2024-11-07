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
    public class DefaultScriptPartHookTests {
        [TestMethod]
        public void HasCorrectHookType() {
            var obj = new DefaultScriptPartHook();

            Assert.AreEqual(AnimationHookType.DefaultScriptPart, obj.HookType);
        }

        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeHook = new DefaultScriptPartHook() {
                Direction = AnimationHookDir.Forward,
                PartIndex = 123
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeHook.Pack(writer);

            var readHook = new DefaultScriptPartHook();
            var reader = new DatBinReader(buffer);
            readHook.Unpack(reader);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.PartIndex, readHook.PartIndex);
        }

        [TestMethod]
        public void CanBeCreatedByAbstractUnpack() {
            var writeHook = new DefaultScriptPartHook() {
                Direction = AnimationHookDir.Forward,
                PartIndex = 123
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeHook.Pack(writer);

            var reader = new DatBinReader(buffer);
            var readObj = AnimationHook.Unpack(reader, writeHook.HookType);

            Assert.IsNotNull(readObj);
            Assert.IsInstanceOfType(readObj, typeof(DefaultScriptPartHook));

            var readHook = readObj as DefaultScriptPartHook;
            Assert.IsNotNull(readHook);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.PartIndex, readHook.PartIndex);
        }
    }
}
