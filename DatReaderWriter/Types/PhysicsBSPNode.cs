using DatReaderWriter.Enums;
using DatReaderWriter.Lib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Types {
    /// <summary>
    /// Represents a node in the Physics BSP tree.
    /// </summary>
    public class PhysicsBSPNode : BSPNode {
        /// <summary>
        /// Positive child node (subspace on the positive side of the plane).
        /// </summary>
        public PhysicsBSPNode? PosNode { get; set; }

        /// <summary>
        /// Negative child node (subspace on the negative side of the plane).
        /// </summary>
        public PhysicsBSPNode? NegNode { get; set; }

        /// <summary>
        /// Index of the leaf (for leaf nodes).
        /// </summary>
        public int LeafIndex { get; set; }

        /// <summary>
        /// Solid index (for leaf nodes).
        /// </summary>
        public int Solid { get; private set; }

        /// <summary>
        /// Bounding sphere for the node.
        /// </summary>
        public Sphere BoundingSphere { get; set; } = new Sphere();

        /// <summary>
        /// Array of polygon references (for leaf and internal nodes).
        /// </summary>
        public List<ushort> Polygons { get; set; } = [];

        /// <inheritdoc/>
        public override bool Unpack(DatBinReader reader) {
            Type = (BSPNodeType)reader.ReadInt32();

            switch (Type) {
                case BSPNodeType.Portal:
                    throw new NotSupportedException("BSPPORTAL nodes are not supported in Physics BSP.");

                case BSPNodeType.Leaf:
                    LeafIndex = reader.ReadInt32();
                    Solid = reader.ReadInt32();
                    BoundingSphere = reader.ReadItem<Sphere>();

                    var numPolys = reader.ReadUInt32();
                    Polygons.Clear();
                    for (int i = 0; i < numPolys; i++) {
                        Polygons.Add(reader.ReadUInt16());
                    }
                    break;

                default: // BPnn, BPIn, BpIN, BpnN, BPIN, BPnN
                    SplittingPlane = reader.ReadPlane();
                    switch (Type) {
                        case BSPNodeType.BPnn:
                        case BSPNodeType.BPIn:
                            PosNode = reader.ReadItem<PhysicsBSPNode>();
                            NegNode = null;
                            break;
                        case BSPNodeType.BpIN:
                        case BSPNodeType.BpnN:
                            PosNode = null;
                            NegNode = reader.ReadItem<PhysicsBSPNode>();
                            break;
                        case BSPNodeType.BPIN:
                        case BSPNodeType.BPnN:
                            PosNode = reader.ReadItem<PhysicsBSPNode>();
                            NegNode = reader.ReadItem<PhysicsBSPNode>();
                            break;
                        default:
                            break;
                    }
                    BoundingSphere = reader.ReadItem<Sphere>();
                    break;
            }

            return true;
        }

        /// <inheritdoc/>
        public override bool Pack(DatBinWriter writer) {
            writer.WriteInt32((int)Type);

            switch (Type) {
                case BSPNodeType.Portal:
                    throw new NotSupportedException("BSPPORTAL nodes are not supported in Physics BSP.");
                case BSPNodeType.Leaf:
                    writer.WriteInt32(LeafIndex);
                    writer.WriteInt32(Solid);
                    writer.WriteItem(BoundingSphere);

                    writer.WriteUInt32((uint)Polygons.Count);
                    for (int i = 0; i < Polygons.Count; i++) {
                        writer.WriteUInt16(Polygons[i]);
                    }
                    break;
                default: // BPnn, BPIn, BpIN, BpnN, BPIN, BPnN
                    writer.WritePlane(SplittingPlane);
                    PosNode?.Pack(writer);
                    NegNode?.Pack(writer);
                    writer.WriteItem(BoundingSphere);
                    break;
            }

            return true;
        }
    }
}
