using ACDatReader.IO.BlockAllocators;
using ACDatReader.IO.DatBTree;
using ACDatReader.Options;

namespace ACDatPacker {
    internal class Program {
        static void Main(string[] args) {
            PackDat(@"C:\Turbine\Asheron's Call\client_highres.dat", @"C:\Turbine\Asheron's Call - Copy\client_highres.dat");
            PackDat(@"C:\Turbine\Asheron's Call\client_local_English.dat", @"C:\Turbine\Asheron's Call - Copy\client_local_English.dat");
            PackDat(@"C:\Turbine\Asheron's Call\client_portal.dat", @"C:\Turbine\Asheron's Call - Copy\client_portal.dat");
            PackDat(@"C:\Turbine\Asheron's Call\client_cell_1.dat", @"C:\Turbine\Asheron's Call - Copy\client_cell_1.dat");
        }

        static void PackDat(string sourceDatPath, string outDatPath) {
            Console.WriteLine($"Writing {sourceDatPath} to {outDatPath}");

            using var sourceDat = new DatBTreeReaderWriter(new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
                FilePath = sourceDatPath
            }));
            Console.WriteLine($"\tSource block size: {sourceDat.BlockAllocator.Header.BlockSize} Free: {sourceDat.BlockAllocator.Header.FreeBlockCount}");


            //Console.WriteLine(sourceDat);

            if (File.Exists(outDatPath)) {
                File.Delete(outDatPath);
            }

            var blockAllocator = new MemoryMappedBlockAllocator(new DatDatabaseOptions() {
                FilePath = outDatPath,
                AccessType = DatAccessType.ReadWrite
            });
            blockAllocator.InitNew(sourceDat.BlockAllocator.Header.Type, sourceDat.BlockAllocator.Header.SubSet, sourceDat.BlockAllocator.Header.BlockSize, 1);

            using var packedDat = new DatBTreeReaderWriter(blockAllocator);
            packedDat.BlockAllocator.SetVersion("", sourceDat.BlockAllocator.Header.EngineVersion, sourceDat.BlockAllocator.Header.GameVersion, sourceDat.BlockAllocator.Header.MajorVersion, sourceDat.BlockAllocator.Header.MinorVersion);

            packedDat.BlockAllocator.Header.Transactions = sourceDat.BlockAllocator.Header.Transactions;
            packedDat.BlockAllocator.Header.MasterMapId = sourceDat.BlockAllocator.Header.MasterMapId;

            var bytes = new byte[20_000_000];
            var sourceFileCount = 0;
            foreach (var fileEntry in sourceDat) {

                sourceDat.BlockAllocator.ReadBlock(bytes, fileEntry.Offset);
                sourceFileCount++;

                var wantedBlocks = (int)Math.Ceiling((float)fileEntry.Size / (sourceDat.BlockAllocator.Header.BlockSize - 4)) - packedDat.BlockAllocator.Header.FreeBlockCount;
                if (wantedBlocks > 0) {
                    //packedDat.BlockAllocator.AllocateEmptyBlocks(wantedBlocks + 3);
                }
                var newFileOffset = packedDat.BlockAllocator.WriteBlock(bytes, (int)fileEntry.Size);
                packedDat.Insert(new DatBTreeFile() {
                    Offset = newFileOffset,
                    Id = fileEntry.Id,
                    Size = fileEntry.Size,
                    Date = fileEntry.Date,
                    Flags = fileEntry.Flags,
                    Iteration = fileEntry.Iteration
                });
            }
            Console.WriteLine($"\tDest block size: {packedDat.BlockAllocator.Header.BlockSize} Free: {packedDat.BlockAllocator.Header.FreeBlockCount}");

            var destFileCount = 0;
            foreach (var fileEntry in packedDat) {
                destFileCount++;
            }
            Console.WriteLine($"\tSourceFileCount: {sourceFileCount}\n\tDestFileCount:{destFileCount}");

            var sourceNodeCount = 0;
            var sourceEntryCount = 0;
            var sourceLowestOffset = uint.MaxValue;
            var destNodeCount = 0;
            var destEntryCount = 0;
            var destLowestOffset = uint.MaxValue;

            sourceDat.CountNodes(ref sourceNodeCount, ref sourceEntryCount, ref sourceLowestOffset);
            packedDat.CountNodes(ref destNodeCount, ref destEntryCount, ref destLowestOffset);

            Console.WriteLine($"\tSourceNodes: {sourceNodeCount} / {sourceEntryCount} entries. Lowest: {sourceLowestOffset:X8}");
            Console.WriteLine($"\tDestNodes: {destNodeCount} / {destEntryCount} entries. Lowest: {destLowestOffset:X8}");
        }
    }
}
