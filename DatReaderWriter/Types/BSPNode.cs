using DatReaderWriter.Enums;
using DatReaderWriter.Lib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Types {
    /// <summary>
    /// Abstract base class for BSP nodes.
    /// </summary>
    public abstract class BSPNode : IDatObjType {
        /// <summary>
        /// Type of the node (e.g., Leaf, Portal, BPnn).
        /// </summary>
        public BSPNodeType Type { get; set; }

        /// <summary>
        /// Splitting plane for internal nodes.
        /// </summary>
        public Plane SplittingPlane { get; set; }

        /// <inheritdoc/>
        public abstract bool Unpack(DatBinReader reader);

        /// <inheritdoc/>
        public abstract bool Pack(DatBinWriter writer);
    }
}
