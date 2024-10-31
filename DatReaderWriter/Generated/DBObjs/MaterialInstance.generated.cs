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
using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.IO;
using ACClientLib.DatReaderWriter.Types;
using ACClientLib.DatReaderWriter.Attributes;

namespace ACClientLib.DatReaderWriter.DBObjs {
    /// <summary>
    /// DB_TYPE_MATERIALINSTANCE in the client.
    /// </summary>
    [DBObjType(typeof(MaterialInstance), DatFileType.Portal, DBObjType.MaterialInstance, DBObjHeaderFlags.HasId, 0x18000000, 0x18FFFFFF, 0x00000000)]
    public class MaterialInstance : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.MaterialInstance;

        public uint MaterialId;

        public uint MaterialType;

        public List<uint> ModifierRefs = [];

        public bool AllowStencilShadows;

        public bool WantDiscardGeometry;

        /// <inheritdoc />
        public override bool Unpack(DatFileReader reader) {
            base.Unpack(reader);
            MaterialId = reader.ReadUInt32();
            MaterialType = reader.ReadUInt32();
            var _numModifierRefs = reader.ReadUInt32();
            for (var i=0; i < _numModifierRefs; i++) {
                ModifierRefs.Add(reader.ReadUInt32());
            }
            AllowStencilShadows = reader.ReadBool(1);
            WantDiscardGeometry = reader.ReadBool(1);
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatFileWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32(MaterialId);
            writer.WriteUInt32(MaterialType);
            writer.WriteUInt32((uint)ModifierRefs.Count());
            foreach (var item in ModifierRefs) {
                writer.WriteUInt32(item);
            }
            writer.WriteBool(AllowStencilShadows, 1);
            writer.WriteBool(WantDiscardGeometry, 1);
            return true;
        }

    }

}
