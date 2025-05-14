using DatReaderWriter.Enums;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.DBObjs {
    /// <summary>
    /// DB_TYPE_ACTIONMAP in the client.
    /// </summary>
    [DBObjType(typeof(DBProperties), DatFileType.Portal, DBObjType.DBProperties, DBObjHeaderFlags.HasId, 0x26000000, 0x2600FFFF, 0x00000000)]
    public class ActionMap : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.ActionMap;

        /// <summary>
        /// The input maps
        /// </summary>
        public Dictionary<uint, Dictionary<uint, ActionMapValue>> InputMaps = [];

        /// <summary>
        /// The string table
        /// </summary>
        public uint StringTableId { get; set; }

        /// <summary>
        /// The input maps
        /// </summary>
        public Dictionary<uint, InputsConflictsValue> ConflictingMaps = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            var _bucketSize = reader.ReadByte();
            var _numInputMaps = reader.ReadCompressedUInt();

            for (var i = 0; i < _numInputMaps; i++) {
                var _key = reader.ReadUInt32();

                var _childBucketSize = reader.ReadByte();
                var _childNumInputMaps = reader.ReadCompressedUInt();

                InputMaps.Add(_key, new Dictionary<uint, ActionMapValue>());

                for (var j = 0; j < _childNumInputMaps; j++) {
                    var _childKey = reader.ReadUInt32();
                    var _childVal = reader.ReadItem<ActionMapValue>();
                    InputMaps[_key].Add(_childKey, _childVal);
                }
            }

            StringTableId = reader.ReadUInt32();

            var _conflictingBucketSize = reader.ReadByte();
            var _numConflictingMaps = reader.ReadCompressedUInt();
            for (var i = 0; i < _numConflictingMaps; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadItem<InputsConflictsValue>();
                ConflictingMaps.Add(_key, _val);
            }

            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);

            writer.WriteByte(23); // bucket size
            writer.WriteCompressedUInt((uint)InputMaps.Count); // num input maps

            foreach (var kv in InputMaps) {
                writer.WriteUInt32(kv.Key);

                var _childBucketSize = CalculateBucketSize(InputMaps[kv.Key].Count);
                writer.WriteByte((byte)_childBucketSize);
                writer.WriteCompressedUInt((uint)InputMaps[kv.Key].Count);

                foreach (var kv2 in InputMaps[kv.Key]) {
                    writer.WriteUInt32(kv2.Key);
                    writer.WriteItem<ActionMapValue>(kv2.Value);
                }
            }

            writer.WriteUInt32(StringTableId);

            writer.WriteByte(1); // bucket size
            writer.WriteCompressedUInt((uint)ConflictingMaps.Count);
            foreach (var kv in ConflictingMaps) {
                writer.WriteUInt32(kv.Key);
                writer.WriteItem<InputsConflictsValue>(kv.Value);
            }

            return true;
        }

        /// <summary>
        /// This is a total hack in order to maintain compatibility with what is in the dats.
        /// </summary>
        /// <param name="childNumInputMaps"></param>
        /// <returns></returns>
        private int CalculateBucketSize(int childNumInputMaps) {
            if (childNumInputMaps == 21) return 11;
            return FindClosestNumber(childNumInputMaps, [11, 23, 47]);
        }

        private int FindClosestNumber(int target, int[] numbers) {
            var closest = numbers[0];
            var minDifference = Math.Abs(target - numbers[0]);

            foreach (var num in numbers) {
                var difference = Math.Abs(target - num);
                if (difference < minDifference) {
                    minDifference = difference;
                    closest = num;
                }
            }

            return closest;
        }

    }
}
