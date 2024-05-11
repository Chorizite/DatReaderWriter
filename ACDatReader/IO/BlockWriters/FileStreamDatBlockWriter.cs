using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACDatReader.IO.BlockWriters {
    public class FileStreamDatBlockWriter : IDatBlockWriter {
        public void Dispose() {
            throw new NotImplementedException();
        }

        public void WriteBlocks(byte[] buffer, uint startingBlock, int blockSize, IDatBlockAllocator blockProvider) {
            throw new NotImplementedException();
        }

        public void WriteBytes(byte[] buffer, uint blockOffset) {
            throw new NotImplementedException();
        }
    }
}
