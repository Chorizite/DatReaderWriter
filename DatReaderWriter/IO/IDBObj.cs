namespace ACClientLib.DatReaderWriter.IO {
    /// <summary>
    /// A file stored in the dat
    /// </summary>
    public interface IDBObj : IUnpackable, IPackable {
        /// <summary>
        /// The id of this texture
        /// </summary>
        public uint Id { get; set; }

        public bool HasDataCategory { get; }
        public uint DataCategory { get; set; }
    }
}