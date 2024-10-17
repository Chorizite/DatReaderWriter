using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACClientLib.DatReaderWriter.Enums {
    /// <summary>
    /// BSPTree type
    /// </summary>
    public enum BSPTreeType : uint {
        /// <summary>
        /// Drawing BSP Tree
        /// </summary>
        Drawing = 0,

        /// <summary>
        /// Physics BSP Tree
        /// </summary>
        Physics = 1,

        /// <summary>
        /// Cell BSP Tree
        /// </summary>
        Cell = 2,
    }
}
