using ACDatReader.IO.BlockAllocators;
using ACDatReader.IO.DatBTree;
using ACDatReader.Tests.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACDatReader.Tests.IO.DatBTree {
    [TestClass]
    public class DatBTreeReaderWriterTests {
        [TestMethod]
        [TestCategory("EOR")]
        [CombinatorialData]
        public void CanReadEORRootNodes(
            [DataValues(EORDBType.Portal, EORDBType.Cell, EORDBType.Language, EORDBType.HighRes)] EORDBType dbType
            ) {

            using var tree = new DatBTreeReaderWriter(new MemoryMappedBlockAllocator(new Options.DatDatabaseOptions() {
                FilePath = EORCommonData.GetDatPath(dbType)
            }));

            Assert.IsNotNull(tree.Root);
            Assert.IsFalse(tree.Root.IsLeaf);

            EORCommonData.AssertGoodRootNode(tree);
        }
    }
}
