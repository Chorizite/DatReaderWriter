namespace ACDatReader.Options {
    /// <summary>
    /// <see cref="ACDatReader.DatDatabase"/> Options
    /// </summary>
    public class DatDatabaseOptions {
        /// <summary>
        /// The absolute file path to the dat file being read.
        /// </summary>
        public string FilePath { get; set; } = @"C:\Turbine\Asheron's Call\client_portal.dat";

        /// <summary>
        /// Wether to preload all file entries into a cache when instantiating the dat reader.
        /// This makes all future file lookups faster, with a little upfront cost in memory and cpu,
        /// but probably not enough to care so should mostly be left as true.
        /// </summary>
        public bool PreloadFileEntries { get; set; } = true;

        /// <summary>
        /// Wether to cache dat directories as they are read. You probably don't need this unless you
        /// are wanting to dig *deep* into the dats. Like this is useful for rewriting dats, but not
        /// so much when reading them.
        /// </summary>
        public bool CacheDirectories { get; set; } = false;
    }
}