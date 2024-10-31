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
    /// DB_TYPE_MATERIALMODIFIER in the client.
    /// </summary>
    [DBObjType(typeof(MaterialModifier), DatFileType.Portal, DBObjType.MaterialModifier, DBObjHeaderFlags.HasId, 0x17000000, 0x17FFFFFF, 0x00000000)]
    public class MaterialModifier : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.MaterialModifier;

        public List<MaterialProperty> MaterialProperties = [];

        /// <inheritdoc />
        public override bool Unpack(DatFileReader reader) {
            base.Unpack(reader);
            var _numMaterialProperties = reader.ReadUInt32();
            for (var i=0; i < _numMaterialProperties; i++) {
                MaterialProperties.Add(reader.ReadItem<MaterialProperty>());
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatFileWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32((uint)MaterialProperties.Count());
            foreach (var item in MaterialProperties) {
                writer.WriteItem<MaterialProperty>(item);
            }
            return true;
        }

    }

}
