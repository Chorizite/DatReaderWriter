using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACDatReader.Options {
    /// <summary>
    /// Options for <see cref="DatManager"/>
    /// </summary>
    public class DatManagerOptions {
        private string? _portalPath;
        private string? _cellPath;
        private string? _languagePath;
        private string? _highResPath;

        private bool? _preloadPortalFileEntries;
        private bool? _preloadCelllFileEntries;
        private bool? _preloadLanguageFileEntries;
        private bool? _preloadHighResFileEntries;

        private bool? _cachePortalDirectories;
        private bool? _cacheCellDirectories;
        private bool? _cacheLanguageDirectories;
        private bool? _cacheHighResDirectories;

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

        /// <inheritdoc cref="DatDatabaseOptions.PreloadFileEntries"/>
        public bool PreloadFileEntries { get; set; } = true;

        /// <inheritdoc cref="DatDatabaseOptions.CacheDirectories"/>
        public bool CacheDirectories { get; set; } = false;

        /// <summary>
        /// Set this option to non-null to override the default value of <see cref="PreloadFileEntries"/>
        /// when loading the portal dat file.
        /// </summary>
        public bool PreloadPortalFileEntries {
            get => _preloadPortalFileEntries ?? PreloadFileEntries;
            set => _preloadPortalFileEntries = value;
        }

        /// <summary>
        /// Set this option to non-null to override the default value of <see cref="PreloadFileEntries"/>
        /// when loading the cell dat file.
        /// </summary>
        public bool PreloadCellFileEntries {
            get => _preloadCelllFileEntries ?? PreloadFileEntries;
            set => _preloadCelllFileEntries = value;
        }

        /// <summary>
        /// Set this option to non-null to override the default value of <see cref="PreloadFileEntries"/>
        /// when loading the language dat file.
        /// </summary>
        public bool PreloadLanguageFileEntries {
            get => _preloadLanguageFileEntries ?? PreloadFileEntries;
            set => _preloadLanguageFileEntries = value;
        }

        /// <summary>
        /// Set this option to non-null to override the default value of <see cref="PreloadFileEntries"/>
        /// when loading the highres dat file.
        /// </summary>
        public bool PreloadHighResFileEntries {
            get => _preloadHighResFileEntries ?? PreloadFileEntries;
            set => _preloadHighResFileEntries = value;
        }

        /// <summary>
        /// Set this option to non-null to override the default value of <see cref="CacheDirectories"/>
        /// when loading the portal dat file.
        /// </summary>
        public bool CachePortalDirectories {
            get => _cachePortalDirectories ?? CacheDirectories;
            set => _cachePortalDirectories = value;
        }

        /// <summary>
        /// Set this option to non-null to override the default value of <see cref="CacheDirectories"/>
        /// when loading the cell dat file.
        /// </summary>
        public bool CacheCellDirectories {
            get => _cacheCellDirectories ?? CacheDirectories;
            set => _cacheCellDirectories = value;
        }

        /// <summary>
        /// Set this option to non-null to override the default value of <see cref="CacheDirectories"/>
        /// when loading the language dat file.
        /// </summary>
        public bool CacheLanguageDirectories {
            get => _cacheLanguageDirectories ?? CacheDirectories;
            set => _cacheLanguageDirectories = value;
        }

        /// <summary>
        /// Set this option to non-null to override the default value of <see cref="CacheDirectories"/>
        /// when loading the highres dat file.
        /// </summary>
        public bool CacheHighResDirectories {
            get => _cacheHighResDirectories ?? CacheDirectories;
            set => _cacheHighResDirectories = value;
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
