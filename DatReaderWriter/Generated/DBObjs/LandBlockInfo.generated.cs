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
    [DBObjType(DatFileType.Cell, false, 0x00000000, 0x00000000)]
    public class LandBlockInfo : DBObj {
        /// <inheritdoc />
        public override bool HasDataCategory => false;

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
        public override bool Unpack(DatFileReader reader) {
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
        public override bool Pack(DatFileWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32(NumCells);
            writer.WriteUInt32((uint)Objects.Count());
            foreach (var item in Objects) {
                writer.WriteItem<Stab>(item);
            }
            writer.WriteUInt16((ushort)Buildings.Count());
            writer.WriteBool(RestrictionTable != null, 2);
            foreach (var item in Buildings) {
                writer.WriteItem<BuildingInfo>(item);
            }
            if (RestrictionTable != null && RestrictionTable.Count() > 0) {
                writer.WriteUInt16((ushort)RestrictionTable.Count());
                writer.WriteUInt16(0);
                foreach (var kv in RestrictionTable) {
                    writer.WriteUInt32(kv.Key);
                    writer.WriteUInt32(kv.Value);
                }
            }
            return true;
        }

    }

}
