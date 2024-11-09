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
    public partial class PhysicsScriptData : IDatObjType {
        public double StartTime;

        public AnimationHook Hook;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            StartTime = reader.ReadDouble();
            var _peekedValue = (AnimationHookType)reader.ReadUInt32();
            reader.Skip(-sizeof(AnimationHookType));
            Hook = AnimationHook.Unpack(reader, _peekedValue);
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteDouble(StartTime);
            writer.WriteItem<AnimationHook>(Hook);
            return true;
        }

    }

}
