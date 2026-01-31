using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using Newtonsoft.Json;
using System.Numerics;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class ChatPoseTableTests {
        [TestMethod]
        public void CanInsertAndReadChatPoseTables() {
            var datFilePath = Path.GetTempFileName();
            var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var chatPoseTable = new ChatPoseTable() {
                Id = 0xE000007,
                ChatPoses = new() {
                    {"AFK", "AFKState" },
                    { "Another", "AnotherState" }
                },
                ChatEmotes = new() {
                    { "AFKState", new() {
                        MyEmote = "are afk.",
                        OtherEmote = "is afk."
                    }},
                    { "AnotherState", new() {
                        MyEmote = "are in another state.",
                        OtherEmote = "is in another state."
                    }}
                }
            };

            var res = dat.TryWriteFile(chatPoseTable);

            var res2 = dat.TryGet<ChatPoseTable>(0xE000007, out var readChatPoseTable);
            Assert.IsTrue(res2);
            Assert.IsNotNull(readChatPoseTable);
            
            Assert.AreEqual(2, readChatPoseTable.ChatPoses.Count);
            Assert.AreEqual("AFKState", readChatPoseTable.ChatPoses["AFK"]);
            Assert.AreEqual("AnotherState", readChatPoseTable.ChatPoses["Another"]);
            Assert.AreEqual(2, readChatPoseTable.ChatEmotes.Count);
            Assert.AreEqual("are afk.", readChatPoseTable.ChatEmotes["AFKState"].MyEmote);
            Assert.AreEqual("is afk.", readChatPoseTable.ChatEmotes["AFKState"].OtherEmote);
            Assert.AreEqual("are in another state.", readChatPoseTable.ChatEmotes["AnotherState"].MyEmote);
            Assert.AreEqual("is in another state.", readChatPoseTable.ChatEmotes["AnotherState"].OtherEmote);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORChatPoseTable() {
            using var dat = new DatCollection(EORCommonData.DatDirectory);

            var res = dat.TryGet<ChatPoseTable>(0xE000007, out var chatPoseTable);
            Assert.IsTrue(res);
            Assert.IsNotNull(chatPoseTable);
            Assert.AreEqual(0xE000007u, chatPoseTable.Id);
            Assert.AreEqual(309, chatPoseTable.ChatPoses.Count);
            Assert.AreEqual(74, chatPoseTable.ChatEmotes.Count);
            
            Assert.IsTrue(chatPoseTable.ChatPoses.ContainsKey("waving"));
            Assert.AreEqual("WAVESTATE", chatPoseTable.ChatPoses["waving"]);
            
            Assert.IsTrue(chatPoseTable.ChatPoses.ContainsKey("waving"));
            Assert.AreEqual("BlowKiss", chatPoseTable.ChatPoses["kisses"]);
            
            Assert.IsTrue(chatPoseTable.ChatEmotes.ContainsKey("Cringe"));
            Assert.AreEqual("cringe in terror.", chatPoseTable.ChatEmotes["Cringe"].MyEmote);
            Assert.AreEqual("cringes in terror.", chatPoseTable.ChatEmotes["Cringe"].OtherEmote);
            
            Assert.IsTrue(chatPoseTable.ChatEmotes.ContainsKey("ATOYOT"));
            Assert.AreEqual("experience quite the feeling!", chatPoseTable.ChatEmotes["ATOYOT"].MyEmote);
            Assert.AreEqual("experiences quite the feeling!", chatPoseTable.ChatEmotes["ATOYOT"].OtherEmote);
            
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<ChatPoseTable>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0xE000007);
        }
    }
}
