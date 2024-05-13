using ACDatReader.IO.BlockAllocators;
using ACDatReader.Options;
using System;
using System.Reflection.PortableExecutable;

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
        public int Degree { get; } = 30;

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
        public int MaxItems => (Degree * 2) + 1;

        /// <summary>
        /// Create a new DatBTreeReaderWriter instance from a <see cref="IDatBlockAllocator"/>
        /// </summary>
        /// <param name="blockAllocator">The block allocator to use</param>
        public DatBTreeReaderWriter(IDatBlockAllocator blockAllocator) {
            BlockAllocator = blockAllocator;
            if (TryGetNode(BlockAllocator.Header.RootBlock, out var root)) {
                Root = root;
            }
        }

        private bool TryGetNode(int blockOffset, out DatBTreeNode result) {
            var buffer = BaseBlockAllocator.SharedBytes.Rent(DatBTreeNode.SIZE);

            BlockAllocator.ReadBlock(buffer, blockOffset);
            result = new DatBTreeNode();
            var success = result.Unpack(new DatFileReader(buffer));

            BaseBlockAllocator.SharedBytes.Return(buffer);

            return success;
        }


        /*
        /// <summary>
        /// Try to get a file entry
        /// </summary>
        /// <param name="fileId">The id of the file</param>
        /// <param name="fileEntry">The file entry, if it exists</param>
        /// <returns>True if a file entry with the specific <paramref name="fileId"/> was found, false otherwise</returns>
        public bool TryGetFileEntry(uint fileId, out DatFileEntry fileEntry) {
            if (Options.IndexCachingStrategy >= IndexCachingStrategy.OnDemand && _fileEntryCache.TryGetValue(fileId, out fileEntry)) {
                return true;
            }

            var buffer = sharedBytes.Rent(DatDirectoryEntry.SIZE);

            Span<int> node = [Header.RootBlock];
            Span<int> track = [0, 0, 0];

            // @paradox logic
            while (node[0] != 0 && node[0] != unchecked((int)0xcdcdcdcd)) {
                DatDirectoryEntry de = GetDirectoryEntry(node[0], ref buffer);

                track[0] = 0; // left
                track[1] = de.EntryCount - 1; // right
                track[2] = 0; // i

                while (track[0] <= track[1]) {
                    track[2] = (track[0] + track[1]) / 2;
                    fileEntry = de.Entries![track[2]];

                    if (fileId == fileEntry.Id) {
                        sharedBytes.Return(buffer);
                        return true;
                    }
                    else if (fileId < fileEntry.Id)
                        track[1] = track[2] - 1;
                    else
                        track[0] = track[2] + 1;
                }

                if (de.IsLeaf)
                    break;

                if (fileId > de.Entries![track[2]].Id)
                    track[2]++;

                node[0] = de.Branches![track[2]];
            }

            sharedBytes.Return(buffer);
            fileEntry = new();
            return false;
        }
        */

        /// <summary>
        /// Inserts a <see cref="DatBTreeFile"/> into this tree, replacing a
        /// a file if one is already there.
        /// </summary>
        /// <param name="file">The file to add</param>
        /// <returns>The file that was replaced, if any</returns>
        public DatBTreeFile? Insert(DatBTreeFile file) {
            throw new NotImplementedException();
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
