using System;

namespace ACClientLib.DatReaderWriter.IO {
    /// <summary>
    /// Can be packed to a dat file
    /// </summary>
    public interface IPackable {
        /// <summary>
        /// Get the total size in bytes this will use when written.
        /// </summary>
        /// <returns></returns>
        int GetSize();

        /// <summary>
        /// Pack this instance into a writer
        /// </summary>
        /// <param name="writer">The writer to write to</param>
        /// <returns>true if successful</returns>
        public bool Pack(DatFileWriter writer);
    }
}