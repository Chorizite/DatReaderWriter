using DatReaderWriter.DBObjs;
using DatReaderWriter.Enums;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.IO.BlockAllocators;
using DatReaderWriter.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading;
using System.Threading.Tasks;

namespace DatReaderWriter {
    /// <summary>
    /// Used to read / write from the cell database. This is just a specialized <see cref="DatDatabase"/>
    /// that has collections exposed for the contained DBObjs.
    /// </summary>
    public partial class CellDatabase : DatDatabase {
        /// <summary>
        /// Get a <see cref="LandBlock"/> entirely from the cell.dat
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LandBlock? GetLandBlock(uint id) => Get<LandBlock>(id);

        /// <summary>
        /// Get a <see cref="LandBlock"/> entirely from the cell.dat asynchronously
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
#if (NET8_0_OR_GREATER)
        public ValueTask<LandBlock?> GetLandBlockAsync(uint id, CancellationToken ct = default) =>
            GetAsync<LandBlock>(id, ct);
#else
        public Task<LandBlock?> GetLandBlockAsync(uint id, CancellationToken ct =
 default) => GetAsync<LandBlock>(id, ct);
#endif
        /// <summary>
        /// Open a <see cref="CellDatabase"/>.
        /// </summary>
        /// <param name="options">An action that lets you configure the options</param>
        /// <param name="blockAllocator"></param>
        public CellDatabase(Action<DatDatabaseOptions> options, IDatBlockAllocator? blockAllocator = null) : base(
            options, blockAllocator) {
            if (BlockAllocator.HasHeaderData && Header.Type != DatFileType.Cell) {
                throw new ArgumentException(
                    $"Tried to open {Options.FilePath} as a cell database, but it's type is {Header.Type}");
            }
        }

        /// <summary>
        /// Open a <see cref="CellDatabase"/>.
        /// </summary>
        /// <param name="datFilePath">The path to the cell dat file</param>
        /// <param name="accessType"></param>
        public CellDatabase(string datFilePath, DatAccessType accessType = DatAccessType.Read) : this(options => {
            options.FilePath = datFilePath;
            options.AccessType = accessType;
        }) {
        }
    }
}
