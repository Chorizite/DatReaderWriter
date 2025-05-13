using DatReaderWriter.Enums;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.Types {
    /// <summary>
    /// Abstract base class for BSP trees.
    /// </summary>
    public abstract class BSPTree<TNode> : IDatObjType where TNode : BSPNode {
        /// <summary>
        /// Root node of the BSP tree.
        /// </summary>
        public TNode? Root { get; set; }

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Root = reader.ReadItem<TNode>();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            Root?.Pack(writer);
            return true;
        }
    }

    /// <summary>
    /// Cell BSP Tree
    /// </summary>
    public class CellBSPTree : BSPTree<CellBSPNode> {
    }

    /// <summary>
    /// Physics BSP Tree
    /// </summary>
    public class PhysicsBSPTree : BSPTree<PhysicsBSPNode> {
    }

    /// <summary>
    /// Drawing BSP Tree
    /// </summary>
    public class DrawingBSPTree : BSPTree<DrawingBSPNode> {
    }
}
