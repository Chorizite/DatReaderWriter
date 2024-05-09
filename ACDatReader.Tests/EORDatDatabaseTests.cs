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

            using var dat = new DatDatabaseReader(options => {
                options.PreloadFileEntries = true;
            }, GetReaderInstance(blockReader, Path.Combine(DatDirectory, $"client_{datPath}.dat")));

            EORCommonData.AssertGoodHeader(dat.Header);
            EORCommonData.AssertGoodCaches(dat);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanGetPortalFileBytes(
            [DataValues(BlockReaderType.MemoryMapped, BlockReaderType.FileStream)] BlockReaderType blockReader,
            [DataValues(0x06007569u, 0x01000001u, 0x0A00001Au)] uint fileId,
            [DataValues(true, false)] bool preloadFileEntries
            ) {

            using var dat = new DatDatabaseReader(options => {
                options.PreloadFileEntries = preloadFileEntries;
            }, GetReaderInstance(blockReader, Path.Combine(DatDirectory, $"client_portal.dat")));


            var foundBytes = dat.TryGetFileBytes(fileId, out var bytes);

            Assert.IsTrue(foundBytes);
            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length > 0);
            // TODO: contents checks
        }
    }
}