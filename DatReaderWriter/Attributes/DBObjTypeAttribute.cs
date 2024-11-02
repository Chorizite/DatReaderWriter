using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.Lib;
using System;

namespace ACClientLib.DatReaderWriter.Attributes {
    /// <summary>
    /// Mark a DBObj with definitions
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DBObjTypeAttribute : Attribute {
        /// <summary>
        /// The dat file type this DBObj exists in
        /// </summary>
        public DatFileType DatFileType { get; }

        /// <summary>
        /// The DBObj type
        /// </summary>
        public DBObjType DBObjType { get; }

        /// <summary>
        /// DBObj header flags. Determines what fields will be packed/unpacked.
        /// </summary>
        public DBObjHeaderFlags HeaderFlags { get; }

        /// <summary>
        /// The lower limit of ids used for this DBObjType
        /// </summary>
        public uint FirstId { get; }

        /// <summary>
        /// The upper limit of ids used for this DBObjType
        /// </summary>
        public uint LastId { get; }

        /// <summary>
        /// The mask used to determine ids used for this DBObjType
        /// </summary>
        public uint MaskId { get; }

        /// <summary>
        /// The DBObj class type
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Determines if this DBObjType is singular entry
        /// </summary>
        public bool IsSingular => FirstId == LastId && FirstId != 0;

        /// <summary>
        /// Determines if this DBObjType has range of valid ids
        /// </summary>
        public bool HasRangeData => FirstId != 0 || LastId != 0;

        /// <summary>
        /// Determines if this DBObjType uses a mask
        /// </summary>
        public bool HasMask => MaskId != 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">The DBObj class type</param>
        /// <param name="datFileType">The dat file type this DBObj exists in</param>
        /// <param name="dbObjType">The DBObj type</param>
        /// <param name="headerFlags">DBObj header flags. Determines what fields will be packed/unpacked.</param>
        /// <param name="firstId">The lower limit of ids used for this DBObjType</param>
        /// <param name="lastId">The upper limit of ids used for this DBObjType</param>
        /// <param name="maskId">The mask used to determine ids used for this DBObjType</param>
        public DBObjTypeAttribute(Type type, DatFileType datFileType, DBObjType dbObjType, DBObjHeaderFlags headerFlags, uint firstId, uint lastId, uint maskId) {
            Type = type;
            DatFileType = datFileType;
            DBObjType = dbObjType;
            HeaderFlags = headerFlags;
            FirstId = firstId;
            LastId = lastId;
            MaskId = maskId;
        }
    }
}