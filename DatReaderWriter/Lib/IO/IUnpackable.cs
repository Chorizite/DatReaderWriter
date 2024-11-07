using System;

namespace DatReaderWriter.Lib.IO {
    /// <summary>
    /// Can be unpacked from a dat file
    /// </summary>
    public interface IUnpackable {
        /// <summary>
        /// Unpack this instance from a reader
        /// </summary>
        /// <param name="reader">The reader to read from</param>
        /// <returns>true if successful</returns>
        public bool Unpack(DatBinReader reader);
    }
}
