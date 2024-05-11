using System;
using System.Buffers.Binary;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;

namespace ACDatReader.IO {
    /// <summary>
    /// A dat directory entry.
    /// </summary>
    public struct DatDirectoryEntry {
        /// <summary>
        /// The maximum size this struct will be
        /// </summary>
        public static readonly int SIZE = 1720;

        /// <summary>
        /// The offset in the dat
        /// </summary>
        public int Offset;

        /// <summary>
        /// Directory branches, if any
        /// </summary>
        public int[]? Branches;

        /// <summary>
        /// Number of File entries directly in this directory
        /// </summary>
        public int EntryCount;

        /// <summary>
        /// File entries directory in this directory, if any
        /// </summary>
        public DatFileEntry[]? Entries;

        /// <summary>
        /// Wether this is a leaf node. Leaf nodes have no branches.
        /// </summary>
        public readonly bool IsLeaf => Branches is null || Branches[0] == 0;

        /// <summary>
        /// Unpack a buffer into this DatDirectoryEntry
        /// </summary>
        /// <param name="buffer">The buffer to unpack from</param>
        /// <returns>True if successfull</returns>
        unsafe public bool Unpack(ReadOnlySpan<byte> buffer) {
            Span<int> branches = stackalloc int[62];
            Span<int> iSpan = stackalloc int[1];
            for (iSpan[0] = 0; iSpan[0] < 62; iSpan[0]++) {
                branches[iSpan[0]] = BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(iSpan[0] * 4, 4));
            }

            EntryCount = BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(248, 4));

            if (branches[0] != 0) {
                Branches = new int[EntryCount + 1];
                branches.Slice(0, EntryCount + 1).CopyTo(Branches);
            }

            Entries = new DatFileEntry[EntryCount];

            Span<nint> entryPtr = [Marshal.UnsafeAddrOfPinnedArrayElement(Entries, 0)];

            fixed (byte* filesPtr = &buffer[(252)]) {
                var size = DatFileEntry.SIZE * EntryCount;
                Buffer.MemoryCopy(filesPtr, (void*)entryPtr[0], size, size);
            }

            return true;
        }

        /// <summary>
        /// Debug string output
        /// </summary>
        public override string ToString() {
            var str = new StringBuilder();

            str.AppendLine($"DatDirectoryEntry:");
            str.AppendLine($"\t Offset: {Offset:X8}");
            str.AppendLine($"\t Branches: [{(Branches is null ? "" : string.Join(" ", Branches.Select(b => b.ToString("X8"))))}]");
            str.AppendLine($"\t EntryCount: {EntryCount}");
            str.AppendLine($"\t Entries: [{(Entries is null ? "" : string.Join(" ", Entries.Select(b => b.Id.ToString("X8"))))}]");
            str.AppendLine($"\t IsLeaf: {IsLeaf}");

            return str.ToString();
        }
    }
}