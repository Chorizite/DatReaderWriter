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
    public class PhysicsScriptDataTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new PhysicsScriptData() {
                StartTime = 1.1,
                Hook = new DefaultScriptHook() { Direction = AnimationHookDir.Backward }
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new PhysicsScriptData();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.StartTime, readObj.StartTime);
            Assert.IsInstanceOfType(readObj.Hook, typeof(DefaultScriptHook));

            Assert.AreEqual(writeObj.Hook.Direction, readObj.Hook.Direction);
        }
    }
}
