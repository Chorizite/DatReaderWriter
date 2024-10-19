using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.IO;
using ACClientLib.DatReaderWriter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests.Types {
    [TestClass]
    public class DiffuseHookTests {
        [TestMethod]
        public void HasCorrectHookType() {
            var obj = new DiffuseHook();

            Assert.AreEqual(AnimationHookType.Diffuse, obj.HookType);
        }

        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeHook = new DiffuseHook() {
                Direction = AnimationHookDir.Forward,
                End = 1f,
                Start = 0f,
                Time = 2f,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeHook.Pack(writer);

            var readHook = new DiffuseHook();
            var reader = new DatFileReader(buffer);
            readHook.Unpack(reader);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.End, readHook.End);
            Assert.AreEqual(writeHook.Start, readHook.Start);
            Assert.AreEqual(writeHook.Time, readHook.Time);
        }

        [TestMethod]
        public void CanBeCreatedByAbstractUnpack() {
            var writeHook = new DiffuseHook() {
                Direction = AnimationHookDir.Forward,
                End = 1f,
                Start = 0f,
                Time = 2f,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeHook.Pack(writer);

            var reader = new DatFileReader(buffer);
            var readObj = AnimationHook.Unpack(reader, writeHook.HookType);

            Assert.IsNotNull(readObj);
            Assert.IsInstanceOfType(readObj, typeof(DiffuseHook));

            var readHook = readObj as DiffuseHook;
            Assert.IsNotNull(readHook);

            Assert.AreEqual(writeHook.Direction, readHook.Direction);
            Assert.AreEqual(writeHook.End, readHook.End);
            Assert.AreEqual(writeHook.Start, readHook.Start);
            Assert.AreEqual(writeHook.Time, readHook.Time);
        }
    }
}
