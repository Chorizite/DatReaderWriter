using System;
using System.Numerics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DatReaderWriter.Enums;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.DBObjs;

namespace DatReaderWriter.Types {
    public abstract partial class BaseProperty : IDatObjType {
        internal bool _includeType = true;
        public abstract BasePropertyType PropertyType { get; }

        /// <inheritdoc />
        public virtual bool Unpack(DatBinReader reader) {
            if (_includeType) {
                var _propKey = reader.ReadInt32();
            }
            return true;
        }

        /// <inheritdoc />
        public virtual bool Pack(DatBinWriter writer) {
            if (_includeType) {
                if (writer.Database is null) {
                    throw new Exception("Database is null");
                }

                if (!writer.Database.TryReadFile<MasterProperty>(0x39000001u, out var masterProperty)) {
                    throw new Exception("MasterProperty not found (0x39000001)");
                }

                var _propLookup = masterProperty.Properties.FirstOrDefault(kv => kv.Value.Type == PropertyType).Key;

                writer.WriteInt32((int)_propLookup);
            }
            return true;
        }

        /// <summary>
        /// Create a typed instance of this abstract class
        /// </summary>
        public static BaseProperty? Unpack(DatBinReader reader, BasePropertyType type, bool includeType = false) {
            BaseProperty? instance = null;
            switch (type) {
                case BasePropertyType.Enum:
                    instance = new EnumBaseProperty();
                    break;
                case BasePropertyType.Bool:
                    instance = new BoolBaseProperty();
                    break;
                case BasePropertyType.DataId:
                    instance = new DataIdBaseProperty();
                    break;
                case BasePropertyType.Float:
                    instance = new FloatBaseProperty();
                    break;
                case BasePropertyType.Integer:
                    instance = new IntegerBaseProperty();
                    break;
                case BasePropertyType.StringInfo:
                    instance = new StringInfoBaseProperty();
                    break;
                case BasePropertyType.Color:
                    instance = new ColorBaseProperty();
                    break;
                case BasePropertyType.Array:
                    instance = new ArrayBaseProperty();
                    break;
                case BasePropertyType.Struct:
                    instance = new StructBaseProperty();
                    break;
                case BasePropertyType.Vector:
                    instance = new VectorBaseProperty();
                    break;
                case BasePropertyType.Bitfield32:
                    instance = new Bitfield32BaseProperty();
                    break;
                case BasePropertyType.Bitfield64:
                    instance = new Bitfield64BaseProperty();
                    break;
                case BasePropertyType.InstanceId:
                    instance = new InstanceIdBaseProperty();
                    break;
                default:
                    throw new Exception($"Unsupported BaseProperty type: {type}");
            }
            instance._includeType = includeType;
            instance.Unpack(reader);
            return instance;
        }
    }

}
