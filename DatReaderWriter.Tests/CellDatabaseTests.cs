﻿using DatReaderWriter;
using DatReaderWriter.Tests.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests {
    [TestClass]
    public class CellDatabaseTests {
        [TestMethod]
        [TestCategory("EOR")]
        public void CanOpenEoRCellDat() {
            using var dat = new CellDatabase(Path.Combine(EORCommonData.DatDirectory, $"client_cell_1.dat"));
            EORCommonData.AssertGoodHeader(dat.Header);
        }
    }
}
