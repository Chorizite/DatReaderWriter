namespace ACDatReader.IO {
    /// <summary>
    /// Can be unpacked from a <see cref="DatParser"/>
    /// </summary>
    public interface IUnpackable {
        /// <summary>
        /// Unpack this instance from a reader
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <returns>true if succesfull</returns>
        public bool Unpack(DatParser reader);
    }
}
