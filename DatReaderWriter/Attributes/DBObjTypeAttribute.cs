using ACClientLib.DatReaderWriter.Enums;
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
        /// Wether this DBObj has a data category
        /// </summary>
        public bool HasDataCategory { get; }

        /// <summary>
        /// The lower limit of ids used for this DBObjType
        /// </summary>
        public uint FirstId { get; }
        
        /// <summary>
        /// The upper limit of ids used for this DBObjType
        /// </summary>
        public uint LastId { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datFileType">The dat file type this DBObj exists in</param>
        /// <param name="hasDataCategory">Wether this DBObj has a data category</param>
        /// <param name="firstId">The lower limit of ids used for this DBObjType</param>
        /// <param name="lastId">The upper limit of ids used for this DBObjType</param>
        public DBObjTypeAttribute(DatFileType datFileType, bool hasDataCategory, uint firstId, uint lastId) {
            DatFileType = datFileType;
            HasDataCategory = hasDataCategory;
            FirstId = firstId;
            LastId = lastId;
        }
    }
}