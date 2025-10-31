﻿using DatReaderWriter.Lib.Extensions;
using DatReaderWriter.Lib.IO;
using System;
using System.Text;

namespace DatReaderWriter.Lib.IO.DatBTree {
    /// <summary>
    /// A dat file entry. This points to where dat files are stored in the dat,
    /// as well as some other meta data.
    /// </summary>
    public struct DatBTreeFile : IPackable, IUnpackable {
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
        /// The date this was last updated (maybe added?)
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The iteration of this file entry
        /// </summary>
        public int Iteration { get; set; }

        /// <inheritdoc/>
        public int GetSize() => SIZE;

        public DatBTreeFile() {
            Flags = 0x20000;
        }

        /// <inheritdoc/>
        public bool Unpack(DatBinReader reader) {
            Flags = reader.ReadUInt32();
            Id = reader.ReadUInt32();
            Offset = reader.ReadInt32();
            Size = reader.ReadUInt32();
            Date = DateTimeOffset.FromUnixTimeSeconds(reader.ReadUInt32()).UtcDateTime;
            Iteration = reader.ReadInt32();

            return true;
        }

        /// <inheritdoc/>
        public bool Pack(DatBinWriter writer) {
            if (Flags != 0) {
                writer.WriteUInt32(Flags);
            }
            else {
                writer.WriteUInt32(0x20000);
            }
            writer.WriteUInt32(Id);
            writer.WriteInt32(Offset);
            writer.WriteUInt32(Size);
            writer.WriteUInt32(Date.ToUnixTimestamp());
            writer.WriteInt32(Iteration);

            return true;
        }

        /// <inheritdoc/>
        public override string ToString() {
            var str = new StringBuilder();
            str.AppendLine($"DatFileEntry:");
            str.AppendLine($"\t Id: {Id:X8}");
            str.AppendLine($"\t Flags: {Flags:X8}");
            str.AppendLine($"\t Offset: {Offset:X8}");
            str.AppendLine($"\t Size: {Size:N0}");
            str.AppendLine($"\t Date: {Date}");
            str.AppendLine($"\t Iteration: {Iteration}");

            return str.ToString();
        }
    }
}