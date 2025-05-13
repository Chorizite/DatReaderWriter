using System;
using System.Numerics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DatReaderWriter.Enums;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;
using System.Text.RegularExpressions;

namespace DatReaderWriter.Types {
    public partial class BasePropertyDesc : IDatObjType {
        public uint Name { get; set; }
        public BasePropertyType Type { get; set; }
        public PropertyGroupName Group { get; set; }
        public uint Provider { get; set; }
        public uint Data { get; set; }
        public PatchFlags PatchFlags { get; set; }
        public BaseProperty? DefaultValue { get; set; }
        public BaseProperty? MaxValue { get; set; }
        public BaseProperty? MinValue { get; set; }
        public float PredictionTimeout { get; set; }
        public PropertyInheritanceType InheritanceType { get; set; }
        public PropertyDatFileType DatFileType { get; set; }
        public PropertyPropagationType PropagationType { get; set; }
        public PropertyCachingType CachingType { get; set; }
        public bool Required { get; set; }
        public bool ReadOnly { get; set; }
        public bool NoCheckpoint { get; set; }
        public bool Recorded { get; set; }
        public bool DoNotReplay { get; set; }
        public bool AbsoluteTimeStamp { get; set; }
        public bool Groupable { get; set; }
        public bool PropagateToChildren { get; set; }
        public Dictionary<uint, uint> AvailableProperties { get; set; } = [];

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Name = reader.ReadUInt32();
            Type = (BasePropertyType)reader.ReadUInt32();
            Group = (PropertyGroupName)reader.ReadUInt32();
            Provider = reader.ReadUInt32();
            Data = reader.ReadUInt32();
            PatchFlags = (PatchFlags)reader.ReadInt32();

            var _hasDefaultValue = reader.ReadBool(1);
            if (_hasDefaultValue) {
                DefaultValue = BaseProperty.UnpackGenericMasterProperty(reader, Type);
            }

            var _hasMaxValue = reader.ReadBool(1);
            if (_hasMaxValue) {
                MaxValue = BaseProperty.UnpackGenericMasterProperty(reader, Type);
            }
            var _hasMinValue = reader.ReadBool(1);
            if (_hasMinValue) {
                MinValue = BaseProperty.UnpackGenericMasterProperty(reader, Type);
            }

            PredictionTimeout = reader.ReadSingle();
            InheritanceType = (PropertyInheritanceType)reader.ReadByte();
            DatFileType = (PropertyDatFileType)reader.ReadByte();
            PropagationType = (PropertyPropagationType)reader.ReadByte();

            CachingType = (PropertyCachingType)reader.ReadByte();

            Required = reader.ReadBool(1);
            ReadOnly = reader.ReadBool(1);
            NoCheckpoint = reader.ReadBool(1);
            Recorded = reader.ReadBool(1);
            DoNotReplay = reader.ReadBool(1);
            AbsoluteTimeStamp = reader.ReadBool(1);
            Groupable = reader.ReadBool(1);
            PropagateToChildren = reader.ReadBool(1);

            var _bucketSize = reader.ReadByte();
            var _numAvailableProperties = reader.ReadByte();
            for (var i=0; i < _numAvailableProperties; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadUInt32();
                AvailableProperties.Add(_key, _val);
            }

            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(Name);
            writer.WriteUInt32((uint)Type);
            writer.WriteUInt32((uint)Group);
            writer.WriteUInt32(Provider);
            writer.WriteUInt32(Data);
            writer.WriteInt32((int)PatchFlags);

            writer.WriteBool(DefaultValue != null, 1);
            DefaultValue?.Pack(writer);

            writer.WriteBool(MaxValue != null, 1);
            MaxValue?.Pack(writer);

            writer.WriteBool(MinValue != null, 1);
            MinValue?.Pack(writer);

            writer.WriteSingle(PredictionTimeout);
            writer.WriteByte((byte)InheritanceType);
            writer.WriteByte((byte)DatFileType);
            writer.WriteByte((byte)PropagationType);
            writer.WriteByte((byte)CachingType);
            writer.WriteBool(Required, 1);
            writer.WriteBool(ReadOnly, 1);
            writer.WriteBool(NoCheckpoint, 1);
            writer.WriteBool(Recorded, 1);
            writer.WriteBool(DoNotReplay, 1);
            writer.WriteBool(AbsoluteTimeStamp, 1);
            writer.WriteBool(Groupable, 1);
            writer.WriteBool(PropagateToChildren, 1);

            writer.WriteByte((byte)1);
            writer.WriteByte((byte)AvailableProperties.Count);

            foreach (var kv in AvailableProperties) {
                writer.WriteUInt32(kv.Key);
                writer.WriteUInt32(kv.Value);
            }
            return true;
        }

    }

}
