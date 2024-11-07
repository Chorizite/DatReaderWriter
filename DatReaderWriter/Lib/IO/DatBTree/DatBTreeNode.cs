using DatReaderWriter.Lib.IO;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DatReaderWriter.Lib.IO.DatBTree {
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
        /// </summary>
        /// <remarks>
        /// Except on leaf nodes, Branches.Count must always be equal to Files.Count + 1.
        /// </remarks>
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
        public int GetSize() => SIZE;

        /// <inheritdoc/>
        unsafe public bool Unpack(DatBinReader reader) {
            Span<int> iSpan = [0];

            bool didFindEnd = false;
            var lastBranch = 0;
            for (iSpan[0] = 0; iSpan[0] < 62; iSpan[0]++) {
                var branch = reader.ReadInt32();
                if (branch == 0 || branch == lastBranch || branch == unchecked((int)0xCDCDCDCD)) {
                    didFindEnd = true;
                }

                if (!didFindEnd) {
                    lastBranch = branch;
                    Branches.Add(branch);
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

            if (Branches.Count > 0) {
                Branches = Branches.GetRange(0, Files.Count + 1);
            }

            return true;
        }

        /// <inheritdoc/>
        public bool Pack(DatBinWriter writer) {
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

            if (!IsLeaf && Branches.Count != Files.Count + 1) {
                throw new Exception($"PACK Branches.Count != Files.Count -+ 1 ({Branches.Count} != {Files.Count} + 1)");
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
