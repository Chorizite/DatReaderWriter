using System;
using System.Collections.Generic;

namespace ACDatReader.IO.DatBTree {
    /// <summary>
    /// A node / directory entry in the dat file.
    /// </summary>
    public class DatBTreeNode : IPackable, IUnpackable {
        /// <summary>
        /// The maximum size this struct will be
        /// </summary>
        public static readonly int SIZE = 1720;

        /// <summary>
        /// A list of branches / child node ids directly contained in this node.
        /// Except on leaf nodes, Branches.Count must always be equal to Files.Count - 1.
        /// </summary>
        public List<int> Branches { get; } = [];

        /// <summary>
        /// A list of file ids (keys) directly contained in this node.
        /// Except for root nodes, this must be between <see cref="DatBTreeReaderWriter.MinItems"/>
        /// and <see cref="DatBTreeReaderWriter.MaxItems"/> in length.
        /// </summary>
        public List<int> Keys { get; } = [];

        /// <summary>
        /// Wether this is a leaf node. Leaf nodes have no branches, only files.
        /// </summary>
        public bool IsLeaf => Branches.Count == 0;

        /// <inheritdoc/>
        unsafe public bool Unpack(DatFileReader reader) {
            Span<int> branchSpan = stackalloc int[1];
            Span<int> iSpan = [0];
            for (iSpan[0] = 0; iSpan[0] < 62; iSpan[0]++) {
                branchSpan[0] = reader.ReadInt32();
                if (branchSpan[0] != 0 && branchSpan[0] != unchecked((int)0xCDCDCDCD)) {
                    Branches.Add(branchSpan[0]);
                }
            }

            Span<int> entryCountSpan = [0, reader.ReadInt32()];
            for (entryCountSpan[0] = 0; entryCountSpan[0] < entryCountSpan[1]; entryCountSpan[0]++) {
                var file = new DatBTreeFile();
                reader.Skip(4);
                Keys.Add(reader.ReadInt32());
                reader.Skip(DatBTreeFile.SIZE - 8);
            }

            return true;
        }

        /// <inheritdoc/>
        public bool Pack(DatFileWriter writer) {
            for (var i = 0; i < 62; i++) {
                if (Branches.Count > i) {
                    writer.WriteInt32(Branches[i]);
                }
                else if (Keys.Count == 0) {
                    writer.WriteInt32(0);
                }
                else {
                    writer.WriteInt32(unchecked((int)0xCDCDCDCD));
                }
            }

            writer.WriteInt32(Keys.Count);
            foreach (var file in Keys) {
                throw new NotImplementedException();
                //file.Pack(writer);
            }

            return true;
        }
    }
}
