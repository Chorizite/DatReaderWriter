using DatReaderWriter.Options;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using DatReaderWriter.Lib.IO.BlockAllocators;

// $ dotnet run -c Release --framework net8.0 -- --job short --runtimes net48 net8.0

namespace DatReaderWriter.Benchmarks {
    public enum BlockReaderType {
        MemoryMapped,
        FileStream
    }

    public class Program {
        static void Main() {
            BenchmarkRunner.Run<DatFileIteration>();
            BenchmarkRunner.Run<PortalDatFileFetching>();
            BenchmarkRunner.Run<CellDatFileFetching>();
        }

        public static IDatBlockAllocator GetBlockAllocator(BlockReaderType type, string datFilePath) {
            return type switch {
                BlockReaderType.MemoryMapped => new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
                    FilePath = datFilePath
                }),
                BlockReaderType => new StreamBlockAllocator(new DatDatabaseOptions() {
                    FilePath = datFilePath
                })
            };
        }
    }

    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net48)]
    [SimpleJob(RuntimeMoniker.Net80, baseline: true)]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class DatFileIteration {
        private const string DatDirectory = @"C:\Turbine\Asheron's Call\";
        private const string PortalFile = "portal";
        private const string CellFile = "cell_1";
        private const string LanguageFile = "local_English";


        //[Params(PortalFile, CellFile, LanguageFile)]
        //public string? DatFile { get; set; }

        [Params(BlockReaderType.FileStream, BlockReaderType.MemoryMapped)]
        public BlockReaderType Reader { get; set; }

        [Benchmark]
        [BenchmarkCategory]
        public void CacheDat() {
            var reader = new DatDatabase(options => {
                options.IndexCachingStrategy = Options.IndexCachingStrategy.Upfront;
            }, Program.GetBlockAllocator(Reader, Path.Combine(DatDirectory, $"client_{CellFile}.dat")));

            var fileCount = reader.Tree.Count();
        }
    }

    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net48, baseline: true)]
    [SimpleJob(RuntimeMoniker.Net80)]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class CellDatFileFetching {
        private const string CellFile = @"C:\Turbine\Asheron's Call\client_cell_1.dat";

        public uint[] FileIds => [0xFB7CFFFEu, 0x1890FFFFu, 0x0001FFFEu, 0xFEFEFFFFu, 0x0390FFFEu, 0xB784FFFFu, 0xB5A4FFFEu, 0x8686FFFFu];

        [Params(BlockReaderType.FileStream, BlockReaderType.MemoryMapped)]
        public BlockReaderType Reader { get; set; }

        //[Params(IndexCachingStrategy.Upfront, IndexCachingStrategy.OnDemand, IndexCachingStrategy.Never)]
        //public IndexCachingStrategy CacheStrategy { get; set; }

        [Benchmark]
        public void Fetch8Files100x() {
            IDatBlockAllocator? reader = Program.GetBlockAllocator(Reader, CellFile);

            var db = new DatDatabase((options) => {
                //options.IndexCachingStrategy = CacheStrategy;
            }, reader);

            for (var i = 0; i < 100; i++) {
                foreach (var fileId in FileIds) {
                    _ = db.TryGetFileBytes(fileId, out var _);
                }
            }
        }
    }

    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net48, baseline: true)]
    [SimpleJob(RuntimeMoniker.Net80)]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class PortalDatFileFetching {
        private const string PortalFile = @"C:\Turbine\Asheron's Call\client_portal.dat";

        public uint[] FileIds => [0x01000001u, 0x06007569u, 0x0A00001Au, 0x340000CFu, 0x1000057Au, 0x22000033u, 0x0300004Bu, 0x0900002Du];

        [Params(BlockReaderType.FileStream, BlockReaderType.MemoryMapped)]
        public BlockReaderType Reader { get; set; }

        //[Params(IndexCachingStrategy.Upfront, IndexCachingStrategy.OnDemand, IndexCachingStrategy.Never)]
        //public IndexCachingStrategy CacheStrategy { get; set; }

        [Benchmark]
        public void Fetch8Files100x() {
            IDatBlockAllocator? reader = Program.GetBlockAllocator(Reader, PortalFile);

            var db = new DatDatabase((options) => {
                //options.IndexCachingStrategy = CacheStrategy;
            }, reader);

            for (var i = 0; i < 100; i++) {
                foreach (var fileId in FileIds) {
                    _ = db.TryGetFileBytes(fileId, out var _);
                }
            }
        }
    }
}
