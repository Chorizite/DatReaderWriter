using ACDatReader.IO.BlockAllocators;
using ACDatReader.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;

/*
 *  Heavily based on https://github.com/rsdcastro/btree-dotnet/ and https://github.com/msambol/dsa/blob/master/trees/b_tree.py
 */
namespace ACDatReader.IO.DatBTree {
    /// <summary>
    /// A dat BTree reader / writer.
    /// </summary>
    public class DatBTreeReaderWriter : IDisposable {
        /// <summary>
        /// Block allocator
        /// </summary>
        public IDatBlockAllocator BlockAllocator { get; }

        /// <summary>
        /// The "degree" of this BTree. Dat files have a degree of 30.
        /// This means at max nodes can have (30 * 2) + 1 file entries,
        /// or at a minimum they can have (30 - 1) file entries.
        /// </summary>
        public int Degree { get; } = 31;

        /// <summary>
        /// The root node of this tree.
        /// </summary>
        public DatBTreeNode? Root { get; private set; }

        /// <summary>
        /// The minimum amount of file entries that can be stored on a node.
        /// This is `<see cref="Degree"/> - 1`
        /// </summary>
        public int MinItems => Degree - 1;

        /// <summary>
        /// The maximum amount of file entries that can be stored on a node.
        /// This is <code>(<see cref="Degree"/> * 2) + 1</code>
        /// </summary>
        public int MaxItems => (Degree * 2) - 1;

        /// <summary>
        /// Create a new DatBTreeReaderWriter instance from a <see cref="IDatBlockAllocator"/>
        /// </summary>
        /// <param name="blockAllocator">The block allocator to use</param>
        public DatBTreeReaderWriter(IDatBlockAllocator blockAllocator) {
            BlockAllocator = blockAllocator;
            if (BlockAllocator.Header.RootBlock != 0 && TryGetNode(BlockAllocator.Header.RootBlock, out var root)) {
                Root = root;
            }
        }

        private bool TryGetNode(int blockOffset, [MaybeNullWhen(false)] out DatBTreeNode result) {
            var buffer = BaseBlockAllocator.SharedBytes.Rent(DatBTreeNode.SIZE);

            BlockAllocator.ReadBlock(buffer, blockOffset);
            result = new DatBTreeNode(blockOffset);
            var success = result.Unpack(new DatFileReader(buffer));

            BaseBlockAllocator.SharedBytes.Return(buffer);

            return success;
        }

        /// <summary>
        /// Write a node back to the dat
        /// </summary>
        /// <param name="node">The node to write</param>
        private void WriteNode(DatBTreeNode node) {
            var buffer = BaseBlockAllocator.SharedBytes.Rent(DatBTreeNode.SIZE);
            var writer = new DatFileWriter(buffer);
            node.Pack(writer);
            BlockAllocator.WriteBlock(buffer, DatBTreeNode.SIZE, node.Offset);
            BaseBlockAllocator.SharedBytes.Return(buffer);
        }

        private bool TryGetFileInternal(uint fileId, int startingBlock, [MaybeNullWhen(false)] out DatBTreeFile file) {
            // 0 and 0xCDCDCDCD are invalid node offsets
            while (startingBlock != 0 && startingBlock != unchecked((int)0xcdcdcdcd)) {
                if (TryGetNode(startingBlock, out var node)) {
                    var left = 0;
                    var right = node.Files.Count - 1;
                    var i = 0;

                    // binary search on keys
                    while (left <= right) {
                        i = (left + right) / 2;
                        var currentFile = node.Files[i];

                        if (fileId == currentFile.Id) {
                            file = currentFile;
                            return true;
                        }
                        else if (fileId < currentFile.Id)
                            right = i - 1;
                        else
                            left = i + 1;
                    }

                    // leaf is end of the line, so no result was found
                    if (node.IsLeaf) {
                        break;
                    }

                    if (fileId > node.Files[i].Id)
                        i++;

                    startingBlock = node.Branches[i];
                }
                else {
                    break;
                }
            }

            file = null;
            return false;
        }

        private void SetNewRoot(DatBTreeNode rootBlock) {
            Root = rootBlock;
            WriteNode(rootBlock);
            BlockAllocator.SetRootBlock(rootBlock.Offset);
        }

        /// <summary>
        /// Split a child node of <paramref name="parent"/>, where the child node
        /// is full
        /// </summary>
        /// <param name="parent">The parent node containing th full node</param>
        /// <param name="child">The full node</param>
        private void SplitChild(DatBTreeNode parent, DatBTreeNode child) {
            var newNode = new DatBTreeNode(BlockAllocator.ReserveBlock());
            var childIdx = parent.Branches.FindIndex(f => f == child.Offset);

            parent.Files.Insert(childIdx, child.Files[Degree - 1]);
            parent.Branches.Insert(childIdx + 1, newNode.Offset);

            newNode.Files.AddRange(child.Files.GetRange(Degree, Degree - 1));
            child.Files.RemoveRange(Degree - 1, Degree);

            if (!child.IsLeaf) {
                newNode.Branches.AddRange(child.Branches.GetRange(Degree, Degree));
                child.Branches.RemoveRange(Degree, Degree);
            }
            WriteNode(newNode);
            WriteNode(child);
            WriteNode(parent);
        }

        private void InsertNonFull(DatBTreeNode node, DatBTreeFile file) {
            int positionToInsert = node.Files.TakeWhile(entry => file.Id.CompareTo(entry.Id) >= 0).Count();
            
            if (node.IsLeaf) {
                node.Files.Insert(positionToInsert, file);
                WriteNode(node);
                return;
            }

            if (!TryGetNode(node.Branches[positionToInsert], out var child)) {
                throw new Exception("Could not look up child node during insertion!");
            }

            if (child.Files.Count == MaxItems) {
                SplitChild(node, child);
                if (file.Id.CompareTo(node.Files[positionToInsert].Id) > 0) {
                    positionToInsert++;

                    if (!TryGetNode(node.Branches[positionToInsert], out child)) {
                        throw new Exception("Could not look up child node during insertion!");
                    }
                }
            }

            InsertNonFull(child, file);
        }

        /// <summary>
        /// Try and get a file in the tree with the specified id
        /// </summary>
        /// <param name="fileId">The file id to search for</param>
        /// <param name="file">The file, or null if not found</param>
        /// <returns>true if the file was found, false otherwise</returns>
        public bool TryGetFile(uint fileId, [MaybeNullWhen(false)] out DatBTreeFile file) {
            return TryGetFileInternal(fileId, BlockAllocator.Header.RootBlock, out file);
        }

        /// <summary>
        /// Check if the specified fileId is contained in the tree
        /// </summary>
        /// <param name="fileId">The file id to check for</param>
        /// <returns>true if in the tree, false if not</returns>
        public bool HasFile(uint fileId) {
            return TryGetFile(fileId, out var _);
        }

        /// <summary>
        /// Inserts a <see cref="DatBTreeFile"/> into this tree, replacing a
        /// a file if one is already there.
        /// </summary>
        /// <param name="file">The file to add</param>
        /// <returns>The file that was replaced, if any</returns>
        public DatBTreeFile? Insert(DatBTreeFile file) {
            // check if file already exists
            if (TryGetFile(file.Id, out var foundFile)) {
                file.Parent = foundFile.Parent;

                if (foundFile.Parent is not null) {
                    var idx = foundFile.Parent.Files.IndexOf(foundFile);
                    foundFile.Parent.Files[idx] = file;
                    WriteNode(foundFile.Parent);
                }
                else {
                    throw new Exception($"Could not find existing parent?");
                }
                return foundFile;
            }

            // if root is empty, create a new root node and add the file there
            if (Root is null) {
                var rootBlock = new DatBTreeNode(BlockAllocator.ReserveBlock());
                rootBlock.Files.Add(file);
                SetNewRoot(rootBlock);
                return null;
            }

            // check if root node is full, if so create a new root node before insertion
            if (Root.Files.Count == MaxItems) {
                var oldRoot = Root;
                var newRoot = new DatBTreeNode(BlockAllocator.ReserveBlock());
                SetNewRoot(newRoot);

                // add old root to new root
                newRoot.Branches.Add(oldRoot.Offset);
                SplitChild(newRoot, oldRoot);
            }

            InsertNonFull(Root, file);

            return null;
        }

        /// <inheritdoc/>
        public override string ToString() {
            var str = new StringBuilder();

            str.AppendLine($"Tree:");
            if (Root is null) {
                str.AppendLine("\tnull");
            }
            else {
                WriteTree(str, Root, 0);
            }

            return str.ToString();
        }

        private void WriteTree(StringBuilder str, DatBTreeNode node, int depth) {
            var tabs = new string('\t', depth);
            str.AppendLine($"{tabs}\tNode: {node.Offset:X8} (Leaf: {node.IsLeaf}, Root: {node.Offset == Root?.Offset})");
            str.AppendLine($"{tabs}\t\tFiles:");
            foreach (var file in node.Files) {
                str.AppendLine($"{tabs}\t\t\t{file.Id:X8}");
            }
            str.AppendLine($"{tabs}\t\tChildren:");
            if (node.Branches.Count == 0) {
                str.AppendLine($"{tabs}\t\t\tNone!");
            }
            else {
                foreach (var child in node.Branches) {
                    if (TryGetNode(child, out var childNode)) {
                        WriteTree(str, childNode, depth + 2);
                    }
                    else {
                        str.AppendLine($"{tabs}\t\t\t!! Unknown branch: {child:X8}");
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                BlockAllocator.Dispose();
            }
        }
    }
}
