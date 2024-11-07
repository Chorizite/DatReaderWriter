using DatReaderWriter.Enums;
using DatReaderWriter.Lib.IO;
using System;
using System.IO;
///
/// From ACE
///
namespace DatReaderWriter.Types {
    /// <summary>
    /// Physics BSP Tree
    /// </summary>
    public class PhysicsBSPTree : BSPTree, IDatObjType {
        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            Pack(writer, BSPTreeType.Physics);
            return true;
        }

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Unpack(reader, BSPTreeType.Physics);
            return true;
        }
    }

    /// <summary>
    /// Drawing BSP Tree
    /// </summary>
    public class DrawingBSPTree : BSPTree, IDatObjType {
        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            Pack(writer, BSPTreeType.Drawing);
            return true;
        }

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Unpack(reader, BSPTreeType.Drawing);
            return true;
        }
    }

    /// <summary>
    /// Cell BSP Tree
    /// </summary>
    public class CellBSPTree : BSPTree, IDatObjType {
        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            Pack(writer, BSPTreeType.Cell);
            return true;
        }

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Unpack(reader, BSPTreeType.Cell);
            return true;
        }
    }

    public abstract class BSPTree {
        public BSPNode RootNode { get; set; } = new BSPNode();

        public void Unpack(DatBinReader reader, BSPTreeType treeType) {
            RootNode = BSPNode.ReadNode(reader, treeType);
        }
        public void Pack(DatBinWriter writer, BSPTreeType treeType) {
            RootNode.Pack(writer, treeType);
        }
    }
}
