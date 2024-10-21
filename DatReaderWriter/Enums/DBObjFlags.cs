using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Enums {
    /// <summary>
    /// Determins what information is stored in the DBObj header
    /// </summary>
    public enum DBObjFlags {
        /// <summary>
        /// Nothing is stored.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// The DBObj has an Id (uint32)
        /// </summary>
        HasId = 0x1,

        /// <summary>
        /// The DBObj has a DataCategory (uint32)
        /// </summary>
        HasDataCategory = 0x2,
    }
}
