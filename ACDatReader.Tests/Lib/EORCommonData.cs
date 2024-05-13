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
            //Console.WriteLine($"Branches: {string.Join(", ", tree.Root?.Branches.Select(b => $"0x{b:X8}"))}");
            //Console.WriteLine($"Keys: {string.Join(", ", tree.Root?.Keys.Select(b => $"0x{b:X8}"))}");

            switch (GetDBType(tree)) {
                case EORDBType.Portal:
                    CollectionAssert.AreEqual(new List<int>() { 0x00000C00, 0x001F1400 }, tree.Root?.Branches);
                    CollectionAssert.AreEqual(new List<int>() { 0x05001B9C }, tree.Root?.Keys);
                    break;
                case EORDBType.HighRes:
                    CollectionAssert.AreEqual(new List<int>() { 0x0034FC00, 0x06812000 }, tree.Root?.Branches);
                    CollectionAssert.AreEqual(new List<int>() { 0x060043BE }, tree.Root?.Keys);
                    break;

                case EORDBType.Cell:
                    CollectionAssert.AreEqual(new List<int>() {
                        0x000E8B00, 0x10046C00, 0x10F87800, 0x11F49800, 0x01B77B00, 0x0284E100, 0x0341AE00, 0x04180700, 0x04D77500, 0x05924A00, 0x06403A00, 0x06E95E00, 0x07B3D800, 0x086C6D00, 0x09256900, 0x09F08500, 0x0AB48300, 0x0C48BD00, 0x0D173600, 0x0DDDDE00, 0x0E826E00, 0x12D8BA00, 0x12D8BA00
                    }, tree.Root?.Branches);
                    CollectionAssert.AreEqual(new List<int>() {
                        0x003A044F, 0x006D019B, 0x00B20199, 0x00F904EA, 0x014101B4, 0x01960386, 0x01EF03C2, 0x028301A9, 0x02C902E3, 0x0336013B, 0x1FF8FFFF, 0x52530120, 0x545C03B6, 0x5775014C, 0x5C4F075E, 0x604B028E, 0x7900025E, unchecked((int)0x8402016D), unchecked((int)0x8D0001EC), unchecked((int)0x9BD80106), unchecked((int)0xC95B0159)
                    }, tree.Root?.Keys);
                    break;

                case EORDBType.Language:
                    CollectionAssert.AreEqual(new List<int>() { 0x00000400, 0x0002B800, 0x000D9000 }, tree.Root?.Branches);
                    CollectionAssert.AreEqual(new List<int>() { 0x21000026, 0x21000045 }, tree.Root?.Keys);
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

        /*
        internal static void AssertGoodCaches(DatDatabaseReader db) {
            switch (db.Header.Type) {
                case DatDatabaseType.Portal:
                    if (db.Header.SubSet == 0) { // portal
                        Assert.AreEqual(79694, db.FileEntryCache.Count);
                    }
                    else { // highres
                        Assert.AreEqual(2295, db.FileEntryCache.Count);
                    }
                    break;

                case DatDatabaseType.Cell:
                    Assert.AreEqual(805348, db.FileEntryCache.Count);
                    break;

                case DatDatabaseType.Language:
                    Assert.AreEqual(118, db.FileEntryCache.Count);
                    break;

                default:
                    Assert.Fail($"Dat Header type {db.Header.Type} was of an unknown type");
                    break;
            }
        }
        */
    }
}