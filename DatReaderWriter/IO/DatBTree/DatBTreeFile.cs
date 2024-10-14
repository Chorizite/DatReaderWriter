using System;
using System.Text;

namespace ACClientLib.DatReaderWriter.IO.DatBTree {
    /// <summary>
    /// A dat file entry. This points to where dat files are stored in the dat,
    /// as well as some other meta data.
    /// </summary>
    public class DatBTreeFile : IPackable, IUnpackable {
        /// <summary>
        /// The size of this struct
        /// </summary>
        public static readonly int SIZE = 24;

        /// <summary>
        /// Some kind of flags?
        /// </summary>
        public uint Flags { get; set; }

        /// <summary>
        /// The id of the file this entry points to. These are dat file ids like
        /// 0x05000001 for textures as an example.
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// The offset in the dat file of the first block containing this files data
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// The total size of this file data. If greater than DatHeader.BlockSize - 4 then
        /// this spans <c>ceil(Size / (DatHeader.BlockSize - 4))</c> blocks.
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// The date this was last updated (maybe added?) in unix timestamp format
        /// </summary>
        public int Date { get; set; }

        /// <summary>
        /// The iteration of this file entry
        /// </summary>
        public int Iteration { get; set; }

        /// <summary>
        /// The parent node of this file entry
        /// </summary>
        public DatBTreeNode? Parent { get; internal set; }

        /// <inheritdoc/>
        public int GetSize() => SIZE;

        /// <inheritdoc/>
        public bool Unpack(DatFileReader reader) {
            Flags = reader.ReadUInt32();
            Id = reader.ReadUInt32();
            Offset = reader.ReadInt32();
            Size = reader.ReadUInt32();
            Date = reader.ReadInt32();
            Iteration = reader.ReadInt32();

            return true;
        }

        /// <inheritdoc/>
        public bool Pack(DatFileWriter writer) {
            writer.WriteUInt32(Flags);
            writer.WriteUInt32(Id);
            writer.WriteInt32(Offset);
            writer.WriteUInt32(Size);
            writer.WriteInt32(Date);
            writer.WriteInt32(Iteration);

            return true;
        }

        /// <inheritdoc/>
        public override string ToString() {
            var str = new StringBuilder();
            
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(Date).ToLocalTime();

            str.AppendLine($"DatFileEntry:");
            str.AppendLine($"\t Id: {Id:X8}");
            str.AppendLine($"\t Flags: {Flags:X8}");
            str.AppendLine($"\t Offset: {Offset:X8}");
            str.AppendLine($"\t Size: {Size:N0}");
            str.AppendLine($"\t Date: {dateTime}");
            str.AppendLine($"\t Iteration: {Iteration}");

            return str.ToString();
        }
    }
}