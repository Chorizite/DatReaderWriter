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
using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.IO;
using ACClientLib.DatReaderWriter.Types;
using ACClientLib.DatReaderWriter.Attributes;

namespace ACClientLib.DatReaderWriter.DBObjs {
    /// <summary>
    /// DB_TYPE_MTABLE in the client.
    /// </summary>
    [DBObjType(typeof(MotionTable), DatFileType.Portal, DBObjType.MotionTable, DBObjHeaderFlags.HasId, 0x09000000, 0x0900FFFF, 0x00000000)]
    public partial class MotionTable : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.MotionTable;

        public MotionCommand DefaultStyle;

        public Dictionary<MotionCommand, MotionCommand> StyleDefaults = [];

        public Dictionary<int, MotionData> Cycles = [];

        public Dictionary<int, MotionData> Modifiers = [];

        public Dictionary<int, MotionCommandData> Links = [];

        /// <inheritdoc />
        public override bool Unpack(DatFileReader reader) {
            base.Unpack(reader);
            DefaultStyle = (MotionCommand)reader.ReadUInt32();
            var _numStyleDefaults = reader.ReadUInt32();
            for (var i=0; i < _numStyleDefaults; i++) {
                var _key = (MotionCommand)reader.ReadUInt32();
                var _val = (MotionCommand)reader.ReadUInt32();
                StyleDefaults.Add(_key, _val);
            }
            var _numCycles = reader.ReadUInt32();
            for (var i=0; i < _numCycles; i++) {
                var _key = reader.ReadInt32();
                var _val = reader.ReadItem<MotionData>();
                Cycles.Add(_key, _val);
            }
            var _numModifiers = reader.ReadUInt32();
            for (var i=0; i < _numModifiers; i++) {
                var _key = reader.ReadInt32();
                var _val = reader.ReadItem<MotionData>();
                Modifiers.Add(_key, _val);
            }
            var _numLinks = reader.ReadUInt32();
            for (var i=0; i < _numLinks; i++) {
                var _key = reader.ReadInt32();
                var _val = reader.ReadItem<MotionCommandData>();
                Links.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatFileWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32((uint)DefaultStyle);
            writer.WriteUInt32((uint)StyleDefaults.Count());
            foreach (var kv in StyleDefaults) {
                writer.WriteUInt32((uint)kv.Key);
                writer.WriteUInt32((uint)kv.Value);
            }
            writer.WriteUInt32((uint)Cycles.Count());
            foreach (var kv in Cycles) {
                writer.WriteInt32(kv.Key);
                writer.WriteItem<MotionData>(kv.Value);
            }
            writer.WriteUInt32((uint)Modifiers.Count());
            foreach (var kv in Modifiers) {
                writer.WriteInt32(kv.Key);
                writer.WriteItem<MotionData>(kv.Value);
            }
            writer.WriteUInt32((uint)Links.Count());
            foreach (var kv in Links) {
                writer.WriteInt32(kv.Key);
                writer.WriteItem<MotionCommandData>(kv.Value);
            }
            return true;
        }

    }

}
