using DatReaderWriter.Enums;
using System;
using System.Linq;
using System.Text;

namespace DatReaderWriter.Lib.IO {

    /// <summary>
    /// The header of a dat file
    /// </summary>
    public class DatHeader : IUnpackable, IPackable {
        /// <summary>
        /// The size of this struct
        /// </summary>
        public static readonly int SIZE = 400;

        /// <summary>
        /// The magic used in retail dats
        /// </summary>
        public static readonly int RETAIL_MAGIC = 0x00005442;

        /// <summary>
        /// Version string. Seems to be empty in all the dats. Can be a maximum of 255 characters long.
        /// </summary>
        public string? Version;

        /// <summary>
        /// Last transaction info
        /// </summary>
        public byte[] Transactions;

        /// <summary>
        /// Magic, obviously. This is always 0x00005442.
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
        public DatFileType Type;

        /// <summary>
        /// Specific subset of the database <see cref="Type"/>. For <see cref="DatFileType.Cell"/>
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

        /// <summary>
        /// Something to do with LastRecentlyUsed cache.
        /// Currently unsupported.
        /// </summary>
        public int NewLRU;

        /// <summary>
        /// Something to do with LastRecentlyUsed cache.
        /// Currently unsupported.
        /// </summary>
        public int OldLRU;

        /// <summary>
        /// Wether to use the LastRecentlyUsed cache. I think when this is enabled,
        /// the underlying dat file is of a fixed size and should not be expanded.
        /// So you must replace oldest files when writing new ones.
        /// Currently unsupported.
        /// </summary>
        public int UseLRU;

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
        /// Initialize a new empty dat header.
        /// </summary>
        internal DatHeader() {
            Magic = RETAIL_MAGIC;
            Transactions = new byte[64];
            WriteEmptyTransaction();
        }

        /// <summary>
        /// Initialize a new dat header. Should only be used when creating a new dat file from scratch.
        /// </summary>
        /// <param name="type">The type of dat database</param>
        /// <param name="subset">The sub type of the database</param>
        /// <param name="blockSize">The size of the blocks in the database, in bytes</param>
        /// <param name="version">The version string</param>
        /// <param name="engineVersion">engine version</param>
        /// <param name="gameVersion">game version</param>
        /// <param name="majorVersion">major version</param>
        /// <param name="minorVersion">minor version</param>
        public DatHeader(DatFileType type, uint subset, int blockSize = 1024, string? version = null, int engineVersion = 1, int gameVersion = 1, Guid majorVersion = new(), uint minorVersion = 1) {
            if (version?.Length > 255) {
                throw new InvalidOperationException($"Version string can be at max 255 characters. It was ${version.Length}");
            }
            Type = type;
            SubSet = subset;
            BlockSize = blockSize;
            EngineVersion = engineVersion;
            GameVersion = gameVersion;
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;

            Transactions = new byte[64];
            WriteEmptyTransaction();
        }

        internal void WriteEmptyTransaction() {
            Transactions = new byte[64];
            var writer = new DatBinWriter(Transactions);
            writer.WriteBytes([0x00, 0x50, 0x4C, 0x00], 4);
        }

        /// <inheritdoc/>
        public int GetSize() => SIZE;

        /// <returns>True if successful (the magic was good)</returns>
        /// <inheritdoc/>
        public bool Unpack(DatBinReader reader) {
            Version = Encoding.ASCII.GetString(reader.ReadBytes(256)).TrimEnd('\0');
            Transactions = reader.ReadBytes(64);
            Magic = reader.ReadInt32();
            BlockSize = reader.ReadInt32();
            FileSize = reader.ReadInt32();
            Type = (DatFileType)reader.ReadUInt32();
            SubSet = reader.ReadUInt32();
            FirstFreeBlock = reader.ReadInt32();
            LastFreeBlock = reader.ReadInt32();
            FreeBlockCount = reader.ReadInt32();
            RootBlock = reader.ReadInt32();
            NewLRU = reader.ReadInt32();
            OldLRU = reader.ReadInt32();
            UseLRU = reader.ReadInt32();
            MasterMapId = reader.ReadInt32();
            EngineVersion = reader.ReadInt32();
            GameVersion = reader.ReadInt32();
            MajorVersion = new Guid(reader.ReadBytes(16));
            MinorVersion = reader.ReadUInt32();

            return Magic == RETAIL_MAGIC;
        }

        /// <inheritdoc/>
        public bool Pack(DatBinWriter writer) {
            var versionBytes = new byte[256];
            versionBytes[0] = 0;
            if (Version is not null) {
                Encoding.ASCII.GetBytes(Version).CopyTo(versionBytes, 0);
                versionBytes[Version.Length] = 0;
            }

            writer.WriteBytes(versionBytes, 256);
            writer.WriteBytes(Transactions, 64);
            writer.WriteInt32(Magic);
            writer.WriteInt32(BlockSize);
            writer.WriteInt32(FileSize);
            writer.WriteUInt32((uint)Type);
            writer.WriteUInt32(SubSet);
            writer.WriteInt32(FirstFreeBlock);
            writer.WriteInt32(LastFreeBlock);
            writer.WriteInt32(FreeBlockCount);
            writer.WriteInt32(RootBlock);
            writer.WriteInt32(NewLRU);
            writer.WriteInt32(OldLRU);
            writer.WriteInt32(UseLRU);
            writer.WriteInt32(MasterMapId);
            writer.WriteInt32(EngineVersion);
            writer.WriteInt32(GameVersion);
            writer.WriteBytes(MajorVersion.ToByteArray(), 16);
            writer.WriteUInt32(MinorVersion);

            return true;
        }

        /// <inheritdoc/>
        public override string ToString() {
            var str = new StringBuilder();

            str.AppendLine($"Database Header:");
            str.AppendLine($"\t Version: {Version}");
            //str.AppendLine($"\t Version(String): {VersionString}");
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
