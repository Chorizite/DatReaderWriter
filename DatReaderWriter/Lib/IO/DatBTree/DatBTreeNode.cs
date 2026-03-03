using DatReaderWriter.Lib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace DatReaderWriter.Lib.IO.DatBTree {
    /// <summary>
    /// A node / directory entry in the dat file.
    /// </summary>
    public class DatBTreeNode : IPackable, IUnpackable {
        /// <summary>
        /// The maximum number of branches a node can have (degree * 2)
        /// </summary>
        public const int MAX_BRANCHES = 62;

        /// <summary>
        /// The maximum number of file entries a node can have (degree * 2 - 1)
        /// </summary>
        public const int MAX_FILES = 61;

        /// <summary>
        /// The maximum size this struct will be
        /// </summary>
        public static readonly int SIZE = 1720;

        /// <summary>
        /// The offset in the dat file of this node
        /// </summary>
        public int Offset { get; internal set; }

        /// <summary>
        /// Fixed-size array of branch offsets.
        /// </summary>
        public readonly int[] Branches = new int[MAX_BRANCHES];

        /// <summary>
        /// The number of valid entries in <see cref="Branches"/>.
        /// </summary>
        public int BranchCount;

        /// <summary>
        /// Fixed-size array of file entries.
        /// </summary>
        public readonly DatBTreeFile[] Files = new DatBTreeFile[MAX_FILES];

        /// <summary>
        /// The number of valid entries in <see cref="Files"/>.
        /// </summary>
        public int FileCount;

        /// <summary>
        /// Whether this is a leaf node. Leaf nodes have no branches.
        /// </summary>
        public bool IsLeaf {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => BranchCount == 0;
        }

        /// <summary>
        /// Create a new node
        /// </summary>
        /// <param name="blockOffset">The offset of this node</param>
        internal DatBTreeNode(int blockOffset = 0) {
            Offset = blockOffset;
        }

        /// <inheritdoc/>
        public int GetSize() => SIZE;

        #region Array helpers for write path

        /// <summary>
        /// Insert a file at the specified index, shifting elements right.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InsertFile(int index, DatBTreeFile file) {
            // Shift right
            for (int i = FileCount; i > index; i--) {
                Files[i] = Files[i - 1];
            }
            Files[index] = file;
            FileCount++;
        }

        /// <summary>
        /// Add a file at the end.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFile(DatBTreeFile file) {
            Files[FileCount++] = file;
        }

        /// <summary>
        /// Remove the file at the specified index, shifting elements left.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveFileAt(int index) {
            FileCount--;
            for (int i = index; i < FileCount; i++) {
                Files[i] = Files[i + 1];
            }
            Files[FileCount] = default;
        }

        /// <summary>
        /// Insert a branch at the specified index, shifting elements right.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InsertBranch(int index, int branchOffset) {
            for (int i = BranchCount; i > index; i--) {
                Branches[i] = Branches[i - 1];
            }
            Branches[index] = branchOffset;
            BranchCount++;
        }

        /// <summary>
        /// Add a branch at the end.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddBranch(int branchOffset) {
            Branches[BranchCount++] = branchOffset;
        }

        /// <summary>
        /// Remove the branch at the specified index, shifting elements left.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveBranchAt(int index) {
            BranchCount--;
            for (int i = index; i < BranchCount; i++) {
                Branches[i] = Branches[i + 1];
            }
            Branches[BranchCount] = 0;
        }

        /// <summary>
        /// Copy files from source node starting at srcIndex for count items, appending to this node.
        /// </summary>
        public void AppendFilesFrom(DatBTreeNode source, int srcIndex, int count) {
            Array.Copy(source.Files, srcIndex, Files, FileCount, count);
            FileCount += count;
        }

        /// <summary>
        /// Copy branches from source node starting at srcIndex for count items, appending to this node.
        /// </summary>
        public void AppendBranchesFrom(DatBTreeNode source, int srcIndex, int count) {
            Array.Copy(source.Branches, srcIndex, Branches, BranchCount, count);
            BranchCount += count;
        }

        /// <summary>
        /// Remove a range of files starting at index for count items.
        /// </summary>
        public void RemoveFileRange(int index, int count) {
            int remaining = FileCount - index - count;
            if (remaining > 0) {
                Array.Copy(Files, index + count, Files, index, remaining);
            }
            // Clear the vacated slots
            for (int i = FileCount - count; i < FileCount; i++) {
                Files[i] = default;
            }
            FileCount -= count;
        }

        /// <summary>
        /// Remove a range of branches starting at index for count items.
        /// </summary>
        public void RemoveBranchRange(int index, int count) {
            int remaining = BranchCount - index - count;
            if (remaining > 0) {
                Array.Copy(Branches, index + count, Branches, index, remaining);
            }
            for (int i = BranchCount - count; i < BranchCount; i++) {
                Branches[i] = 0;
            }
            BranchCount -= count;
        }

        /// <summary>
        /// Replace all files with files from source, prepending source files before existing ones.
        /// </summary>
        public void PrependFilesFrom(DatBTreeNode source) {
            // Shift existing files right by source.FileCount
            var srcCount = source.FileCount;
            for (int i = FileCount - 1; i >= 0; i--) {
                Files[i + srcCount] = Files[i];
            }
            // Copy source files to the front
            Array.Copy(source.Files, 0, Files, 0, srcCount);
            FileCount += srcCount;
        }

        /// <summary>
        /// Replace all branches with branches from source, prepending source branches before existing ones.
        /// </summary>
        public void PrependBranchesFrom(DatBTreeNode source) {
            var srcCount = source.BranchCount;
            for (int i = BranchCount - 1; i >= 0; i--) {
                Branches[i + srcCount] = Branches[i];
            }
            Array.Copy(source.Branches, 0, Branches, 0, srcCount);
            BranchCount += srcCount;
        }

        #endregion

        /// <inheritdoc/>
        public bool Unpack(DatBinReader reader) {
            // Read file count first — it's at offset 62 * 4 = 248 bytes from start
            // We can peek ahead to know how many branches we actually need
            var startOffset = reader.Offset;

            // Skip branches for now, go directly to file count
            reader.Skip(MAX_BRANCHES * 4);
            int fileCount = reader.ReadInt32();

            // Go back and read only the needed branches
            // For non-leaf: branchCount = fileCount + 1, for leaf: 0
            // We need to determine if it's a leaf by reading the first branch
            reader.Skip(-(MAX_BRANCHES * 4 + 4));  // rewind to start of branches

            BranchCount = 0;
            FileCount = 0;

            // Read branches — we need to detect actual valid branches
            bool didFindEnd = false;
            var lastBranch = 0;
            for (int i = 0; i < MAX_BRANCHES; i++) {
                var branch = reader.ReadInt32();
                if (branch == 0 || branch == lastBranch || branch == unchecked((int)0xCDCDCDCD)) {
                    didFindEnd = true;
                }

                if (!didFindEnd) {
                    lastBranch = branch;
                    Branches[BranchCount++] = branch;
                }
            }

            // Skip file count (already read)
            reader.Skip(4);

            // Read file entries
            for (int i = 0; i < fileCount; i++) {
                Files[i] = new DatBTreeFile();
                Files[i].Unpack(reader);
            }
            FileCount = fileCount;

            // Trim branches to match file count
            if (BranchCount > 0 && FileCount > 0) {
                BranchCount = FileCount + 1;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool Pack(DatBinWriter writer) {
            for (var i = 0; i < MAX_BRANCHES; i++) {
                if (i < BranchCount) {
                    writer.WriteInt32(Branches[i]);
                }
                else if (FileCount == 0) {
                    writer.WriteInt32(0);
                }
                else {
                    writer.WriteInt32(unchecked((int)0xCDCDCDCD));
                }
            }

            writer.WriteInt32(FileCount);
            for (int i = 0; i < FileCount; i++) {
                Files[i].Pack(writer);
            }

            if (!IsLeaf && BranchCount != FileCount + 1) {
                throw new Exception($"PACK Branches.Count != Files.Count + 1 ({BranchCount} != {FileCount} + 1)");
            }

            return true;
        }

        /// <inheritdoc/>
        public override string ToString() {
            var str = new StringBuilder();

            str.AppendLine($"DatBTreeNode @ 0x{Offset:X8}:");
            str.Append("Branches: [");
            for (int i = 0; i < BranchCount; i++) {
                if (i > 0) str.Append(' ');
                str.Append(Branches[i].ToString("X8"));
            }
            str.AppendLine("]");
            str.Append("Files: [");
            for (int i = 0; i < FileCount; i++) {
                if (i > 0) str.Append(' ');
                str.Append(Files[i].Id.ToString("X8"));
            }
            str.AppendLine("]");

            return str.ToString();
        }
    }
}
