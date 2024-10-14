namespace ACClientLib.DatReaderWriter.IO {
    /// <summary>
    /// A file stored in the dat
    /// </summary>
    public interface IDatFileType : IUnpackable, IPackable {
        /// <summary>
        /// The id of this texture
        /// </summary>
        public uint Id { get; set; }
    }
}