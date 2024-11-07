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
    public partial class MotionData : IDatObjType {
        public byte Bitfield;

        public MotionDataFlags Flags;

        public List<AnimData> Anims = [];

        public Vector3 Velocity;

        public Vector3 Omega;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            var _numAnims = reader.ReadByte();
            Bitfield = reader.ReadByte();
            Flags = (MotionDataFlags)reader.ReadByte();
            reader.Align(4);
            for (var i=0; i < _numAnims; i++) {
                Anims.Add(reader.ReadItem<AnimData>());
            }
            if (Flags.HasFlag(MotionDataFlags.HasVelocity)) {
                Velocity = reader.ReadVector3();
            }
            if (Flags.HasFlag(MotionDataFlags.HasOmega)) {
                Omega = reader.ReadVector3();
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteByte((byte)Anims.Count());
            writer.WriteByte(Bitfield);
            writer.WriteByte((byte)Flags);
            writer.Align(4);
            foreach (var item in Anims) {
                writer.WriteItem<AnimData>(item);
            }
            if (Flags.HasFlag(MotionDataFlags.HasVelocity)) {
                writer.WriteVector3(Velocity);
            }
            if (Flags.HasFlag(MotionDataFlags.HasOmega)) {
                writer.WriteVector3(Omega);
            }
            return true;
        }

    }

}
