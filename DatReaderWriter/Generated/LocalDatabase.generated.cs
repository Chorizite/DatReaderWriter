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
    public partial class LocalDatabase {
        private DBObjCollection<StringTable>? _StringTables;
        private DBObjCollection<LanguageInfo>? _LanguageInfos;
        
        /// <summary>
        /// All StringTables in the database.
        /// </summary>
        public DBObjCollection<StringTable> StringTables => _StringTables ??= new DBObjCollection<StringTable>(this);
        
        /// <summary>
        /// All LanguageInfos in the database.
        /// </summary>
        public DBObjCollection<LanguageInfo> LanguageInfos => _LanguageInfos ??= new DBObjCollection<LanguageInfo>(this);
        
    }
}
