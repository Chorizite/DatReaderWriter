using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ACDatReader.IO {

    /// <summary>
    /// The header of a dat file. This is contained within the first <see cref="DatHeader.SIZE"/>
    /// (400) bytes of the header
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DatHeader {
        /// <summary>
        /// The size of this struct
        /// </summary>
        public static readonly int SIZE = 400;

        /// <summary>
        /// Version string. Seems to be empty in all the dats.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] Transactions;

        /// <summary>
        /// Magic, obviously
        /// </summary>
        public int Magic;

        /// <summary>
        /// The size of the blocks in this dat file.
        /// </summary>
        public int BlockSize;

        /// <summary>
        /// The total size of this dat, in bytes
        /// </summary>
        public int FileSize;

        /// <summary>
        /// The type of database this is. In the case of HighRes, it is a portal type
        /// and you will need to check its <see cref="SubSet"/>. for portal the <see cref="SubSet"/>
        /// is 0
        /// </summary>
        public DatDatabaseType Type;

        /// <summary>
        /// Specific subset of the database <see cref="Type"/>. For <see cref="DatDatabaseType.Cell"/>
        /// databases this is the region id
        /// </summary>
        public uint SubSet;

        /// <summary>
        /// The offset of the first free block in this file
        /// </summary>
        public int FirstFreeBlock;

        /// <summary>
        /// The offset of the last free block in this file
        /// </summary>
        public int LastFreeBlock;

        /// <summary>
        /// The number of free blocks in this file
        /// </summary>
        public int FreeBlockCount;

        /// <summary>
        /// The root block. This is where traversal starts.
        /// </summary>
        public int RootBlock;

        public int NewLRU;
        public int OldLRU;
        public bool UseLRU;

        /// <summary>
        /// The file id of the MasterMap
        /// </summary>
        public int MasterMapId;

        /// <summary>
        /// Engine Version
        /// </summary>
        public int EngineVersion;

        /// <summary>
        /// Game Version
        /// </summary>
        public int GameVersion;

        /// <summary>
        /// Major Version
        /// </summary>
        public Guid MajorVersion;

        /// <summary>
        /// Minor Version
        /// </summary>
        public uint MinorVersion;

        /// <summary>
        /// debug output string
        /// </summary>
        public override string ToString() {
            var str = new StringBuilder();

            str.AppendLine($"Database Header:");
            str.AppendLine($"\t Version: {Version}");
            str.AppendLine($"\t Transactions: [{(Transactions is null ? "" : string.Join(" ", Transactions.Select(b => b.ToString("X2"))))}]");
            str.AppendLine($"\t Magic: {Magic:X8}");
            str.AppendLine($"\t BlockSize: {BlockSize}");
            str.AppendLine($"\t FileSize: {FileSize}");
            str.AppendLine($"\t Type: {Type}");
            str.AppendLine($"\t SubSet: {SubSet}");
            str.AppendLine($"\t FirstFreeBlock: {FirstFreeBlock:X8}");
            str.AppendLine($"\t LastFreeBlock: {LastFreeBlock:X8}");
            str.AppendLine($"\t FreeBlockCount: {FreeBlockCount}");
            str.AppendLine($"\t RootBlock: {RootBlock:X8}");
            str.AppendLine($"\t NewLRU: {NewLRU}");
            str.AppendLine($"\t OldLRU: {OldLRU}");
            str.AppendLine($"\t UseLRU: {UseLRU}");
            str.AppendLine($"\t MasterMapId: {MasterMapId:X8}");
            str.AppendLine($"\t EngineVersion: {EngineVersion}");
            str.AppendLine($"\t GameVersion: {GameVersion}");
            str.AppendLine($"\t MajorVersion: {MajorVersion}");
            str.AppendLine($"\t MinorVersion: {MinorVersion}");

            return str.ToString();
        }
    }
}
