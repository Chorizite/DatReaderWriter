namespace ACClientLib.DatReaderWriter {

    /// <summary>
    /// Dat database types. HighRes dat uses the <see cref="Portal"/> type
    /// </summary>
    public enum DatDatabaseType {
        /// <summary>
        ///  Portal / HighRes
        /// </summary>
        Portal = 1,

        /// <summary>
        /// Cell
        /// </summary>
        Cell = 2,

        /// <summary>
        /// Language
        /// </summary>
        Language = 3,
    }
}