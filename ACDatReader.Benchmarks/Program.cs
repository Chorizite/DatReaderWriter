using ACDatReader.IO;
using ACDatReader.IO.BlockReaders;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;

namespace ACDatReader.Benchmarks {
    public class Program {
        static void Main() {
            BenchmarkRunner.Run<DatFileEntryCaching>();
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
            _ = new DatDatabase(null, new MemoryMappedDatBlockReader(Path.Combine(DatDirectory, $"client_{DatFile}.dat")));
        }

        [Benchmark]
        public void CacheDatStream() {
            _ = new DatDatabase(null, new FileStreamDatBlockReader(Path.Combine(DatDirectory, $"client_{DatFile}.dat")));
        }

        [Benchmark]
        public void CacheDatACE() {
            _ = new ACE.DatLoader.DatDatabase(Path.Combine(DatDirectory, $"client_{DatFile}.dat"), true);
        }
    }
}
