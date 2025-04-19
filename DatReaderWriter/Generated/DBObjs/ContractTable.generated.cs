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
using DatReaderWriter.Types;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.DBObjs {
    /// <summary>
    /// DB_TYPE_CONTRACT_TABLE_0 in the client.
    /// </summary>
    [DBObjType(typeof(ContractTable), DatFileType.Portal, DBObjType.ContractTable, DBObjHeaderFlags.HasId, 0x0E00001D, 0x0E00001D, 0x00000000)]
    public partial class ContractTable : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.ContractTable;

        public Dictionary<uint, Contract> Contracts = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            var _numContracts = reader.ReadUInt16();
            var _numBuckets = reader.ReadUInt16();
            for (var i=0; i < _numContracts; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadItem<Contract>();
                Contracts.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt16((ushort)Contracts.Count());
            writer.WriteUInt16(4096);
            foreach (var kv in Contracts) {
                writer.WriteUInt32(kv.Key);
                writer.WriteItem<Contract>(kv.Value);
            }
            return true;
        }

    }

}
