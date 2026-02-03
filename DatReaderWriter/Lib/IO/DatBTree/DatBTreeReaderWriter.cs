using DatReaderWriter.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.Collections;
using DatReaderWriter.Lib.IO.BlockAllocators;
using System.Threading;
using System.Threading.Tasks;

/*
 *  Heavily based on https://github.com/rsdcastro/btree-dotnet/ and https://github.com/msambol/dsa/blob/master/trees/b_tree.py
 */
namespace DatReaderWriter.Lib.IO.DatBTree {
    /// <summary>
    /// A dat BTree reader / writer.
    /// </summary>
    public class DatBTreeReaderWriter : IEnumerable<DatBTreeFile>, IDisposable {
        /// <summary>
        /// Block allocator
        /// </summary>
        public IDatBlockAllocator BlockAllocator { get; }

        /// <summary>
        /// The "degree" of this BTree. Dat files have a degree of 31.
        /// This means at max nodes can have (31 * 2) - 1 file entries,
        /// or at a minimum they can have (31 - 1) file entries.
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
        /// This is <code>(<see cref="Degree"/> * 2) - 1</code>
        /// </summary>
        public int MaxItems => Degree * 2 - 1;

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

#if (NET8_0_OR_GREATER)
        private bool TryGetNode(int blockOffset, [MaybeNullWhen(false)] out DatBTreeNode result) {
#else
        private bool TryGetNode(int blockOffset, out DatBTreeNode result) {
#endif
            var buffer = BaseBlockAllocator.SharedBytes.Rent(DatBTreeNode.SIZE);

            BlockAllocator.ReadBlock(buffer, blockOffset);
            result = new DatBTreeNode(blockOffset);
            var success = result.Unpack(new DatBinReader(buffer));

            BaseBlockAllocator.SharedBytes.Return(buffer);

            return success;
        }

#if (NET8_0_OR_GREATER)
        private async ValueTask<(bool Success, DatBTreeNode? Node)> TryGetNodeAsync(int blockOffset,
            CancellationToken ct = default) {
#else
        private async Task<(bool Success, DatBTreeNode? Node)> TryGetNodeAsync(int blockOffset, CancellationToken ct =
 default) {
#endif
            var buffer = BaseBlockAllocator.SharedBytes.Rent(DatBTreeNode.SIZE);

            await BlockAllocator.ReadBlockAsync(buffer, blockOffset, ct);
            var result = new DatBTreeNode(blockOffset);
            var success = result.Unpack(new DatBinReader(buffer));

            BaseBlockAllocator.SharedBytes.Return(buffer);

            return (success, result);
        }

        /// <summary>
        /// Write a node back to the dat
        /// </summary>
        /// <param name="node">The node to write</param>
        private void WriteNode(DatBTreeNode node) {
            var buffer = BaseBlockAllocator.SharedBytes.Rent(DatBTreeNode.SIZE);
            var writer = new DatBinWriter(buffer);
            node.Pack(writer);
            node.Offset = BlockAllocator.WriteBlock(buffer, DatBTreeNode.SIZE, node.Offset);
            BaseBlockAllocator.SharedBytes.Return(buffer);
        }

        private async ValueTask WriteNodeAsync(DatBTreeNode node, CancellationToken ct = default) {
            var buffer = BaseBlockAllocator.SharedBytes.Rent(DatBTreeNode.SIZE);
            var writer = new DatBinWriter(buffer);
            node.Pack(writer);
            node.Offset = await BlockAllocator.WriteBlockAsync(buffer, DatBTreeNode.SIZE, node.Offset, ct);
            BaseBlockAllocator.SharedBytes.Return(buffer);
        }

#if (NET8_0_OR_GREATER)
        private bool TryGetFileInternal(uint fileId, int startingBlock, [MaybeNullWhen(false)] out DatBTreeFile file) {
#else
        private bool TryGetFileInternal(uint fileId, int startingBlock, out DatBTreeFile file) {
#endif
            // 0 and 0xCDCDCDCD are invalid node offsets
            while (startingBlock != 0 && startingBlock != unchecked((int)0xCDCDCDCD)) {
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

            file = default;
            return false;
        }

#if (NET8_0_OR_GREATER)
        private async ValueTask<(bool Success, DatBTreeFile File)> TryGetFileInternalAsync(uint fileId,
            int startingBlock, CancellationToken ct = default) {
#else
        private async Task<(bool Success, DatBTreeFile File)> TryGetFileInternalAsync(uint fileId, int startingBlock, CancellationToken ct
 = default) {
#endif
            // 0 and 0xCDCDCDCD are invalid node offsets
            while (startingBlock != 0 && startingBlock != unchecked((int)0xCDCDCDCD)) {
                var (success, node) = await TryGetNodeAsync(startingBlock, ct);
                if (success && node is not null) {
                    var left = 0;
                    var right = node.Files.Count - 1;
                    var i = 0;

                    // binary search on keys
                    while (left <= right) {
                        i = (left + right) / 2;
                        var currentFile = node.Files[i];

                        if (fileId == currentFile.Id) {
                            return (true, currentFile);
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

            return (false, default);
        }

        private void SetNewRoot(DatBTreeNode rootBlock) {
            Root = rootBlock;
            WriteNode(rootBlock);
            BlockAllocator.SetRootBlock(rootBlock.Offset);
        }

        private async ValueTask SetNewRootAsync(DatBTreeNode rootBlock, CancellationToken ct = default) {
            Root = rootBlock;
            await WriteNodeAsync(rootBlock, ct);
            // BlockAllocator.SetRootBlock modifies the header, usually synchronous/fast.
            // But if we want to be fully async, BlockAllocator logic might need async header write?
            // Currently BlockAllocator.SetRootBlock is sync.
            // Header is small and usually cached/memory mapped IO is fast.
            // We can keep it sync or add SetRootBlockAsync to Allocator interface?
            // Allocator interface doesn't have Async SetRootBlock.
            // Keeping it sync for now as it's just a header update.
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

            // Insert the middle entry from the full child into the parent
            parent.Files.Insert(childIdx, child.Files[Degree - 1]);
            parent.Branches.Insert(childIdx + 1, newNode.Offset);

            // Transfer second half of files from child to new node
            // Correcting ranges to ensure split aligns with degree and doesn't leave empty branches.
            newNode.Files.AddRange(child.Files.Skip(Degree).Take(Degree - 1));
            child.Files.RemoveRange(Degree - 1, Degree);

            // If not a leaf, also split the branches
            if (!child.IsLeaf) {
                newNode.Branches.AddRange(child.Branches.GetRange(Degree, child.Branches.Count - Degree));
                child.Branches.RemoveRange(Degree, child.Branches.Count - Degree);
            }

            // Write nodes back to ensure the changes are persisted in the tree structure
            WriteNode(newNode);
            WriteNode(child);
            WriteNode(parent);
        }

        private async ValueTask SplitChildAsync(DatBTreeNode parent, DatBTreeNode child,
            CancellationToken ct = default) {
            var newNode = new DatBTreeNode(BlockAllocator.ReserveBlock());
            // ReserveBlock is sync, but we fixed it to be thread safe.

            var childIdx = parent.Branches.FindIndex(f => f == child.Offset);

            // Insert the middle entry from the full child into the parent
            parent.Files.Insert(childIdx, child.Files[Degree - 1]);
            parent.Branches.Insert(childIdx + 1, newNode.Offset);

            // Transfer second half of files from child to new node
            newNode.Files.AddRange(child.Files.Skip(Degree).Take(Degree - 1));
            child.Files.RemoveRange(Degree - 1, Degree);

            // If not a leaf, also split the branches
            if (!child.IsLeaf) {
                newNode.Branches.AddRange(child.Branches.GetRange(Degree, child.Branches.Count - Degree));
                child.Branches.RemoveRange(Degree, child.Branches.Count - Degree);
            }

            // Write nodes back
            await WriteNodeAsync(newNode, ct);
            await WriteNodeAsync(child, ct);
            await WriteNodeAsync(parent, ct);
        }

        private IEnumerable<DatBTreeFile> GetFilesRecursive(DatBTreeNode? node) {
            if (node is not null) {
                int i;
                for (i = 0; i < node.Files.Count; i++) {
                    if (!node.IsLeaf) {
                        if (TryGetNode(node.Branches[i], out var branch)) {
                            var files = GetFilesRecursive(branch);
                            foreach (var file in files) {
                                yield return file;
                            }
                        }
                    }

                    yield return node.Files[i];
                }

                if (!node.IsLeaf) {
                    if (TryGetNode(node.Branches[i], out var branch)) {
                        var files2 = GetFilesRecursive(branch);
                        foreach (var file in files2) {
                            yield return file;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get an enumerator that yields all <see cref="DatBTreeFile"/> entries in
        /// this tree recursively in order
        /// </summary>
        /// <returns>An enumerator</returns>
        public IEnumerator<DatBTreeFile> GetEnumerator() {
            if (Root is not null) {
                foreach (var fileEntry in GetFilesRecursive(Root)) {
                    yield return fileEntry;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets all file entries within the specified ID range (inclusive).
        /// </summary>
        /// <param name="minId">The minimum file ID (inclusive)</param>
        /// <param name="maxId">The maximum file ID (inclusive)</param>
        /// <returns>An enumerable of files within the range</returns>
        public IEnumerable<DatBTreeFile> GetFilesInRange(uint minId, uint maxId) {
            if (Root is null) {
                yield break;
            }

            foreach (var file in GetFilesInRangeRecursive(Root, minId, maxId)) {
                yield return file;
            }
        }

        private IEnumerable<DatBTreeFile> GetFilesInRangeRecursive(DatBTreeNode node, uint minId, uint maxId) {
            int i = 0;

            // Skip entries less than minId
            while (i < node.Files.Count && node.Files[i].Id < minId) {
                i++;
            }

            if (!node.IsLeaf) {
                // Check the leftmost qualifying subtree
                if (i < node.Branches.Count && TryGetNode(node.Branches[i], out var child)) {
                    foreach (var file in GetFilesInRangeRecursive(child, minId, maxId)) {
                        yield return file;
                    }
                }
            }

            // Return entries from this node that are in range
            while (i < node.Files.Count && node.Files[i].Id <= maxId) {
                yield return node.Files[i];

                if (!node.IsLeaf) {
                    // Check the right subtree for this entry
                    if (i + 1 < node.Branches.Count && TryGetNode(node.Branches[i + 1], out var child)) {
                        foreach (var file in GetFilesInRangeRecursive(child, minId, maxId)) {
                            yield return file;
                        }
                    }
                }

                i++;
            }
        }

        /// <summary>
        /// Try and get a file in the tree with the specified id
        /// </summary>
        /// <param name="fileId">The file id to search for</param>
        /// <param name="file">The file, or null if not found</param>
        /// <returns>true if the file was found, false otherwise</returns>
#if (NET8_0_OR_GREATER)
        public bool TryGetFile(uint fileId, [MaybeNullWhen(false)] out DatBTreeFile file) {
#else
        public bool TryGetFile(uint fileId, out DatBTreeFile file) {
#endif
            return TryGetFileInternal(fileId, BlockAllocator.Header.RootBlock, out file);
        }

        /// <summary>
        /// Try and get a file in the tree with the specified id asynchronously
        /// </summary>
        /// <param name="fileId">The file id to search for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>A ValueTask containing success status and the file if found</returns>
#if (NET8_0_OR_GREATER)
        public async ValueTask<(bool Success, DatBTreeFile File)> TryGetFileAsync(uint fileId,
            CancellationToken ct = default) {
#else
        public async Task<(bool Success, DatBTreeFile File)> TryGetFileAsync(uint fileId, CancellationToken ct =
 default) {
#endif
            return await TryGetFileInternalAsync(fileId, BlockAllocator.Header.RootBlock, ct);
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
                // Find the parent node and update the file in place
                if (TryFindParentAndUpdate(Root, file, out var replacedFile)) {
                    return replacedFile;
                }

                throw new Exception($"Could not find existing file in tree structure");
            }

            // if root is null, create a new root node and add the file there
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

                newRoot.Branches.Add(oldRoot.Offset);
                SplitChild(newRoot, oldRoot);
            }

            InsertNonFull(Root, file);
            return null;
        }

        /// <summary>
        /// Inserts a <see cref="DatBTreeFile"/> into this tree asynchronously, replacing a
        /// a file if one is already there.
        /// </summary>
        /// <param name="file">The file to add</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The file that was replaced, if any</returns>
#if (NET8_0_OR_GREATER)
        public async ValueTask<DatBTreeFile?> InsertAsync(DatBTreeFile file, CancellationToken ct = default) {
#else
        public async Task<DatBTreeFile?> InsertAsync(DatBTreeFile file, CancellationToken ct = default) {
#endif
            // check if file already exists
            var (found, _) = await TryGetFileAsync(file.Id, ct);
            if (found) {
                // Find the parent node and update the file in place
                var (success, replacedFile) = await TryFindParentAndUpdateAsync(Root, file, ct);
                if (success) {
                    return replacedFile;
                }

                throw new Exception($"Could not find existing file in tree structure");
            }

            // if root is null, create a new root node and add the file there
            if (Root is null) {
                var rootBlock = new DatBTreeNode(BlockAllocator.ReserveBlock());
                rootBlock.Files.Add(file);
                await SetNewRootAsync(rootBlock, ct);
                return null;
            }

            // check if root node is full, if so create a new root node before insertion
            if (Root.Files.Count == MaxItems) {
                var oldRoot = Root;
                var newRoot = new DatBTreeNode(BlockAllocator.ReserveBlock());
                await SetNewRootAsync(newRoot, ct);

                newRoot.Branches.Add(oldRoot.Offset);
                await SplitChildAsync(newRoot, oldRoot, ct);
            }

            await InsertNonFullAsync(Root, file, ct);
            return null;
        }

        private bool TryFindParentAndUpdate(DatBTreeNode? node, DatBTreeFile file, out DatBTreeFile replacedFile) {
            if (node is null) {
                replacedFile = default;
                return false;
            }

            var idx = node.Files.FindIndex(f => f.Id == file.Id);
            if (idx >= 0) {
                replacedFile = node.Files[idx];
                node.Files[idx] = file;
                WriteNode(node);
                return true;
            }

            // Search children
            if (!node.IsLeaf) {
                foreach (var branchOffset in node.Branches) {
                    if (TryGetNode(branchOffset, out var child)) {
                        if (TryFindParentAndUpdate(child, file, out replacedFile)) {
                            return true;
                        }
                    }
                }
            }

            replacedFile = default;
            return false;
        }

#if (NET8_0_OR_GREATER)
        private async ValueTask<(bool Success, DatBTreeFile ReplacedFile)> TryFindParentAndUpdateAsync(
            DatBTreeNode? node, DatBTreeFile file, CancellationToken ct = default) {
#else
        private async Task<(bool Success, DatBTreeFile ReplacedFile)> TryFindParentAndUpdateAsync(DatBTreeNode? node, DatBTreeFile file, CancellationToken ct
 = default) {
#endif
            if (node is null) {
                return (false, default);
            }

            var idx = node.Files.FindIndex(f => f.Id == file.Id);
            if (idx >= 0) {
                var replaced = node.Files[idx];
                node.Files[idx] = file;
                await WriteNodeAsync(node, ct);
                return (true, replaced);
            }

            // Search children
            if (!node.IsLeaf) {
                foreach (var branchOffset in node.Branches) {
                    var (success, child) = await TryGetNodeAsync(branchOffset, ct);
                    if (success && child is not null) {
                        var (found, replaced) = await TryFindParentAndUpdateAsync(child, file, ct);
                        if (found) {
                            return (true, replaced);
                        }
                    }
                }
            }

            return (false, default);
        }

        private void InsertNonFull(DatBTreeNode node, DatBTreeFile file) {
            int positionToInsert = node.Files.TakeWhile(entry => file.Id.CompareTo(entry.Id) >= 0).Count();

            if (node.IsLeaf) {
                // Directly insert into the leaf node at the calculated position.
                node.Files.Insert(positionToInsert, file);
                WriteNode(node);
                return;
            }

            // Ensure the child at positionToInsert is loaded
            if (!TryGetNode(node.Branches[positionToInsert], out var child)) {
                throw new Exception("Could not look up child node during insertion!");
            }

            // Check if the child node is full
            if (child.Files.Count == MaxItems) {
                // Split the child node
                SplitChild(node, child);

                // After splitting, if the file's ID is greater than the middle key
                // (which was moved up to the parent), adjust positionToInsert to the right child.
                if (file.Id.CompareTo(node.Files[positionToInsert].Id) > 0) {
                    positionToInsert++;
                    // Reload child if it points to the newly created right child
                    if (!TryGetNode(node.Branches[positionToInsert], out child)) {
                        throw new Exception("Could not look up child node after split!");
                    }
                }
            }

            // Recursively insert into the appropriate child
            InsertNonFull(child, file);
        }

        private async ValueTask
            InsertNonFullAsync(DatBTreeNode node, DatBTreeFile file, CancellationToken ct = default) {
            int positionToInsert = node.Files.TakeWhile(entry => file.Id.CompareTo(entry.Id) >= 0).Count();

            if (node.IsLeaf) {
                // Directly insert into the leaf node at the calculated position.
                node.Files.Insert(positionToInsert, file);
                await WriteNodeAsync(node, ct);
                return;
            }

            // Ensure the child at positionToInsert is loaded
            var (success, child) = await TryGetNodeAsync(node.Branches[positionToInsert], ct);
            if (!success || child is null) {
                throw new Exception("Could not look up child node during insertion!");
            }

            // Check if the child node is full
            if (child.Files.Count == MaxItems) {
                // Split the child node
                await SplitChildAsync(node, child, ct);

                // After splitting, if the file's ID is greater than the middle key
                // (which was moved up to the parent), adjust positionToInsert to the right child.
                if (file.Id.CompareTo(node.Files[positionToInsert].Id) > 0) {
                    positionToInsert++;
                    // Reload child if it points to the newly created right child
                    (success, child) = await TryGetNodeAsync(node.Branches[positionToInsert], ct);
                    if (!success || child is null) {
                        throw new Exception("Could not look up child node after split!");
                    }
                }
            }

            // Recursively insert into the appropriate child
            await InsertNonFullAsync(child, file, ct);
        }

        /// <summary>
        /// Delete a file entry from the tree
        /// </summary>
        /// <param name="fileId">Key to be deleted.</param>
        /// <param name="fileEntry">The file entry that was deleted</param>
        /// <returns>True if file was deleted, false otherwise (not found)</returns>
#if (NET8_0_OR_GREATER)
        public bool TryDelete(uint fileId, [MaybeNullWhen(false)] out DatBTreeFile fileEntry) {
#else
        public bool TryDelete(uint fileId, out DatBTreeFile fileEntry) {
#endif
            if (Root is null) {
                fileEntry = default;
                return false;
            }

            if (!TryGetFile(fileId, out fileEntry)) {
                return false;
            }

            DeleteInternal(Root, fileId);

            // if root's last entry was moved to a child node, remove it
            if (Root.Files.Count == 0 && !Root.IsLeaf && TryGetNode(Root.Branches[0], out var branch)) {
                SetNewRoot(branch);
            }

            return true;
        }

        /// <summary>
        /// Internal method to delete keys from the BTree
        /// </summary>
        /// <param name="node">Node to use to start search for the key.</param>
        /// <param name="fileId">The id of the file to delete</param>
        private void DeleteInternal(DatBTreeNode node, uint fileId) {
            int i = node.Files.TakeWhile(entry => fileId.CompareTo(entry.Id) > 0).Count();

            // found key in node, so delete if from it
            if (i < node.Files.Count && node.Files[i].Id.CompareTo(fileId) == 0) {
                DeleteKeyFromNode(node, fileId, i);
            }

            // delete key from subtree
            else if (!node.IsLeaf) {
                DeleteKeyFromSubtree(node, fileId, i);
            }
        }

        /// <summary>
        /// Helper method that deletes a key from a subtree.
        /// </summary>
        /// <param name="parentNode">Parent node used to start search for the key.</param>
        /// <param name="keyToDelete">Key to be deleted.</param>
        /// <param name="subtreeIndexInNode">Index of subtree node in the parent node.</param>
        private void DeleteKeyFromSubtree(DatBTreeNode parentNode, uint keyToDelete, int subtreeIndexInNode) {
            if (!TryGetNode(parentNode.Branches[subtreeIndexInNode], out var childNode)) {
                Debug.Assert(false, "Unable to lookup child node!");
                return;
            }

            // node has reached min # of entries, and removing any from it will break the btree property,
            // so this block makes sure that the "child" has at least "degree" # of nodes by moving an 
            // entry from a sibling node or merging nodes
            if (childNode.Files.Count == MinItems) {
                int leftIndex = subtreeIndexInNode - 1;
                int rightIndex = subtreeIndexInNode + 1;
                DatBTreeNode? leftSibling = null;
                DatBTreeNode? rightSibling = null;

                if (subtreeIndexInNode > 0) {
                    if (!TryGetNode(parentNode.Branches[leftIndex], out leftSibling)) {
                        Debug.Assert(false, "Unable to lookup leftSibling node!");
                        return;
                    }
                }

                if (subtreeIndexInNode < parentNode.Branches.Count - 1) {
                    if (!TryGetNode(parentNode.Branches[rightIndex], out rightSibling)) {
                        Debug.Assert(false, "Unable to lookup rightSibling node!");
                        return;
                    }
                }

                if (leftSibling is not null && leftSibling.Files.Count > Degree - 1) {
                    // left sibling has a node to spare, so this moves one node from left sibling 
                    // into parent's node and one node from parent into this current node ("child")
                    childNode.Files.Insert(0, parentNode.Files[subtreeIndexInNode]);
                    parentNode.Files[subtreeIndexInNode] = leftSibling.Files.Last();
                    leftSibling.Files.RemoveAt(leftSibling.Files.Count - 1);

                    if (!leftSibling.IsLeaf) {
                        childNode.Branches.Insert(0, leftSibling.Branches.Last());
                        leftSibling.Branches.RemoveAt(leftSibling.Branches.Count - 1);
                    }

                    WriteNode(childNode);
                    WriteNode(parentNode);
                    WriteNode(leftSibling);
                }
                else if (rightSibling != null && rightSibling.Files.Count > Degree - 1) {
                    // right sibling has a node to spare, so this moves one node from right sibling 
                    // into parent's node and one node from parent into this current node ("child")
                    childNode.Files.Add(parentNode.Files[subtreeIndexInNode]);
                    parentNode.Files[subtreeIndexInNode] = rightSibling.Files.First();
                    rightSibling.Files.RemoveAt(0);

                    if (!rightSibling.IsLeaf) {
                        childNode.Branches.Add(rightSibling.Branches.First());
                        rightSibling.Branches.RemoveAt(0);
                    }

                    WriteNode(childNode);
                    WriteNode(parentNode);
                    WriteNode(rightSibling);
                }
                else {
                    // this block merges either left or right sibling into the current node "child"
                    if (leftSibling != null) {
                        childNode.Files.Insert(0, parentNode.Files[subtreeIndexInNode]);
                        var oldEntries = childNode.Files;
                        childNode.Files = leftSibling.Files;
                        childNode.Files.AddRange(oldEntries);
                        if (!leftSibling.IsLeaf) {
                            var oldChildren = childNode.Branches;
                            childNode.Branches = leftSibling.Branches;
                            childNode.Branches.AddRange(oldChildren);
                        }

                        parentNode.Branches.RemoveAt(leftIndex);
                        parentNode.Files.RemoveAt(subtreeIndexInNode);

                        WriteNode(childNode);
                        WriteNode(parentNode);
                    }
                    else {
                        Debug.Assert(rightSibling != null, "Node should have at least one sibling");
                        childNode.Files.Add(parentNode.Files[subtreeIndexInNode]);
                        childNode.Files.AddRange(rightSibling!.Files);
                        if (!rightSibling.IsLeaf) {
                            childNode.Branches.AddRange(rightSibling.Branches);
                        }

                        parentNode.Branches.RemoveAt(rightIndex);
                        parentNode.Files.RemoveAt(subtreeIndexInNode);

                        WriteNode(childNode);
                        WriteNode(parentNode);
                    }
                }
            }

            // at this point, we know that "child" has at least "degree" nodes, so we can
            // move on - this guarantees that if any node needs to be removed from it to
            // guarantee BTree's property, we will be fine with that
            DeleteInternal(childNode, keyToDelete);
        }

        /// <summary>
        /// Helper method that deletes key from a node that contains it, be this
        /// node a leaf node or an internal node.
        /// </summary>
        /// <param name="node">Node that contains the key.</param>
        /// <param name="keyToDelete">Key to be deleted.</param>
        /// <param name="keyIndexInNode">Index of key within the node.</param>
        private void DeleteKeyFromNode(DatBTreeNode node, uint keyToDelete, int keyIndexInNode) {
            // if leaf, just remove it from the list of entries (we're guaranteed to have
            // at least "degree" # of entries, to BTree property is maintained
            if (node.IsLeaf) {
                node.Files.RemoveAt(keyIndexInNode);
                WriteNode(node);
                return;
            }


            if (!TryGetNode(node.Branches[keyIndexInNode], out var predecessorChild)) {
                Debug.Assert(false, "Unable to lookup predecessorChild node!");
                return;
            }

            if (predecessorChild.Files.Count >= Degree) {
                var predecessor = DeletePredecessor(predecessorChild);
                node.Files[keyIndexInNode] = predecessor;
                WriteNode(node);
            }
            else {
                if (!TryGetNode(node.Branches[keyIndexInNode + 1], out var successorChild)) {
                    Debug.Assert(false, "Unable to lookup successorChild node!");
                    return;
                }

                if (successorChild.Files.Count >= Degree) {
                    var successor = DeleteSuccessor(predecessorChild);
                    node.Files[keyIndexInNode] = successor;
                    WriteNode(node);
                }
                else {
                    predecessorChild.Files.Add(node.Files[keyIndexInNode]);
                    predecessorChild.Files.AddRange(successorChild.Files);
                    predecessorChild.Branches.AddRange(successorChild.Branches);

                    node.Files.RemoveAt(keyIndexInNode);
                    node.Branches.RemoveAt(keyIndexInNode + 1);

                    WriteNode(node);
                    WriteNode(predecessorChild);

                    DeleteInternal(predecessorChild, keyToDelete);
                }
            }
        }

        /// <summary>
        /// Helper method that deletes a predecessor key (i.e. rightmost key) for a given node.
        /// </summary>
        /// <param name="node">Node for which the predecessor will be deleted.</param>
        /// <returns>Predecessor entry that got deleted.</returns>
        private DatBTreeFile DeletePredecessor(DatBTreeNode node) {
            if (node.IsLeaf) {
                var result = node.Files[node.Files.Count - 1];
                node.Files.RemoveAt(node.Files.Count - 1);
                WriteNode(node);

                return result;
            }


            if (!TryGetNode(node.Branches.Last(), out var predecessor)) {
                throw new Exception($"Failed to look up predecessor");
            }

            return DeletePredecessor(predecessor);
        }

        /// <summary>
        /// Helper method that deletes a successor key (i.e. leftmost key) for a given node.
        /// </summary>
        /// <param name="node">Node for which the successor will be deleted.</param>
        /// <returns>Successor entry that got deleted.</returns>
        private DatBTreeFile DeleteSuccessor(DatBTreeNode node) {
            if (node.IsLeaf) {
                var result = node.Files[0];
                node.Files.RemoveAt(0);
                WriteNode(node);
                return result;
            }

            if (!TryGetNode(node.Branches.First(), out var predecessor)) {
                throw new Exception($"Failed to look up predecessor");
            }

            return DeletePredecessor(predecessor);
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
            str.AppendLine(
                $"{tabs}\tNode: {node.Offset:X8} (Leaf: {node.IsLeaf}, Root: {node.Offset == Root?.Offset})");
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
            }
        }
    }
}
