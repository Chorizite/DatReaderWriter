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
        public abstract BasePropertyType PropertyType { get; }

        /// <summary>
        /// The type key, from MasterProperties (not valid for BaseProperties inside MasterProperties)
        /// </summary>
        public uint MasterPropertyId { get; set; }

        /// <summary>
        /// True if this property should pack its type. This should be set to true
        /// except for MasterProperties
        /// </summary>
        public bool ShouldPackMasterPropertyId { get; set; }

        /// <inheritdoc />
        public virtual bool Unpack(DatBinReader reader) {
            return true;
        }

        /// <inheritdoc />
        public virtual bool Pack(DatBinWriter writer) {
            if (ShouldPackMasterPropertyId) {
                writer.WriteUInt32(MasterPropertyId);
            }
            return true;
        }

        /// <summary>
        /// Create a typed instance of this abstract class, used for MasterProperties
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static BaseProperty? UnpackGenericMasterProperty(DatBinReader reader, BasePropertyType type) {
            return UnpackInstanceFromType(reader, type, false, 0);
        }

        /// <summary>
        /// Create a typed instance of this abstract class
        /// </summary>
        public static BaseProperty UnpackGeneric(DatBinReader reader) {
            MasterProperty? masterProperty = null;
            if (masterProperty is null && reader.Database is not null && reader.Database is PortalDatabase portalDatabase) {
                masterProperty = portalDatabase.MasterProperty;
            }
            masterProperty ??= reader.Database?.DatCollection?.Portal?.MasterProperty;

            if (masterProperty is null) {
                throw new Exception($"writer.Database.DatCollection is null! Unable to read MasterProperties and pack {typeof(BaseProperty).Name}. Use DatCollection instead of creating a standalone DatDatabase");
            }

            var key = reader.ReadUInt32();
            var type = masterProperty.Properties[key].Type;
            
            return UnpackInstanceFromType(reader, type, true, key);
        }

        private static BaseProperty UnpackInstanceFromType(DatBinReader reader, BasePropertyType type, bool shouldPackType, uint key) {
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
            instance.ShouldPackMasterPropertyId = shouldPackType;
            instance.MasterPropertyId = key;
            instance.Unpack(reader);
            return instance;
        }
    }

}
