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

namespace ACClientLib.DatReaderWriter.Types {
    public class DefaultScriptPartHook : AnimationHook {
        /// <inheritdoc />
        public override AnimationHookType HookType => AnimationHookType.DefaultScriptPart;

        public uint PartIndex;

        /// <inheritdoc />
        public override bool Unpack(DatFileReader reader) {
            base.Unpack(reader);
            PartIndex = reader.ReadUInt32();
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatFileWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32(PartIndex);
            return true;
        }

    }

}
