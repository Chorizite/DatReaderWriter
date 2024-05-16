using ACDatReader.IO;
using ACDatReader.IO.DatBTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ACDatReader.Tests.Lib {
    public enum EORDBType {
        Unknown = 0,
        Portal,
        Cell,
        Language,
        HighRes
    }

    internal static class EORCommonData {
        internal const string DatDirectory = @"C:\Turbine\Asheron's Call\";

        internal const string PortalName = @"portal";
        internal const string CellName = @"cell_1";
        internal const string LanguageName = @"local_English";
        internal const string HighResName = @"highres";

        internal static string PortalPath => Path.Combine(DatDirectory, $"client_{PortalName}.dat");
        internal static string CellPath => Path.Combine(DatDirectory, $"client_{CellName}.dat");
        internal static string LanguagePath => Path.Combine(DatDirectory, $"client_{LanguageName}.dat");
        internal static string HighResPath => Path.Combine(DatDirectory, $"client_{HighResName}.dat");

        internal static string GetDatPath(EORDBType dbType) => dbType switch {
            EORDBType.Portal => PortalPath,
            EORDBType.Cell => CellPath,
            EORDBType.Language => LanguagePath,
            EORDBType.HighRes => HighResPath,
            _ => throw new NotImplementedException($"Unknown dat type"),
        };

        internal static EORDBType GetDBType(DatBTreeReaderWriter tree) {
            return GetDBType(tree.BlockAllocator.Header);
        }

        internal static EORDBType GetDBType(DatHeader header) {
            switch (header.Type) {
                case DatDatabaseType.Portal:
                    if (header.SubSet == 0) {
                        return EORDBType.Portal;
                    }
                    else {
                        return EORDBType.HighRes;
                    }

                case DatDatabaseType.Cell:
                    return EORDBType.Cell;

                case DatDatabaseType.Language:
                    return EORDBType.Language;

                default:
                    return EORDBType.Unknown;
            }
        }

        internal static void AssertGoodRootNode(DatBTreeReaderWriter tree) {
            Console.WriteLine(tree.BlockAllocator.Header);
            Console.WriteLine($"Branches: {string.Join(", ", tree.Root?.Branches?.Select(b => $"0x{b:X8}") ?? [])}");
            Console.WriteLine($"Keys: {string.Join(", ", tree.Root?.Files?.Select(f => f.Id).ToList().Select(b => $"0x{b:X8}") ?? [])}");

            switch (GetDBType(tree)) {
                case EORDBType.Portal:
                    CollectionAssert.AreEqual(new List<int>() { 0x00114400, 0x0FA3D800, 0x0473E800, 0x114EB800, 0x077BB000, 0x1617E400, 0x02D58800, 0x24B09800, 0x2BF3D400, 0x2B395800, 0x00D6F000, 0x08A61000, 0x1C478C00, 0x241ACC00, 0x112F7800, 0x328FA400, 0x15951800, 0x1A75D000, 0x1E29AC00, 0x0442FC00, 0x141A9800, 0x1CC74800, 0x08089000, 0x101E2800, 0x33654800, 0x24DCC000, 0x11D3A000, 0x13DA5000, 0x155F1400, 0x17582800, 0x195E4000, 0x0232DC00, 0x05401400, 0x08878800, 0x0E24E000, 0x14EE0C00, 0x1AABC800, 0x1C856800, 0x1FAD2800, 0x242BDC00, 0x27EFC000, 0x3236A000, 0x02B37000, 0x054B6000, 0x07CDE000, 0x16551800, 0x1EE0E000, 0x2CADC400, 0x0FBF2800, 0x14AA7400, 0x1AB50000, 0x0363A400, 0x102EB000, 0x1993DC00, 0x06007800, 0x2033FC00, 0x10044000, 0x13565000, 0x20D74000 }, tree.Root?.Branches);
                    CollectionAssert.AreEqual(new List<uint>() { 0x01000D8E, 0x0100167F, 0x01001F38, 0x0100281C, 0x010030C4, 0x01003AE2, 0x01003F6C, 0x010043DE, 0x01004854, 0x020004FB, 0x02000AF3, 0x02001063, 0x02001557, 0x03000803, 0x03000CA1, 0x040010AF, 0x040016DC, 0x04001B91, 0x05000DAF, 0x05001A02, 0x05002138, 0x05002691, 0x05002A69, 0x05002E89, 0x0600105F, 0x06001831, 0x06001E6E, 0x06002503, 0x06002A52, 0x06002ED7, 0x060037DE, 0x06003E8C, 0x0600452E, 0x06004B49, 0x06005123, 0x06005747, 0x06005B9A, 0x060060A6, 0x060065B4, 0x060069F3, 0x06006F47, 0x08000229, 0x08000601, 0x080009DD, 0x08000DB6, 0x080011D5, 0x09000021, 0x0D00007A, 0x0F00059E, 0x10000244, 0x11000195, 0x11000574, 0x11000941, 0x11000DFE, 0x32000208, 0x3300003D, 0x33000641, 0x33000C0F }, tree.Root?.Files.Select(f => f.Id).ToList());
                    break;
                case EORDBType.HighRes:
                    CollectionAssert.AreEqual(new List<int>() { 0x0034FC00, 0x06812000 }, tree.Root?.Branches);
                    CollectionAssert.AreEqual(new List<uint>() { 0x060043BE }, tree.Root?.Files.Select(f => f.Id).ToList());
                    break;

                case EORDBType.Cell:
                    CollectionAssert.AreEqual(new List<int>() {
                        0x000E8B00, 0x10046C00, 0x10F87800, 0x11F49800, 0x01B77B00, 0x0284E100, 0x0341AE00, 0x04180700, 0x04D77500, 0x05924A00, 0x06403A00, 0x06E95E00, 0x07B3D800, 0x086C6D00, 0x09256900, 0x09F08500, 0x0AB48300, 0x0C48BD00, 0x0D173600, 0x0DDDDE00, 0x0E826E00, 0x12D8BA00, 0x12D8BA00
                    }, tree.Root?.Branches);
                    CollectionAssert.AreEqual(new List<uint>() {
                        0x003A044F, 0x006D019B, 0x00B20199, 0x00F904EA, 0x014101B4, 0x01960386, 0x01EF03C2, 0x028301A9, 0x02C902E3, 0x0336013B, 0x1FF8FFFF, 0x52530120, 0x545C03B6, 0x5775014C, 0x5C4F075E, 0x604B028E, 0x7900025E, 0x8402016D, 0x8D0001EC, 0x9BD80106, 0xC95B0159
                    }, tree.Root?.Files.Select(f => f.Id).ToList());
                    break;

                case EORDBType.Language:
                    CollectionAssert.AreEqual(new List<int>() { 0x00000400, 0x0002B800, 0x000D9000 }, tree.Root?.Branches);
                    CollectionAssert.AreEqual(new List<uint>() { 0x21000026, 0x21000045 }, tree.Root?.Files.Select(f => f.Id).ToList());
                    break;
                default:
                    Assert.Fail($"Dat Header type {tree.BlockAllocator.Header.Type} was of an unknown type");
                    break;
            }
        }

        internal static void AssertGoodHeader(DatHeader header) {
            switch (GetDBType(header)) {
                case EORDBType.Portal:
                    Assert.AreEqual(0x00005442, header.Magic);
                    Assert.AreEqual(1024, header.BlockSize);
                    Assert.AreEqual(110, header.EngineVersion);
                    Assert.AreEqual(6657u, header.MinorVersion);
                    Assert.AreEqual(0u, header.SubSet);
                    break;
                case EORDBType.HighRes:
                    Assert.AreEqual(0x00005442, header.Magic);
                    Assert.AreEqual(1024, header.BlockSize);
                    Assert.AreEqual(110, header.EngineVersion);
                    Assert.AreEqual(6657u, header.MinorVersion);
                    Assert.AreEqual(1766222152u, header.SubSet);
                    break;

                case EORDBType.Cell:
                    Assert.AreEqual(0x00005442, header.Magic);
                    Assert.AreEqual(256, header.BlockSize);
                    Assert.AreEqual(22, header.EngineVersion);
                    Assert.AreEqual(6657u, header.MinorVersion);
                    Assert.AreEqual(1u, header.SubSet);
                    break;

                case EORDBType.Language:
                    Assert.AreEqual(0x00005442, header.Magic);
                    Assert.AreEqual(1024, header.BlockSize);
                    Assert.AreEqual(110, header.EngineVersion);
                    Assert.AreEqual(6657u, header.MinorVersion);
                    Assert.AreEqual(1u, header.SubSet);
                    break;

                default:
                    Assert.Fail($"Dat Header type {header.Type} was of an unknown type");
                    break;
            }
        }

        internal static void AssertGoodFileEntryCount(EORDBType type, int fileCount) {
            switch (type) {
                case EORDBType.Portal:
                    Assert.AreEqual(79694, fileCount);
                    break;
                case EORDBType.HighRes:
                    Assert.AreEqual(2295, fileCount);
                    break;

                case EORDBType.Cell:
                    Assert.AreEqual(805348, fileCount);
                    break;

                case EORDBType.Language:
                    Assert.AreEqual(118, fileCount);
                    break;

                default:
                    Assert.Fail($"Dat Header type {type} was of an unknown type");
                    break;
            }
        }
    }
}