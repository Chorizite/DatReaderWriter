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
    public class AttackHookTests {
        [TestMethod]
        public void HasCorrectHookType() {
            var obj = new AttackHook();

            Assert.AreEqual(AnimationHookType.Attack, obj.HookType);
        }

        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new AttackHook() {
                AttackCone = new AttackCone() {
                    Height = 1,
                    LeftX = 2,
                    LeftY = 3,
                    RightX = 4,
                    RightY = 5,
                    PartIndex = 6,
                    Radius = 7f
                },
                Direction = AnimationHookDir.Both,
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new AttackHook();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Direction, readObj.Direction);
            Assert.IsNotNull(readObj.AttackCone);

            Assert.AreEqual(writeObj.AttackCone.Height, readObj.AttackCone.Height);
            Assert.AreEqual(writeObj.AttackCone.LeftX, readObj.AttackCone.LeftX);
            Assert.AreEqual(writeObj.AttackCone.LeftY, readObj.AttackCone.LeftY);
            Assert.AreEqual(writeObj.AttackCone.RightX, readObj.AttackCone.RightX);
            Assert.AreEqual(writeObj.AttackCone.RightY, readObj.AttackCone.RightY);
            Assert.AreEqual(writeObj.AttackCone.PartIndex, readObj.AttackCone.PartIndex);
            Assert.AreEqual(writeObj.AttackCone.Radius, readObj.AttackCone.Radius);
        }

        [TestMethod]
        public void CanBeCreatedByAbstractUnpack() {
            var writeHook = new AttackHook() {
                AttackCone = new AttackCone() {
                    Height = 1,
                    LeftX = 2,
                    LeftY = 3,
                    RightX = 4,
                    RightY = 5,
                    PartIndex = 6,
                    Radius = 7f
                },
                Direction = AnimationHookDir.Forward
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeHook.Pack(writer);

            var reader = new DatBinReader(buffer);
            var readObj = AnimationHook.Unpack(reader, writeHook.HookType);

            Assert.IsNotNull(readObj);
            Assert.IsInstanceOfType(readObj, typeof(AttackHook));

            var readHook = readObj as AttackHook;
            Assert.IsNotNull(readHook);
            Assert.IsNotNull(readHook.AttackCone);

            Assert.AreEqual(writeHook.AttackCone.Height, readHook.AttackCone.Height);
            Assert.AreEqual(writeHook.AttackCone.LeftX, readHook.AttackCone.LeftX);
            Assert.AreEqual(writeHook.AttackCone.LeftY, readHook.AttackCone.LeftY);
            Assert.AreEqual(writeHook.AttackCone.RightX, writeHook.AttackCone.RightX);
            Assert.AreEqual(writeHook.AttackCone.RightY, writeHook.AttackCone.RightY);
            Assert.AreEqual(writeHook.AttackCone.PartIndex, writeHook.AttackCone.PartIndex);
            Assert.AreEqual(writeHook.AttackCone.Radius, writeHook.AttackCone.Radius);
        }
    }
}
