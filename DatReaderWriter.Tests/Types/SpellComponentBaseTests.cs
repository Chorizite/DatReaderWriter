using DatReaderWriter.Enums;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Types;
using DatReaderWriter.Tests.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests.Types {
    [TestClass]
    public class SpellComponentBaseTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new SpellComponentBase() {
                Name = "Test",
                Category = 1,
                Icon = 0x12345678,
                Type = ComponentType.Scarab,
                Gesture = 0x98765432,
                Time = 1.1f,
                Text = "Test text",
                CDM = 0.5f
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new SpellComponentBase();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Name, readObj.Name);
            Assert.AreEqual(writeObj.Category, readObj.Category);
            Assert.AreEqual(writeObj.Icon, readObj.Icon);
            Assert.AreEqual(writeObj.Type, readObj.Type);
            Assert.AreEqual(writeObj.Gesture, readObj.Gesture);
            Assert.AreEqual(writeObj.Time, readObj.Time);
            Assert.AreEqual(writeObj.Text, readObj.Text);
            Assert.AreEqual(writeObj.CDM, readObj.CDM);
        }
    }
}
