namespace DatReaderWriter.Options {
    /// <summary>
    /// The strategy for caching file indexes
    /// </summary>
    public enum IndexCachingStrategy {
        /// <summary>
        /// Never cache any entries, always do a full seek.
        /// </summary>
        Never,

        /// <summary>
        /// Cache file entries as the tree is walked during file seeking. This is probably
        /// the best option by default assuming you dont need to get a list of files.
        /// </summary>
        OnDemand,

        /// <summary>
        /// Cache all file entries upfront upon opening the database. If you're going to be
        /// listing out all file entries, like say in a dat viewer app, this is probably what
        /// you want.
        /// </summary>
        Upfront,
    }
}
