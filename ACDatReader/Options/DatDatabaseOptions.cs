namespace ACDatReader.Options {
    /// <summary>
    /// <see cref="ACDatReader.DatDatabaseReader"/> Options
    /// </summary>
    public class DatDatabaseOptions {
        /// <summary>
        /// The absolute file path to the dat file being read.
        /// </summary>
        public string FilePath { get; set; } = @"C:\Turbine\Asheron's Call\client_portal.dat";

        /// <summary>
        /// The file location index caching strategy to use.
        /// </summary>
        public IndexCachingStrategy IndexCachingStrategy { get; set; } = IndexCachingStrategy.OnDemand;
    }
}