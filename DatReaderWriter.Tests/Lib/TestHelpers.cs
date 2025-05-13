using DatReaderWriter.Lib.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests.Lib {
    internal static class TestHelpers {
        internal static void CanReadAndWriteIdentical<T>(string datPath, uint objId, DatDatabase? existingDat = null) where T : IDBObj {
            DatDatabase dat = existingDat;
            if (dat == null) {
                dat = new DatDatabase(options => {
                    options.FilePath = datPath;
                });
            }

            dat.Tree.TryGetFile(objId, out var originalEntry);
            Assert.IsNotNull(originalEntry);

            var res = dat.TryReadFile<T>(objId, out var file);
            Assert.IsTrue(res);
            Assert.IsNotNull(file);

            var originalFileJson = JsonConvert.SerializeObject(file, Formatting.Indented);
            //Console.WriteLine(originalFileJson);

            var originalBytes = new byte[originalEntry.Size];
            dat.BlockAllocator.ReadBlock(originalBytes, originalEntry.Offset);

            var writtenBytes = new byte[originalEntry.Size * 2];
            var writer = new DatBinWriter(writtenBytes, dat);
            file.Pack(writer);

            var readRes = dat.TryReadFile<T>(objId, out var writtenFile);
            Assert.IsTrue(readRes);
            Assert.IsNotNull(writtenFile);

            var max = Math.Min(writtenBytes.Length, originalBytes.Length);

            //Console.WriteLine($"Original size: {originalEntry.Size} bytes");
            //Console.WriteLine($"{string.Join(" ", originalBytes.Select(b => b.ToString("X2")))}");
            //Console.WriteLine($"Written size: {writer.Offset} bytes");
            //Console.WriteLine($"{string.Join(" ", writtenBytes.Select(b => b.ToString("X2")))}");

            var writtenFileJson = JsonConvert.SerializeObject(writtenFile, Formatting.Indented);
            Assert.AreEqual(originalFileJson, writtenFileJson, false, "Written JSON does not match original JSON");

            for (var i = 0; i < max; i++) {
                if (originalBytes[i] != writtenBytes[i]) {
                    Console.WriteLine($"Byte {i} does not match");
                    Console.WriteLine();
                    var min = Math.Max(i-16, 0);
                    Console.WriteLine($"Original@{min}: {string.Join(" ", originalBytes.Skip(min).Take(16).Select(b => b.ToString("X2")))}");
                    Console.WriteLine($"Written@{min}: {string.Join(" ", writtenBytes.Skip(min).Take(16).Select(b => b.ToString("X2")))}");
                    Console.WriteLine();
                    Console.WriteLine($"Original@{i}: {string.Join(" ", originalBytes.Skip(i).Take(16).Select(b => b.ToString("X2")))}");
                    Console.WriteLine($"Written@{i}: {string.Join(" ", writtenBytes.Skip(i).Take(16).Select(b => b.ToString("X2")))}");
                }
                Assert.AreEqual(originalBytes[i], writtenBytes[i], $"Byte {i} does not match");
            }

            //CollectionAssert.AreEqual(originalBytes, writtenBytes, "Written bytes do not match original bytes");

            Assert.AreEqual((int)originalEntry.Size, writer.Offset, "Written size does not match original size");

            if (existingDat == null) dat.Dispose();
        }
    }
}
