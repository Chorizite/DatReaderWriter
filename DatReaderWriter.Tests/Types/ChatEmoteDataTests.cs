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
    public class ChatEmoteDataTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new ChatEmoteData() {
                MyEmote = "test",
                OtherEmote = "test2",
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new ChatEmoteData();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.MyEmote, readObj.MyEmote);
            Assert.AreEqual(writeObj.OtherEmote, readObj.OtherEmote);
        }
    }
}
