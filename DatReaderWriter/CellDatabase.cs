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
    /// Used to read / write from the cell database. This is just a specialized <see cref="DatDatabaseReader"/>
    /// that has collections exposed for the contained DBObjs
    /// </summary>
    public class CellDatabase : DatDatabaseReader {
        private DBObjCollection<LandBlock>? _landBlocks;

        /// <summary>
        /// All <see cref="LandBlock"/> DBObjs.
        /// </summary>
        public DBObjCollection<LandBlock> LandBlocks => _landBlocks ??= new DBObjCollection<LandBlock>(this);

        /// <summary>
        /// Open a <see cref="CellDatabase"/> (client_cell_1.dat)
        /// </summary>
        /// <param name="options">An action that lets you configure the options</param>
        public CellDatabase(Action<DatDatabaseOptions> options) : base(options) {
            if (Header.Type != DatFileType.Cell) {
                throw new ArgumentException($"Tried to open {Options.FilePath} as a cell database, but it's type is {Header.Type}");
            }
        }

        /// <summary>
        /// Open a <see cref="CellDatabase"/> for reading.
        /// </summary>
        /// <param name="datFilePath">The path to the cell dat file</param>
        public CellDatabase(string datFilePath) : this(options => options.FilePath = datFilePath) {
        
        }
    }
}
