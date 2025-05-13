using DatReaderWriter.Enums;
using DatReaderWriter.Lib.IO;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DatReaderWriter.Types {
    /// <summary>
    /// Represents a node in the Cell BSP tree.
    /// </summary>
    public class CellBSPNode : BSPNode {
        /// <summary>
        /// Positive child node (subspace on the positive side of the plane).
        /// </summary>
        public CellBSPNode? PosNode { get; set; }

        /// <summary>
        /// Negative child node (subspace on the negative side of the plane).
        /// </summary>
        public CellBSPNode? NegNode { get; set; }

        /// <summary>
        /// Index of the leaf (for leaf nodes).
        /// </summary>
        public int LeafIndex { get; set; }

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            Type = (BSPNodeType)reader.ReadUInt32();

            switch (Type) {
                case BSPNodeType.Portal:
                    throw new NotSupportedException("BSPPORTAL nodes are not supported in Cell BSP.");
                case BSPNodeType.Leaf:
                    LeafIndex = reader.ReadInt32();
                    break;

                default: // BPnn, BPIn, BpIN, BpnN, BPIN, BPnN
                    SplittingPlane = reader.ReadPlane();
                    switch (Type) {
                        case BSPNodeType.BPnn:
                        case BSPNodeType.BPIn:
                            PosNode = reader.ReadItem<CellBSPNode>();
                            NegNode = null;
                            break;
                        case BSPNodeType.BpIN:
                        case BSPNodeType.BpnN:
                            PosNode = null;
                            NegNode = reader.ReadItem<CellBSPNode>();
                            break;
                        case BSPNodeType.BPIN:
                        case BSPNodeType.BPnN:
                            PosNode = reader.ReadItem<CellBSPNode>();
                            NegNode = reader.ReadItem<CellBSPNode>();
                            break;
                        default:
                            break;
                    }
                    break;
            }

            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            writer.WriteUInt32((uint)Type);

            switch (Type) {
                case BSPNodeType.Portal:
                    throw new NotSupportedException("BSPPORTAL nodes are not supported in Cell BSP.");
                case BSPNodeType.Leaf:
                    writer.WriteInt32(LeafIndex);
                    break;
                default: // BPnn, BPIn, BpIN, BpnN, BPIN, BPnN
                    writer.WritePlane(SplittingPlane);
                    PosNode?.Pack(writer);
                    NegNode?.Pack(writer);
                    break;
            }

            return true;
        }
    }
}