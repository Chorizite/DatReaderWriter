using DatReaderWriter.Lib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests.Lib {
    internal static class TestHelpers {
        internal static void CanReadAndWriteIdentical<T>(string datPath, uint objId) where T : IDBObj {
            using var dat = new DatDatabase(options => {
                options.FilePath = datPath;
            });

            dat.Tree.TryGetFile(objId, out var originalEntry);
            Assert.IsNotNull(originalEntry);
            var res = dat.TryReadFile<T>(objId, out var file);
            Assert.IsNotNull(file);

            var originalBytes = new byte[originalEntry.Size];
            dat.BlockAllocator.ReadBlock(originalBytes, originalEntry.Offset);

            var writtenBytes = new byte[originalEntry.Size];
            var writer = new DatBinWriter(writtenBytes, dat);
            file.Pack(writer);

            var max = Math.Min(writtenBytes.Length, originalBytes.Length);
            var i = 0;
            while (i < max && originalBytes[i] == writtenBytes[i]) i++;

            //Console.WriteLine($"Original size: {originalEntry.Size} bytes");
            //Console.WriteLine($"{string.Join(" ", originalBytes.Select(b => b.ToString("X2")))}");
            //Console.WriteLine($"Written size: {writer.Offset} bytes");
            //Console.WriteLine($"{string.Join(" ", writtenBytes.Select(b => b.ToString("X2")))}");

            CollectionAssert.AreEqual(originalBytes, writtenBytes);

            Assert.AreEqual((int)originalEntry.Size, writer.Offset, "Written size does not match original size");

            dat.Dispose();
        }
    }
}
