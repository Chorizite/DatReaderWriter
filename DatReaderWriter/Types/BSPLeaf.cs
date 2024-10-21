using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
///
/// From ACE
///
namespace ACClientLib.DatReaderWriter.Types {
    public class BSPLeaf : BSPNode {
        public int LeafIndex { get; private set; }
        public int Solid { get; private set; }

        public override void Unpack(DatFileReader reader, BSPTreeType treeType) {
            Type = Encoding.ASCII.GetString(reader.ReadBytes(4).Reverse().ToArray());

            LeafIndex = reader.ReadInt32();

            if (treeType == BSPTreeType.Physics) {
                Solid = reader.ReadInt32();

                // Note that if Solid is equal to 0, these values will basically be null. Still read them, but they don't mean anything.
                Sphere = reader.ReadItem<Sphere>();

                InPolys = new List<ushort>();
                uint numPolys = reader.ReadUInt32();
                for (uint i = 0; i < numPolys; i++) {
                    InPolys.Add(reader.ReadUInt16());
                }
            }
        }

        public override void Pack(DatFileWriter writer, BSPTreeType treeType) {
            var typeBytes = Encoding.ASCII.GetBytes(Type).Reverse().ToArray();
            writer.WriteBytes(typeBytes, typeBytes.Length);

            writer.WriteInt32(LeafIndex);

            if (treeType == BSPTreeType.Physics) {
                writer.WriteInt32(Solid);

                // Note that if Solid is equal to 0, these values will basically be null. Still write them, but they don't mean anything.
                writer.WriteItem(Sphere);

                writer.WriteUInt32((uint)InPolys.Count);
                foreach (var inPoly in InPolys) {
                    writer.WriteUInt16(inPoly);
                }
            }
        }
    }
}
