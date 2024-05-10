using ACDatReader.IO;
using ACDatReader.IO.BlockReaders;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;

namespace ACDatReader.Benchmarks {
    public class Program {
        static void Main() {
            //BenchmarkRunner.Run<DatFileEntryCaching>();
            BenchmarkRunner.Run<PortalDatFileFetchingNoCache>();
        }
    }

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
            _ = new DatDatabaseReader(null, new MemoryMappedDatBlockReader(Path.Combine(DatDirectory, $"client_{DatFile}.dat")));
        }

        [Benchmark]
        public void CacheDatStream() {
            _ = new DatDatabaseReader(null, new FileStreamDatBlockReader(Path.Combine(DatDirectory, $"client_{DatFile}.dat")));
        }

        [Benchmark]
        public void CacheDatACE() {
            _ = new ACE.DatLoader.DatDatabase(Path.Combine(DatDirectory, $"client_{DatFile}.dat"), true);
        }
    }

    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class PortalDatFileFetchingNoCache {
        private const string PortalFile = @"C:\Turbine\Asheron's Call\client_portal.dat";

        [Params(0x01000001u, 0x06007569u, 0x0A00001Au)]
        public uint FileId { get; set; }

        private readonly DatDatabaseReader db = new ((options) => {
            options.CacheDirectories = false;
            options.PreloadFileEntries = false;
        }, new MemoryMappedDatBlockReader(PortalFile));


        [Benchmark]
        public void FetchMemMap() {
            _ = db.TryGetFileBytes(FileId, out var _);
        }

        [Benchmark]
        public void FetchStream() {
            _ = db.TryGetFileBytes(FileId, out var _);
        }
    }
}
