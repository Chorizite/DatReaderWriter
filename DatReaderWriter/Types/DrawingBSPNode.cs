using DatReaderWriter.Enums;
using DatReaderWriter.Lib.IO;
using System.Collections.Generic;

namespace DatReaderWriter.Types {
    /// <summary>
    /// Represents a node in the Drawing BSP tree.
    /// </summary>
    public class DrawingBSPNode : BSPNode {
        /// <summary>
        /// Positive child node (subspace on the positive side of the plane).
        /// </summary>
        public DrawingBSPNode? PosNode { get; set; }

        /// <summary>
        /// Negative child node (subspace on the negative side of the plane).
        /// </summary>
        public DrawingBSPNode? NegNode { get; set; }

        /// <summary>
        /// Index of the leaf (for leaf nodes).
        /// </summary>
        public int LeafIndex { get; set; }

        /// <summary>
        /// Bounding sphere for the node.
        /// </summary>
        public Sphere BoundingSphere { get; set; } = new Sphere();

        /// <summary>
        /// Array of polygon references (for leaf and internal nodes).
        /// </summary>
        public List<ushort> Polygons { get; set; } = [];

        /// <summary>
        /// Array of portal references (for portal nodes).
        /// </summary>
        public List<PortalRef> Portals { get; set; } = [];

        public override bool Unpack(DatBinReader reader) {
            Type = (BSPNodeType)reader.ReadInt32();

            switch (Type) {
                case BSPNodeType.Portal:
                    SplittingPlane = reader.ReadPlane();
                    PosNode = reader.ReadItem<DrawingBSPNode>();
                    NegNode = reader.ReadItem<DrawingBSPNode>();
                    BoundingSphere = reader.ReadItem<Sphere>();

                    var numPolys = reader.ReadUInt32();
                    var numPortals = reader.ReadUInt32();

                    Polygons.Clear();
                    for (int i = 0; i < numPolys; i++) {
                        Polygons.Add(reader.ReadUInt16());
                    }

                    Portals.Clear();
                    for (int i = 0; i < numPortals; i++) {
                        Portals.Add(reader.ReadItem<PortalRef>());
                    }
                    break;

                case BSPNodeType.Leaf:
                    LeafIndex = reader.ReadInt32();
                    break;

                default: // BPnn, BPIn, BpIN, BpnN, BPIN, BPnN
                    SplittingPlane = reader.ReadPlane();
                    ReadPosNegNodes(reader);
                    BoundingSphere = reader.ReadItem<Sphere>();

                    numPolys = reader.ReadUInt32();
                    Polygons.Clear();
                    for (int i = 0; i < numPolys; i++) {
                        Polygons.Add(reader.ReadUInt16());
                    }
                    break;
            }

            return true;
        }

        private void ReadPosNegNodes(DatBinReader reader) {
            switch (Type) {
                case BSPNodeType.BPnn:
                case BSPNodeType.BPIn:
                    PosNode = reader.ReadItem<DrawingBSPNode>();
                    NegNode = null;
                    break;
                case BSPNodeType.BpIN:
                case BSPNodeType.BpnN:
                    PosNode = null;
                    NegNode = reader.ReadItem<DrawingBSPNode>();
                    break;
                case BSPNodeType.BPIN:
                case BSPNodeType.BPnN:
                    PosNode = reader.ReadItem<DrawingBSPNode>();
                    NegNode = reader.ReadItem<DrawingBSPNode>();
                    break;
                default:
                    break;
            }
        }

        public override bool Pack(DatBinWriter writer) {
            writer.WriteInt32((int)Type);

            switch (Type) {
                case BSPNodeType.Portal:
                    writer.WritePlane(SplittingPlane);
                    PosNode?.Pack(writer);
                    NegNode?.Pack(writer);
                    writer.WriteItem(BoundingSphere);

                    writer.WriteInt32(Polygons?.Count ?? 0);
                    writer.WriteInt32(Portals?.Count ?? 0);

                    if (Polygons != null) {
                        foreach (var poly in Polygons) {
                            writer.WriteUInt16(poly);
                        }
                    }
                    if (Portals != null) {
                        foreach (var portal in Portals) {
                            writer.WriteItem(portal);
                        }
                    }
                    break;

                case BSPNodeType.Leaf:
                    writer.WriteInt32(LeafIndex);
                    break;

                default: // BPnn, BPIn, BpIN, BpnN, BPIN, BPnN
                    writer.WritePlane(SplittingPlane);
                    PosNode?.Pack(writer);
                    NegNode?.Pack(writer);
                    writer.WriteItem(BoundingSphere);
                    writer.WriteInt32(Polygons?.Count ?? 0);
                    if (Polygons != null) {
                        foreach (var poly in Polygons) {
                            writer.WriteUInt16(poly);
                        }
                    }
                    break;
            }

            return true;
        }
    }
}
