
using System;
using ACDatReader.IO.BlockAllocators;
using ACDatReader.Options;

namespace ACDatReader {

    /// <summary>
    /// A configurable dat manager implementation that loads up all of the dat files
    /// </summary>
    public class DatManager : IDisposable {
        /// <summary>
        /// Options
        /// </summary>
        public DatManagerOptions Options { get; } = new DatManagerOptions();

        /// <summary>
        /// Portal dat database
        /// </summary>
        public DatDatabaseReader Portal { get; }

        /// <summary>
        /// Cell dat database
        /// </summary>
        public DatDatabaseReader Cell { get; }

        /// <summary>
        /// Language dat database
        /// </summary>
        public DatDatabaseReader Language { get; }

        /// <summary>
        /// HighRes dat database
        /// </summary>
        public DatDatabaseReader HighRes { get; }

        /// <summary>
        /// Create a new datmanager
        /// </summary>
        /// <param name="options">Options configuration action</param>
        /// <param name="blockAllocator">An instance of a block allocator to use</param>
        public DatManager(Action<DatManagerOptions>? options = null, IDatBlockAllocator? blockAllocator = null) {
            options?.Invoke(Options);

            Portal = new DatDatabaseReader(portalOptions => {
                portalOptions.FilePath = Options.PortalDatPath;
                portalOptions.IndexCachingStrategy = Options.IndexCachingStrategy;
            }, blockAllocator);

            Cell = new DatDatabaseReader(cellOptions => {
                cellOptions.FilePath = Options.PortalDatPath;
                cellOptions.IndexCachingStrategy = Options.IndexCachingStrategy;
            }, blockAllocator);

            Language = new DatDatabaseReader(languageOptions => {
                languageOptions.FilePath = Options.PortalDatPath;
                languageOptions.IndexCachingStrategy = Options.IndexCachingStrategy;
            }, blockAllocator);

            HighRes = new DatDatabaseReader(highResOptions => {
                highResOptions.FilePath = Options.HighResDatPath;
                highResOptions.IndexCachingStrategy = Options.IndexCachingStrategy;
            }, blockAllocator);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Dispose
        /// </summary>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                Portal?.Dispose();
                Cell?.Dispose();
                Language?.Dispose();
            }
        }
    }
}
