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
    public partial class GfxObjInfo : IDatObjType {
        /// <summary>
        /// Id of a GfxObj
        /// </summary>
        public uint Id;

        public uint DegradeMode;

        public float MinDist;

        public float IdealDist;

        public float MaxDist;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Id = reader.ReadUInt32();
            DegradeMode = reader.ReadUInt32();
            MinDist = reader.ReadSingle();
            IdealDist = reader.ReadSingle();
            MaxDist = reader.ReadSingle();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(Id);
            writer.WriteUInt32(DegradeMode);
            writer.WriteSingle(MinDist);
            writer.WriteSingle(IdealDist);
            writer.WriteSingle(MaxDist);
            return true;
        }

    }

}
