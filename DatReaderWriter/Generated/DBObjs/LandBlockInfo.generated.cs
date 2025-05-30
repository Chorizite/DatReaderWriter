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
    /// Stores static spawns, buildings. DB_TYPE_LBI in the client.
    /// </summary>
    [DBObjType(typeof(LandBlockInfo), DatFileType.Cell, DBObjType.LandBlockInfo, DBObjHeaderFlags.HasId, 0x00000000, 0x00000000, 0x0000FFFE)]
    public partial class LandBlockInfo : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.LandBlockInfo;

        /// <summary>
        /// The number of inside cells. These can be looked up at 0xAAAABBBB where AAAA is the landblock id and BBBB is the cell id + 0x100
        /// </summary>
        public uint NumCells;

        /// <summary>
        /// Static objects
        /// </summary>
        public List<Stab> Objects = [];

        public List<BuildingInfo> Buildings = [];

        public Dictionary<uint, uint> RestrictionTable = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            NumCells = reader.ReadUInt32();
            var _numObjects = reader.ReadUInt32();
            for (var i=0; i < _numObjects; i++) {
                Objects.Add(reader.ReadItem<Stab>());
            }
            var _numBuildings = reader.ReadUInt16();
            var _hasRestrictionsTable = reader.ReadBool(2);
            for (var i=0; i < _numBuildings; i++) {
                Buildings.Add(reader.ReadItem<BuildingInfo>());
            }
            if (_hasRestrictionsTable) {
                var _numRestrictionTableEntries = reader.ReadUInt16();
                var _numBuckets = reader.ReadUInt16();
                for (var i=0; i < _numRestrictionTableEntries; i++) {
                    var _key = reader.ReadUInt32();
                    var _val = reader.ReadUInt32();
                    RestrictionTable.Add(_key, _val);
                }
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32(NumCells);
            writer.WriteUInt32((uint)Objects.Count());
            foreach (var item in Objects) {
                writer.WriteItem<Stab>(item);
            }
            writer.WriteUInt16((ushort)Buildings.Count());
            writer.WriteBool(RestrictionTable != null && RestrictionTable.Count() > 0, 2);
            foreach (var item in Buildings) {
                writer.WriteItem<BuildingInfo>(item);
            }
            if (RestrictionTable != null && RestrictionTable.Count() > 0) {
                writer.WriteUInt16((ushort)RestrictionTable.Count());
                writer.WriteUInt16(8);
                foreach (var kv in RestrictionTable) {
                    writer.WriteUInt32(kv.Key);
                    writer.WriteUInt32(kv.Value);
                }
            }
            return true;
        }

    }

}
