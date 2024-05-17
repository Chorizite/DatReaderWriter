using System.IO;

namespace ACClientLIb.DatReaderWriter.Options {
    /// <summary>
    /// Options for <see cref="DatManager"/>
    /// </summary>
    public class DatManagerOptions {
        private string? _portalPath;
        private string? _cellPath;
        private string? _languagePath;
        private string? _highResPath;

        private IndexCachingStrategy? _portalIndexCachingStrategy;
        private IndexCachingStrategy? _cellIndexCachingStrategy;
        private IndexCachingStrategy? _languageIndexCachingStrategy;
        private IndexCachingStrategy? _highresIndexCachingStrategy;

        /// <summary>
        /// The directory to read dats from.
        /// </summary>
        public string DatDirectory { get; set; } = @"C:\Turbine\Asheron's Call\";

        /// <summary>
        /// The name of the portal dat file. will be joined with <see cref="DatDirectory"/>
        /// </summary>
        public string PortalDatFileName { get; set; } = @"client_portal.dat";

        /// <summary>
        /// The name of the cell dat file. will be joined with <see cref="DatDirectory"/>
        /// </summary>
        public string CellDatFileName { get; set; } = @"client_cell_1.dat";

        /// <summary>
        /// The name of the language dat file. will be joined with <see cref="DatDirectory"/>
        /// </summary>
        public string LanguageDatFileName { get; set; } = @"client_local_English.dat";

        /// <summary>
        /// The name of the highres dat file. will be joined with <see cref="DatDirectory"/>
        /// </summary>
        public string HighResDatFileName { get; set; } = @"client_highres.dat";

        /// <inheritdoc cref="DatDatabaseOptions.IndexCachingStrategy"/>
        public IndexCachingStrategy IndexCachingStrategy { get; set; } = IndexCachingStrategy.OnDemand;

        /// <inheritdoc cref="DatDatabaseOptions.IndexCachingStrategy"/>
        /// <remarks>
        /// This overrides the file location index caching strategy for the portal dat file loaded
        /// by the manager. If not set, defaults to using <see cref="IndexCachingStrategy"/>
        /// </remarks>
        public IndexCachingStrategy? PortalIndexCachingStrategy {
            get => _portalIndexCachingStrategy ?? IndexCachingStrategy;
            set => _portalIndexCachingStrategy = value;
        }

        /// <inheritdoc cref="DatDatabaseOptions.IndexCachingStrategy"/>
        /// <remarks>
        /// This overrides the file location index caching strategy for the cell dat file loaded
        /// by the manager. If not set, defaults to using <see cref="IndexCachingStrategy"/>
        /// </remarks>
        public IndexCachingStrategy? CellIndexCachingStrategy {
            get => _cellIndexCachingStrategy ?? IndexCachingStrategy;
            set => _cellIndexCachingStrategy = value;
        }

        /// <inheritdoc cref="DatDatabaseOptions.IndexCachingStrategy"/>
        /// <remarks>
        /// This overrides the file location index caching strategy for the language dat file loaded
        /// by the manager. If not set, defaults to using <see cref="IndexCachingStrategy"/>
        /// </remarks>
        public IndexCachingStrategy? LanguageIndexCachingStratgey {
            get => _languageIndexCachingStrategy ?? IndexCachingStrategy;
            set => _languageIndexCachingStrategy = value;
        }

        /// <inheritdoc cref="DatDatabaseOptions.IndexCachingStrategy"/>
        /// <remarks>
        /// This overrides the file location index caching strategy for the highres dat file loaded
        /// by the manager. If not set, defaults to using <see cref="IndexCachingStrategy"/>
        /// </remarks>
        public IndexCachingStrategy? HighResIndexCachingStrategy {
            get => _highresIndexCachingStrategy ?? IndexCachingStrategy;
            set => _highresIndexCachingStrategy = value;
        }

        /// <summary>
        /// The absolute path to the portal dat file. By default this uses <see cref="DatDirectory"/> and
        /// <see cref="PortalDatFileName"/>, but if set to a non null value you can override the path
        /// to this specific dat.
        /// </summary>
        public string PortalDatPath {
            get => _portalPath ?? Path.Combine(DatDirectory, PortalDatFileName);
            set => _portalPath = value;
        }

        /// <summary>
        /// The absolute path to the cell dat file. By default this uses <see cref="DatDirectory"/> and
        /// <see cref="CellDatFileName"/>, but if set to a non null value you can override the path
        /// to this specific dat.
        /// </summary>
        public string CellDatPath {
            get => _cellPath ?? Path.Combine(DatDirectory, CellDatFileName);
            set => _cellPath = value;
        }

        /// <summary>
        /// The absolute path to the language dat file. By default this uses <see cref="DatDirectory"/> and
        /// <see cref="LanguageDatFileName"/>, but if set to a non null value you can override the path
        /// to this specific dat.
        /// </summary>
        public string LanguageDatPath {
            get => _languagePath ?? Path.Combine(DatDirectory, LanguageDatFileName);
            set => _languagePath = value;
        }

        /// <summary>
        /// The absolute path to the highres dat file. By default this uses <see cref="DatDirectory"/> and
        /// <see cref="HighResDatFileName"/>, but if set to a non null value you can override the path
        /// to this specific dat.
        /// </summary>
        public string HighResDatPath {
            get => _highResPath ?? Path.Combine(DatDirectory, HighResDatFileName);
            set => _highResPath = value;
        }
    }
}
