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
    public class SetOmegaHookTests {
        [TestMethod]
        public void HasCorrectHookType() {
            var obj = new SetOmegaHook();

            Assert.AreEqual(AnimationHookType.SetOmega, obj.HookType);
        }

        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new SetOmegaHook() {
                Direction = AnimationHookDir.Both,
                Axis = new Vector3(1, 2, 3)
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new SetOmegaHook();
            var reader = new DatFileReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Direction, readObj.Direction);
            Assert.AreEqual(writeObj.Axis, readObj.Axis);
        }

        [TestMethod]
        public void CanBeCreatedByAbstractUnpack() {
            var writeHook = new SetOmegaHook() {
                Direction = AnimationHookDir.Forward,
                Axis = new Vector3(1, 2, 3),
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeHook.Pack(writer);

            var reader = new DatFileReader(buffer);
            var readObj = AnimationHook.Unpack(reader, writeHook.HookType);

            Assert.IsNotNull(readObj);
            Assert.IsInstanceOfType(readObj, typeof(SetOmegaHook));

            var readHook = readObj as SetOmegaHook;
            Assert.IsNotNull(readHook);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.Axis, readHook.Axis);
        }
    }
}
