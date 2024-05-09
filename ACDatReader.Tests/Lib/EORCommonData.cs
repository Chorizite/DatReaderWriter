using ACDatReader.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ACDatReader.Tests.Lib {
    internal static class EORCommonData {
        internal static void AssertGoodCaches(DatHeader header) {
            throw new NotImplementedException();
        }

        internal static void AssertGoodHeader(DatHeader header) {
            switch (header.Type) {
                case DatDatabaseType.Portal:
                    if (header.SubSet == 0) { // portal
                        Assert.AreEqual(0x00005442, header.Magic);
                        Assert.AreEqual(1024, header.BlockSize);
                        Assert.AreEqual(110u, header.EngineVersion);
                        Assert.AreEqual(6657u, header.MinorVersion);
                        Assert.AreEqual(0, header.SubSet);
                    }
                    else { // highres
                        Assert.AreEqual(0x00005442, header.Magic);
                        Assert.AreEqual(1024, header.BlockSize);
                        Assert.AreEqual(110u, header.EngineVersion);
                        Assert.AreEqual(6657u, header.MinorVersion);
                        Assert.AreEqual(1766222152, header.SubSet);
                    }
                    break;

                case DatDatabaseType.Cell:
                    Assert.AreEqual(0x00005442, header.Magic);
                    Assert.AreEqual(256, header.BlockSize);
                    Assert.AreEqual(22u, header.EngineVersion);
                    Assert.AreEqual(6657u, header.MinorVersion);
                    Assert.AreEqual(1, header.SubSet);
                    break;

                case DatDatabaseType.Language:
                    Assert.AreEqual(0x00005442, header.Magic);
                    Assert.AreEqual(1024, header.BlockSize);
                    Assert.AreEqual(110u, header.EngineVersion);
                    Assert.AreEqual(6657u, header.MinorVersion);
                    Assert.AreEqual(1, header.SubSet);
                    break;

                default:
                    Assert.Fail($"Dat Header type {header.Type} was of an unknown type");
                    break;
            }
        }

        internal static void AssertGoodCaches(DatDatabase db) {
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
    }
}
