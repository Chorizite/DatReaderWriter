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
    public class ReplaceObjectHookTests {
        [TestMethod]
        public void HasCorrectHookType() {
            var obj = new ReplaceObjectHook();

            Assert.AreEqual(AnimationHookType.ReplaceObject, obj.HookType);
        }

        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new ReplaceObjectHook() {
                Direction = AnimationHookDir.Both,
                PartId = 0x12 | 0x01000000,
                PartIndex = 123,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new ReplaceObjectHook();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Direction, readObj.Direction);
            Assert.AreEqual(writeObj.PartId, readObj.PartId);
            Assert.AreEqual(writeObj.PartIndex, readObj.PartIndex);
        }

        [TestMethod]
        public void CanBeCreatedByAbstractUnpack() {
            var writeHook = new ReplaceObjectHook() {
                Direction = AnimationHookDir.Forward,
                PartId = 0x8123 | 0x01000000,
                PartIndex = 123,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeHook.Pack(writer);

            var reader = new DatBinReader(buffer);
            var readObj = AnimationHook.Unpack(reader, writeHook.HookType);

            Assert.IsNotNull(readObj);
            Assert.IsInstanceOfType(readObj, typeof(ReplaceObjectHook));

            var readHook = readObj as ReplaceObjectHook;
            Assert.IsNotNull(readHook);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.PartId, readHook.PartId);
            Assert.AreEqual(writeHook.PartIndex, readHook.PartIndex);
        }
    }
}
