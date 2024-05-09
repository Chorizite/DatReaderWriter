using ACDatReader.Enums;
using ACDatReader.IO;
using ACDatReader.IO.BlockReaders;
using ACDatReader.Tests.Lib;

namespace ACDatReader.Tests {
    [TestClass]
    public class EORDatDatabaseTests {
        private const string DatDirectory = @"C:\Turbine\Asheron's Call\";

        private const string PortalPath = @"portal";
        private const string CellPath = @"cell_1";
        private const string LanguagePath = @"local_English";
        private const string HighResPath = @"highres";

        public enum BlockReaderType {
            MemoryMapped,
            FileStream
        }

        private static IDatBlockReader GetReaderInstance(BlockReaderType type, string datPath) {
            return type switch {
                BlockReaderType.MemoryMapped => new MemoryMappedDatBlockReader(datPath),
                BlockReaderType.FileStream => new FileStreamDatBlockReader(datPath),
                _ => throw new InvalidDataException(),
            };
        }

        [TestMethod]
        [CombinatorialData]
        public void CanParseEORDats(
            [DataValues(PortalPath, CellPath, LanguagePath, HighResPath)] string datPath,
            [DataValues(BlockReaderType.MemoryMapped, BlockReaderType.FileStream)] BlockReaderType blockReader
            ) {

            using var dat = new DatDatabase(options => {
                options.PreloadFileEntries = true;
            }, GetReaderInstance(blockReader, Path.Combine(DatDirectory, $"client_{datPath}.dat")));

            EORCommonData.AssertGoodHeader(dat.Header);
            EORCommonData.AssertGoodCaches(dat);
        }
    }
}