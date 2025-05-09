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
        internal int? _propKey;

        public abstract BasePropertyType PropertyType { get; }

        /// <inheritdoc />
        public virtual bool Unpack(DatBinReader reader) {
            if (_includeType) {
                _propKey = reader.ReadInt32();
                if (reader.Database?.DatCollection is null) {
                    throw new Exception("reader.Database.DatCollection is null! Unable to read MasterProperties and unpack StateDesc. Use DatCollection instead of creating a standalone DatDatabase");
                }
                if (!reader.Database.DatCollection.TryReadFile<MasterProperty>(0x39000001u, out var masterProperty)) {
                    throw new Exception("Unable to read MasterProperty (0x39000001)");
                }
                var _propLookup = masterProperty.Properties[(uint)_propKey].Type;
            }
            return true;
        }

        /// <inheritdoc />
        public virtual bool Pack(DatBinWriter writer) {
            if (_includeType) {
                if (writer.Database?.DatCollection is null) {
                    throw new Exception("reader.Database.DatCollection is null! Unable to read MasterProperties and unpack StateDesc. Use DatCollection instead of creating a standalone DatDatabase");
                }
                if (!writer.Database.DatCollection.TryReadFile<MasterProperty>(0x39000001u, out var masterProperty)) {
                    throw new Exception("Unable to read MasterProperty (0x39000001)");
                }

                var _propLookup = _propKey.HasValue ? (uint)_propKey.Value : masterProperty.Properties.FirstOrDefault((kv) => {
                    return kv.Value.Type == PropertyType;
                }).Key;

                writer.WriteInt32((int)_propLookup);
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
