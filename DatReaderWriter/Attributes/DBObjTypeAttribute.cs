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
        /// 
        /// </summary>
        /// <param name="datFileType">The dat file type this DBObj exists in</param>
        /// <param name="headerFlags">DBObj header flags. Determines what fields will be packed/unpacked.</param>
        /// <param name="firstId">The lower limit of ids used for this DBObjType</param>
        /// <param name="lastId">The upper limit of ids used for this DBObjType</param>
        public DBObjTypeAttribute(DatFileType datFileType, DBObjHeaderFlags headerFlags, uint firstId, uint lastId) {
            DatFileType = datFileType;
            HeaderFlags = headerFlags;
            FirstId = firstId;
            LastId = lastId;
        }
    }
}