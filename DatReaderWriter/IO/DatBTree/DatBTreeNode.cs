using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ACClientLIb.DatReaderWriter.IO.DatBTree {
    /// <summary>
    /// A node / directory entry in the dat file.
    /// </summary>
    public class DatBTreeNode : IPackable, IUnpackable {
        /// <summary>
        /// The maximum size this struct will be
        /// </summary>
        public static readonly int SIZE = 1720;

        /// <summary>
        /// The offset in the dat file of this node
        /// </summary>
        public int Offset { get; internal set; }

        /// <summary>
        /// A list of branches / child node ids directly contained in this node.
        /// Except on leaf nodes, Branches.Count must always be equal to Files.Count - 1.
        /// </summary>
        public List<int> Branches { get; set; } = [];

        /// <summary>
        /// A list of file ids (keys) directly contained in this node.
        /// Except for root nodes, this must be between <see cref="DatBTreeReaderWriter.MinItems"/>
        /// and <see cref="DatBTreeReaderWriter.MaxItems"/> in length.
        /// </summary>
        public List<DatBTreeFile> Files { get; set; } = [];

        /// <summary>
        /// Wether this is a leaf node. Leaf nodes have no branches, only files.
        /// </summary>
        public bool IsLeaf => Branches.Count == 0;

        /// <summary>
        /// Create a new node
        /// </summary>
        /// <param name="blockOffset">The offset of this node</param>
        internal DatBTreeNode(int blockOffset = 0) {
            Offset = blockOffset;
        }

        /// <inheritdoc/>
        unsafe public bool Unpack(DatFileReader reader) {
            Span<int> branchSpan = stackalloc int[1];
            Span<int> iSpan = [0];

            bool didFindEnd = false;
            for (iSpan[0] = 0; iSpan[0] < 62; iSpan[0]++) {
                branchSpan[0] = reader.ReadInt32();

                if (branchSpan[0] == 0 || branchSpan[0] == unchecked((int)0xCDCDCDCD)) {
                    didFindEnd = true;
                }

                if (!didFindEnd) {
                    Branches.Add(branchSpan[0]);
                }
            }

            Span<int> entryCountSpan = [0, reader.ReadInt32()];

            for (entryCountSpan[0] = 0; entryCountSpan[0] < entryCountSpan[1]; entryCountSpan[0]++) {
                var file = new DatBTreeFile {
                    Parent = this
                };
                file.Unpack(reader);
                Files.Add(file);
            }

            return true;
        }

        /// <inheritdoc/>
        public bool Pack(DatFileWriter writer) {
            for (var i = 0; i < 62; i++) {
                if (Branches.Count > i) {
                    writer.WriteInt32(Branches[i]);
                }
                else if (Files.Count == 0) {
                    writer.WriteInt32(0);
                }
                else {
                    writer.WriteInt32(unchecked((int)0xCDCDCDCD));
                }
            }

            writer.WriteInt32(Files.Count);
            foreach (var file in Files) {
                file.Pack(writer);
            }

            return true;
        }

        /// <inheritdoc/>
        public override string ToString() {
            var str = new StringBuilder();

            str.AppendLine($"DatBTreeNode @ 0x{Offset:X8}:");
            str.AppendLine($"Branches: [{string.Join(" ", Branches.Select(b => b.ToString("X8")))}]");
            str.AppendLine($"Files: [{string.Join(" ", Files.Select(b => b.Id.ToString("X8")))}]");

            return str.ToString();
        }
    }
}
