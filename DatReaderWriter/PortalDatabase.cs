using DatReaderWriter.DBObjs;
using DatReaderWriter.Enums;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.IO.BlockAllocators;
using DatReaderWriter.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter {
    /// <summary>
    /// Used to read / write from the portal database. This is just a specialized <see cref="DatDatabase"/>
    /// that has collections exposed for the contained DBObjs.
    /// </summary>
    public partial class PortalDatabase : DatDatabase {
        /// <summary>
        /// The master property in portal database. This is used for property lookups. Although technically
        /// there can be multiple master properties, in practice there is only ever one. This will always
        /// return the first one. This file will always be cached.
        /// </summary>
        public MasterProperty? MasterProperty => GetCached<MasterProperty>(0x39000001u);

        /// <summary>
        /// The Region in portal database. Although technically
        /// there can be multiple regions, in practice there is only ever one (for now?). This will always
        /// return the first one. This file will always be cached.
        /// </summary>
        public Region? Region => GetCached<Region>(0x13000000u);

        /// <summary>
        /// Open a <see cref="PortalDatabase"/> (client_portal.dat)
        /// </summary>
        /// <param name="options">An action that lets you configure the options</param>
        /// <param name="blockAllocator"></param>
        public PortalDatabase(Action<DatDatabaseOptions> options, IDatBlockAllocator? blockAllocator = null) : base(options, blockAllocator) {
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
        /// <param name="accessType"></param>
        public PortalDatabase(string datFilePath, DatAccessType accessType = DatAccessType.Read) : this(options => {
            options.FilePath = datFilePath;
            options.AccessType = accessType;
        }) {

        }
    }
}
