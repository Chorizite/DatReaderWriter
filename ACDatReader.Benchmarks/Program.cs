
using ACDatReader.Options;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;

namespace ACDatReader.Benchmarks {
    public class Program {
        static void Main() {
            //BenchmarkRunner.Run<DatFileEntryCaching>();
            //BenchmarkRunner.Run<PortalDatFileFetching>();
            //BenchmarkRunner.Run<CellDatFileFetching>();
        }
    }

    public enum BlockReaderType {
        MemoryMapped,
        FileStream
    }

    /*
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class DatFileEntryCaching {
        private const string DatDirectory = @"C:\Turbine\Asheron's Call\";
        private const string PortalFile = "portal";
        private const string CellFile = "cell_1";
        private const string LanguageFile = "local_English";


        [Params(PortalFile, CellFile, LanguageFile)]
        public string? DatFile { get; set; }

        [Benchmark]
        public void CacheDatMemMap() {
            _ = new DatDatabaseReader(options => {
                options.IndexCachingStrategy = Options.IndexCachingStrategy.Upfront;
            }, new MemoryMappedDatBlockReader(Path.Combine(DatDirectory, $"client_{DatFile}.dat")));
        }

        [Benchmark]
        public void CacheDatStream() {
            _ = new DatDatabaseReader(options => {
                options.IndexCachingStrategy = Options.IndexCachingStrategy.Upfront;
            }, new FileStreamDatBlockReader(Path.Combine(DatDirectory, $"client_{DatFile}.dat")));
        }

        [Benchmark]
        public void CacheDatACE() {
            _ = new ACE.DatLoader.DatDatabase(Path.Combine(DatDirectory, $"client_{DatFile}.dat"), true);
        }
    }

    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class CellDatFileFetching {
        private const string CellFile = @"C:\Turbine\Asheron's Call\client_cell_1.dat";

        public uint[] FileIds => [0xFB7CFFFEu, 0x1890FFFFu, 0x0001FFFEu, 0xFEFEFFFFu, 0x0390FFFEu, 0xB784FFFFu, 0xB5A4FFFEu, 0x8686FFFFu];

        [Params(BlockReaderType.FileStream, BlockReaderType.MemoryMapped)]
        public BlockReaderType Reader { get; set; }

        [Params(IndexCachingStrategy.Upfront, IndexCachingStrategy.OnDemand, IndexCachingStrategy.Never)]
        public IndexCachingStrategy CacheStrategy { get; set; }

        [Benchmark]
        public void Fetch8Files100x() {
            IDatBlockReader? reader = Reader switch {
                BlockReaderType.MemoryMapped => new MemoryMappedDatBlockReader(CellFile),
                BlockReaderType.FileStream => new FileStreamDatBlockReader(CellFile),
                _ => null
            };

            var db = new DatDatabaseReader((options) => {
                options.IndexCachingStrategy = CacheStrategy;
            }, reader);

            for (var i = 0; i < 100; i++) {
                foreach (var fileId in FileIds) {
                    _ = db.TryGetFileBytes(fileId, out var _);
                }
            }
        }
    }

    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class PortalDatFileFetching {
        private const string PortalFile = @"C:\Turbine\Asheron's Call\client_portal.dat";

        public uint[] FileIds => [0x01000001u, 0x06007569u, 0x0A00001Au, 0x340000CFu, 0x1000057Au, 0x22000033u, 0x0300004Bu, 0x0900002Du];

        [Params(BlockReaderType.FileStream, BlockReaderType.MemoryMapped)]
        public BlockReaderType Reader { get; set; }

        [Params(IndexCachingStrategy.Upfront, IndexCachingStrategy.OnDemand, IndexCachingStrategy.Never)]
        public IndexCachingStrategy CacheStrategy { get; set; }

        [Benchmark]
        public void Fetch8Files100x() {
            IDatBlockReader? reader = Reader switch {
                BlockReaderType.MemoryMapped => new MemoryMappedDatBlockReader(PortalFile),
                BlockReaderType.FileStream => new FileStreamDatBlockReader(PortalFile),
                _ => null
            };

            var db = new DatDatabaseReader((options) => {
                options.IndexCachingStrategy = CacheStrategy;
            }, reader);

            for (var i = 0; i < 100; i++) {
                foreach (var fileId in FileIds) {
                    _ = db.TryGetFileBytes(fileId, out var _);
                }
            }
        }
    }
    */
}
