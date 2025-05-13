using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Enums {
    /// <summary>
    /// BSP node types
    /// </summary>
    public enum BSPNodeType : uint {
        /// <summary>
        /// Leaf node type. "LEAF"
        /// </summary>
        Leaf = 1279607110,

        /// <summary>
        /// Portal node type. "PORT"
        /// </summary>
        Portal = 1347375700,

        /// <summary>
        /// "BPnn"
        /// </summary>
        BPnn = 0x42506E6E,

        /// <summary>
        /// "BPIn"
        /// </summary>
        BPIn = 0x4250496E,

        /// <summary>
        /// "BpIN"
        /// </summary>
        BpIN = 0x4270494E,

        /// <summary>
        /// "BpnN"
        /// </summary>
        BpnN = 0x42706E4E,

        /// <summary>
        /// "BPIN"
        /// </summary>
        BPIN = 0x4250494E,

        /// <summary>
        /// "BPnN"
        /// </summary>
        BPnN = 0x42506E4E,

        /// <summary>
        /// "BPOL"
        /// </summary>
        BPOL = 0x42504F4C,

        /// <summary>
        /// "BPFL"
        /// </summary>
        BPFL = 0x4250464C
    }
}
