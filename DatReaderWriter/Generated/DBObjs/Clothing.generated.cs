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
    /// DB_TYPE_CLOTHING in the client.
    /// </summary>
    [DBObjType(typeof(Clothing), DatFileType.Portal, DBObjType.Clothing, DBObjHeaderFlags.HasId, 0x10000000, 0x1000FFFF, 0x00000000)]
    public partial class Clothing : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.Clothing;

        public Dictionary<uint, ClothingBaseEffect> ClothingBaseEffects = [];

        public Dictionary<uint, CloSubPalEffect> ClothingSubPalEffects = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            var _numClothingBaseEffects = reader.ReadUInt16();
            var _numClothingBaseEffectsBuckets = reader.ReadUInt16();
            for (var i=0; i < _numClothingBaseEffects; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadItem<ClothingBaseEffect>();
                ClothingBaseEffects.Add(_key, _val);
            }
            var _numClothingSubPalEffects = reader.ReadUInt16();
            var _numClothingSubPalEffectsBuckets = reader.ReadUInt16();
            for (var i=0; i < _numClothingSubPalEffects; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadItem<CloSubPalEffect>();
                ClothingSubPalEffects.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt16((ushort)ClothingBaseEffects.Count());
            writer.WriteUInt16(8);
            foreach (var kv in ClothingBaseEffects) {
                writer.WriteUInt32(kv.Key);
                writer.WriteItem<ClothingBaseEffect>(kv.Value);
            }
            writer.WriteUInt16((ushort)ClothingSubPalEffects.Count());
            writer.WriteUInt16(32);
            foreach (var kv in ClothingSubPalEffects) {
                writer.WriteUInt32(kv.Key);
                writer.WriteItem<CloSubPalEffect>(kv.Value);
            }
            return true;
        }

    }

}
