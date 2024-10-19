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
    public class CreateParticleHook : AnimationHook {
        /// <inheritdoc />
        public override AnimationHookType HookType => AnimationHookType.CreateParticle;

        public uint EmitterInfoId;

        public uint PartIndex;

        public Frame Offset;

        public uint EmitterId;

        /// <inheritdoc />
        public override bool Unpack(DatFileReader reader) {
            base.Unpack(reader);
            EmitterInfoId = reader.ReadUInt32();
            PartIndex = reader.ReadUInt32();
            Offset = reader.ReadItem<Frame>();
            EmitterId = reader.ReadUInt32();
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatFileWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32(EmitterInfoId);
            writer.WriteUInt32(PartIndex);
            writer.WriteItem<Frame>(Offset);
            writer.WriteUInt32(EmitterId);
            return true;
        }

    }

}
