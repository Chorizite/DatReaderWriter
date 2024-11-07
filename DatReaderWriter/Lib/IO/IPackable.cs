using System;

namespace DatReaderWriter.Lib.IO {
    /// <summary>
    /// Can be packed to a dat file
    /// </summary>
    public interface IPackable {
        /// <summary>
        /// Pack this instance into a writer
        /// </summary>
        /// <param name="writer">The writer to write to</param>
        /// <returns>true if successful</returns>
        public bool Pack(DatBinWriter writer);
    }
}