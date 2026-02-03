using System;

namespace DatReaderWriter.Lib.IO.DatBTree {
    /// <summary>
    /// Flags for a dat file entry.
    /// </summary>
    [Flags]
    public enum DatBTreeFileFlags : ushort {
        /// <summary>
        /// No flags.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// This file is compressed using ZLib.
        /// </summary>
        IsCompressed = 0x01,
    }
}
