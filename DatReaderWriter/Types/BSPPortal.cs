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
    public class BSPPortal : BSPNode {
        public List<PortalPoly> InPortals { get; } = new List<PortalPoly>();

        /// <inheritdoc />
        public override void Unpack(DatBinReader reader, BSPTreeType treeType) {
            Type = Encoding.ASCII.GetString(reader.ReadBytes(4).Reverse().ToArray());

            SplittingPlane = reader.ReadPlane();

            PosNode = BSPNode.ReadNode(reader, treeType);
            NegNode = BSPNode.ReadNode(reader, treeType);

            if (treeType == BSPTreeType.Drawing) {
                Sphere = reader.ReadItem<Sphere>();

                var numPolys = reader.ReadUInt32();
                var numPortals = reader.ReadUInt32();

                InPolys = new List<ushort>();
                for (uint i = 0; i < numPolys; i++) {
                    InPolys.Add(reader.ReadUInt16());
                }

                for (uint i = 0; i < numPortals; i++) {
                    InPortals.Add(reader.ReadItem<PortalPoly>());
                }
            }
        }

        /// <inheritdoc />
        public override void Pack(DatBinWriter writer, BSPTreeType treeType) {
            var typeBytes = Encoding.ASCII.GetBytes(Type).Reverse().ToArray();
            writer.WriteBytes(typeBytes, typeBytes.Length);

            writer.WritePlane(SplittingPlane);

            PosNode.Pack(writer, treeType);
            NegNode.Pack(writer, treeType);

            if (treeType == BSPTreeType.Drawing) {
                writer.WriteItem(Sphere);

                writer.WriteUInt32((uint)InPolys.Count);
                writer.WriteUInt32((uint)InPortals.Count);

                foreach (var inPoly in InPolys) {
                    writer.WriteUInt16(inPoly);
                }

                foreach (var inPortal in InPortals) {
                    writer.WriteItem(inPortal);
                }
            }
        }
    }
}
