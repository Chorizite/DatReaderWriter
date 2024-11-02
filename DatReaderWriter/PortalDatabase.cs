using ACClientLib.DatReaderWriter.DBObjs;
using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.Lib;
using ACClientLib.DatReaderWriter.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACClientLib.DatReaderWriter {
    /// <summary>
    /// Used to read / write from the portal database. This is just a specialized <see cref="DatDatabaseReader"/>
    /// that has collections exposed for the contained DBObjs.
    /// </summary>
    public partial class PortalDatabase : DatDatabaseReader {
        /// <summary>
        /// Open a <see cref="PortalDatabase"/> (client_portal.dat)
        /// </summary>
        /// <param name="options">An action that lets you configure the options</param>
        public PortalDatabase(Action<DatDatabaseOptions> options) : base(options) {
            if (BlockAllocator.HasHeaderData && Header.Type != DatFileType.Portal) {
                BlockAllocator.Dispose();
                Tree.Dispose();
                throw new ArgumentException($"Tried to open {Options.FilePath} as a portal database, but it's type is {Header.Type}");
            }
        }

        /// <summary>
        /// Open a <see cref="CellDatabase"/> for reading.
        /// </summary>
        /// <param name="datFilePath">The path to the cell dat file</param>
        public PortalDatabase(string datFilePath) : this(options => options.FilePath = datFilePath) {

        }
    }
}
