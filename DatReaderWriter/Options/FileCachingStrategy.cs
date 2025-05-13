namespace DatReaderWriter.Options {
    /// <summary>
    /// The strategy for caching file entry data
    /// </summary>
    public enum FileCachingStrategy {
        /// <summary>
        /// Never cache any file data, always do a full read.
        /// </summary>
        Never,

        /// <summary>
        /// Cache file data when reading files.
        /// </summary>
        OnDemand,
    }
}
