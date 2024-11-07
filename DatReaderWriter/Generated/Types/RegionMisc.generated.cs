//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//
//                                                            //
//                          WARNING                           //
//                                                            //
//           DO NOT MAKE LOCAL CHANGES TO THIS FILE           //
//               EDIT THE .tt TEMPLATE INSTEAD                //
//                                                            //
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//


using System;
using System.Numerics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DatReaderWriter.Enums;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.Types {
    public partial class RegionMisc : IDatObjType {
        public uint Version;

        public uint GameMapID;

        public uint AutotestMapId;

        public uint AutotestMapSize;

        public uint ClearCellId;

        public uint ClearMonsterId;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Version = reader.ReadUInt32();
            GameMapID = reader.ReadUInt32();
            AutotestMapId = reader.ReadUInt32();
            AutotestMapSize = reader.ReadUInt32();
            ClearCellId = reader.ReadUInt32();
            ClearMonsterId = reader.ReadUInt32();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(Version);
            writer.WriteUInt32(GameMapID);
            writer.WriteUInt32(AutotestMapId);
            writer.WriteUInt32(AutotestMapSize);
            writer.WriteUInt32(ClearCellId);
            writer.WriteUInt32(ClearMonsterId);
            return true;
        }

    }

}
