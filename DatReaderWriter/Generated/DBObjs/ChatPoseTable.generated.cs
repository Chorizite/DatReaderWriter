//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//
//                                                            //
//                          WARNING                           //
//                                                            //
//           DO NOT MAKE LOCAL CHANGES TO THIS FILE           //
//               EDIT THE .tt TEMPLATE INSTEAD                //
//                                                            //
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//


using System;
using System.Numerics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DatReaderWriter.Enums;
using DatReaderWriter.Types;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.DBObjs {
    /// <summary>
    /// DB_TYPE_CHAT_POSE_TABLE in the client.
    /// </summary>
    [DBObjType(typeof(ChatPoseTable), DatFileType.Portal, DBObjType.ChatPoseTable, DBObjHeaderFlags.HasId, 0x0E000007, 0x0E000007, 0x00000000)]
    public partial class ChatPoseTable : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.ChatPoseTable;

        public Dictionary<string, string> ChatPoseHash = [];

        public Dictionary<string, ChatEmoteData> ChatEmoteHash = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            var _numChatPoseHashes = reader.ReadUInt16();
            var _numChatPoseHashBuckets = reader.ReadUInt16();
            for (var i=0; i < _numChatPoseHashes; i++) {
                var _key = reader.ReadString16L();
                var _val = reader.ReadString16L();
                ChatPoseHash.Add(_key, _val);
            }
            var _numChatEmoteHashes = reader.ReadUInt16();
            var _numChatEmoteHashBuckets = reader.ReadUInt16();
            for (var i=0; i < _numChatEmoteHashes; i++) {
                var _key = reader.ReadString16L();
                var _val = reader.ReadItem<ChatEmoteData>();
                ChatEmoteHash.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt16((ushort)ChatPoseHash.Count());
            writer.WriteUInt16(32);
            foreach (var kv in ChatPoseHash) {
                writer.WriteString16L(kv.Key);
                writer.WriteString16L(kv.Value);
            }
            writer.WriteUInt16((ushort)ChatEmoteHash.Count());
            writer.WriteUInt16(32);
            foreach (var kv in ChatEmoteHash) {
                writer.WriteString16L(kv.Key);
                writer.WriteItem<ChatEmoteData>(kv.Value);
            }
            return true;
        }

    }

}
