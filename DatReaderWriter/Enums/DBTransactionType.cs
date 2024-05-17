using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACClientLib.DatReaderWriter.Enums {
    /// <summary>
    /// Dat BTree transaction type. Used when writing transactions header during tree modifications.
    /// </summary>
    public enum DBTransactionType : uint {
        /// <summary>
        /// No current transaction
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Add a file entry
        /// </summary>
        AddFileEntry = 0x1,

        /// <summary>
        /// Delete a leaf node
        /// </summary>
        DeleteLeaf = 0x2,

        /// <summary>
        /// Delete an internal node
        /// </summary>
        DeleteInternalNode = 0x3,

        /// <summary>
        /// Merge two nodes, because one is too small
        /// </summary>
        MergeNodes = 0x4,

        /// <summary>
        /// Update a file entry
        /// </summary>
        UpdateFileEntry = 0x5,

        /// <summary>
        /// Split a node into two when it becomes full
        /// </summary>
        SplitNode = 0x6,

        /// <summary>
        /// Key rotation
        /// </summary>
        RotateKeys = 0x7,

        /// <summary>
        /// Expand Least Recently Used cache
        /// </summary>
        LRUExpand = 0x8,

        /// <summary>
        /// Delete from Least Recently Used cache
        /// </summary>
        LRUDelete = 0x9,
    }
}
