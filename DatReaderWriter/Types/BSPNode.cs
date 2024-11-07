using DatReaderWriter.Enums;
using DatReaderWriter.Lib.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
///
/// From ACE
///
namespace DatReaderWriter.Types {
    public class BSPNode {
        public string Type { get; set; }
        public Plane SplittingPlane { get; set; }
        public BSPNode PosNode { get; set; }
        public BSPNode NegNode { get; set; }
        public Sphere Sphere { get; set; } = new Sphere();
        public List<ushort> InPolys { get; set; } = []; // List of PolygonIds

        public virtual void Unpack(DatBinReader reader, BSPTreeType treeType) {
            Type = Encoding.ASCII.GetString(reader.ReadBytes(4).Reverse().ToArray());

            switch (Type) {
                // These types will unpack the data completely, in their own classes
                case "PORT":
                case "LEAF":
                    throw new Exception();
            }

            SplittingPlane = reader.ReadPlane();

            switch (Type) {
                case "BPnn":
                case "BPIn":
                    PosNode = BSPNode.ReadNode(reader, treeType);
                    break;
                case "BpIN":
                case "BpnN":
                    NegNode = BSPNode.ReadNode(reader, treeType);
                    break;
                case "BPIN":
                case "BPnN":
                    PosNode = BSPNode.ReadNode(reader, treeType);
                    NegNode = BSPNode.ReadNode(reader, treeType);
                    break;
                default:
                    //Console.WriteLine($"Unhandled Node Type: {Type} (0x{string.Join("", Type.ToArray().Select(c => $"{(byte)c:X2}"))})");
                    break;
            }

            if (treeType == BSPTreeType.Cell)
                return;

            Sphere = reader.ReadItem<Sphere>();

            if (treeType == BSPTreeType.Physics)
                return;

            InPolys = new List<ushort>();
            uint numPolys = reader.ReadUInt32();
            for (uint i = 0; i < numPolys; i++)
                InPolys.Add(reader.ReadUInt16());
        }

        public static BSPNode ReadNode(DatBinReader reader, BSPTreeType treeType) {
            // We peek forward to get the type, then revert our position.
            var type = Encoding.ASCII.GetString(reader.ReadBytes(4).Reverse().ToArray());
            reader.Skip(-4);

            BSPNode node;

            switch (type) {
                case "PORT":
                    node = new BSPPortal();
                    break;

                case "LEAF":
                    node = new BSPLeaf();
                    break;

                case "BPnn":
                case "BPIn":
                case "BpIN":
                case "BpnN":
                case "BPIN":
                case "BPnN":
                default:
                    node = new BSPNode();
                    break;
            }

            node.Unpack(reader, treeType);

            return node;
        }

        public virtual void Pack(DatBinWriter writer, BSPTreeType treeType) {
            var typeBytes = Encoding.ASCII.GetBytes(Type).Reverse().ToArray();
            writer.WriteBytes(typeBytes, typeBytes.Length);

            switch (Type) {
                // These types will pack the data completely, in their own classes
                case "PORT":
                case "LEAF":
                    throw new Exception();
            }

            writer.WritePlane(SplittingPlane);

            switch (Type) {
                case "BPnn":
                case "BPIn":
                    PosNode.Pack(writer, treeType);
                    break;
                case "BpIN":
                case "BpnN":
                    NegNode.Pack(writer, treeType);
                    break;
                case "BPIN":
                case "BPnN":
                    PosNode.Pack(writer, treeType);
                    NegNode.Pack(writer, treeType);
                    break;
                default:
                    throw new Exception();
            }

            if (treeType == BSPTreeType.Cell)
                return;

            writer.WriteItem(Sphere);

            if (treeType == BSPTreeType.Physics)
                return;

            writer.WriteUInt32((uint)InPolys.Count);
            foreach (var inPoly in InPolys) {
                writer.WriteUInt16(inPoly);
            }
        }
    }
}
