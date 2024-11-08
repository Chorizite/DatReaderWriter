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
            var res = dat.TryReadFile<T>(objId, out var spellTable);
            Assert.IsNotNull(spellTable);

            var originalBytes = new byte[originalEntry.Size];
            dat.BlockAllocator.ReadBlock(originalBytes, originalEntry.Offset);

            var writtenBytes = new byte[originalEntry.Size];
            var writer = new DatBinWriter(writtenBytes);
            spellTable.Pack(writer);

            Assert.AreEqual((int)originalEntry.Size, writer.Offset);
            CollectionAssert.AreEqual(originalBytes, writtenBytes);

            dat.Dispose();
        }
    }
}
