//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//
//                                                            //
//                          WARNING                           //
//                                                            //
//           DO NOT MAKE LOCAL CHANGES TO THIS FILE           //
//               EDIT THE .tt TEMPLATE INSTEAD                //
//                                                            //
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//


using System.Numerics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DatReaderWriter.Enums;
using DatReaderWriter.Types;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.DBObjs;

namespace DatReaderWriter {
    public partial class CellDatabase {
        private DBObjCollection<LandBlock>? _LandBlocks;
        private DBObjCollection<LandBlockInfo>? _LandBlockInfos;
        private DBObjCollection<EnvCell>? _EnvCells;
        
        /// <summary>
        /// All LandBlocks in the database.
        /// </summary>
        public DBObjCollection<LandBlock> LandBlocks => _LandBlocks ??= new DBObjCollection<LandBlock>(this);
        
        /// <summary>
        /// All LandBlockInfos in the database.
        /// </summary>
        public DBObjCollection<LandBlockInfo> LandBlockInfos => _LandBlockInfos ??= new DBObjCollection<LandBlockInfo>(this);
        
        /// <summary>
        /// All EnvCells in the database.
        /// </summary>
        public DBObjCollection<EnvCell> EnvCells => _EnvCells ??= new DBObjCollection<EnvCell>(this);
        
    }
}
