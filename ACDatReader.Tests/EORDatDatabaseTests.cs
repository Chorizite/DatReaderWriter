using ACDatReader.Enums;
using ACDatReader.IO.BlockReaders;
using ACDatReader.Options;
using ACDatReader.Tests.Lib;
using System.Text;

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
                options.IndexCachingStrategy = IndexCachingStrategy.Upfront;
            }, GetReaderInstance(blockReader, Path.Combine(DatDirectory, $"client_{datPath}.dat")));

            Console.WriteLine(dat.Header);

            EORCommonData.AssertGoodHeader(dat.Header);
            EORCommonData.AssertGoodCaches(dat);
        }

        [TestMethod]
        [CombinatorialData]
        public void CanGetPortalFileBytes(
            [DataValues(BlockReaderType.MemoryMapped, BlockReaderType.FileStream)] BlockReaderType blockReader,
            [DataValues(0x06007569u, 0x01000001u, 0x0A00001Au)] uint fileId,
            [DataValues(IndexCachingStrategy.Never, IndexCachingStrategy.OnDemand, IndexCachingStrategy.Upfront)] IndexCachingStrategy lookupCachingStrategy
            ) {

            using var dat = new DatDatabaseReader(options => {
                options.IndexCachingStrategy = lookupCachingStrategy;
            }, GetReaderInstance(blockReader, Path.Combine(DatDirectory, $"client_portal.dat")));

            var foundBytes = dat.TryGetFileBytes(fileId, out var bytes);

            Assert.IsTrue(foundBytes);
            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length > 0);
            // TODO: contents checks
        }
    }
}