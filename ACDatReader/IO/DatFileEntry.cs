using System.Runtime.InteropServices;
using System.Text;

namespace ACDatReader.IO {
    /// <summary>
    /// A dat file entry. This points to where dat files are stored in the dat,
    /// as well as some other meta data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DatFileEntry {
        /// <summary>
        /// The size of this struct
        /// </summary>
        public static readonly int SIZE = 24;

        /// <summary>
        /// Some kind of flags?
        /// </summary>
        public uint Flags;

        /// <summary>
        /// The id of the file this entry points to. These are dat file ids like
        /// 0x05000001 for textures as an example.
        /// </summary>
        public uint Id;

        /// <summary>
        /// The offset in the dat file of the first block containing this files data
        /// </summary>
        public uint Offset;

        /// <summary>
        /// The total size of this file data. If greater than DatHeader.BlockSize - 4 then
        /// this spans `ceil(Size / (DatHeader.BlockSize - 4))` blocks.
        /// </summary>
        public uint Size;

        /// <summary>
        /// The dat this was last updated (maybe added?) in unix timestamp format
        /// </summary>
        public uint Date;

        /// <summary>
        /// The iteration of this file entry
        /// </summary>
        public uint Iteration;

        /// <summary>
        /// debug string output
        /// </summary>
        public override string ToString() {
            var str = new StringBuilder();

            str.AppendLine($"DatFileEntry:");
            str.AppendLine($"\t Id: {Id:X8}");
            str.AppendLine($"\t Flags: {Flags:X8}");
            str.AppendLine($"\t Offset: {Offset:X8}");
            str.AppendLine($"\t Size: {Size:N0}");
            str.AppendLine($"\t Date: {Date:X8}");
            str.AppendLine($"\t Iteration: {Iteration:X8}");

            return str.ToString();
        }
    }
}