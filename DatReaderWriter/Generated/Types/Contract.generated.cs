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
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.Types {
    public partial class Contract : IDatObjType {
        public uint Version;

        public uint ContractId;

        public string ContractName;

        public string Description;

        public string DescriptionProgress;

        public string NameNPCStart;

        public string NameNPCEnd;

        public string QuestflagStamped;

        public string QuestflagStarted;

        public string QuestflagFinished;

        public string QuestflagProgress;

        public string QuestflagTimer;

        public string QuestflagRepeatTime;

        public Position LocationNPCStart;

        public Position LocationNPCEnd;

        public Position LocationQuestArea;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Version = reader.ReadUInt32();
            ContractId = reader.ReadUInt32();
            ContractName = reader.ReadString16L();
            reader.Align(4);
            Description = reader.ReadString16L();
            reader.Align(4);
            DescriptionProgress = reader.ReadString16L();
            reader.Align(4);
            NameNPCStart = reader.ReadString16L();
            reader.Align(4);
            NameNPCEnd = reader.ReadString16L();
            reader.Align(4);
            QuestflagStamped = reader.ReadString16L();
            reader.Align(4);
            QuestflagStarted = reader.ReadString16L();
            reader.Align(4);
            QuestflagFinished = reader.ReadString16L();
            reader.Align(4);
            QuestflagProgress = reader.ReadString16L();
            reader.Align(4);
            QuestflagTimer = reader.ReadString16L();
            reader.Align(4);
            QuestflagRepeatTime = reader.ReadString16L();
            reader.Align(4);
            LocationNPCStart = reader.ReadItem<Position>();
            LocationNPCEnd = reader.ReadItem<Position>();
            LocationQuestArea = reader.ReadItem<Position>();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(Version);
            writer.WriteUInt32(ContractId);
            writer.WriteString16L(ContractName);
            writer.Align(4);
            writer.WriteString16L(Description);
            writer.Align(4);
            writer.WriteString16L(DescriptionProgress);
            writer.Align(4);
            writer.WriteString16L(NameNPCStart);
            writer.Align(4);
            writer.WriteString16L(NameNPCEnd);
            writer.Align(4);
            writer.WriteString16L(QuestflagStamped);
            writer.Align(4);
            writer.WriteString16L(QuestflagStarted);
            writer.Align(4);
            writer.WriteString16L(QuestflagFinished);
            writer.Align(4);
            writer.WriteString16L(QuestflagProgress);
            writer.Align(4);
            writer.WriteString16L(QuestflagTimer);
            writer.Align(4);
            writer.WriteString16L(QuestflagRepeatTime);
            writer.Align(4);
            writer.WriteItem<Position>(LocationNPCStart);
            writer.WriteItem<Position>(LocationNPCEnd);
            writer.WriteItem<Position>(LocationQuestArea);
            return true;
        }

    }

}
