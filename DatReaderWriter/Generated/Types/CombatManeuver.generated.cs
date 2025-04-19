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
    public partial class CombatManeuver : IDatObjType {
        public MotionStance Style;

        public AttackHeight AttackHeight;

        public AttackType AttackType;

        public uint MinSkillLevel;

        public MotionCommand Motion;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Style = (MotionStance)reader.ReadUInt32();
            AttackHeight = (AttackHeight)reader.ReadInt32();
            AttackType = (AttackType)reader.ReadInt32();
            MinSkillLevel = reader.ReadUInt32();
            Motion = (MotionCommand)reader.ReadUInt32();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32((uint)Style);
            writer.WriteInt32((int)AttackHeight);
            writer.WriteInt32((int)AttackType);
            writer.WriteUInt32(MinSkillLevel);
            writer.WriteUInt32((uint)Motion);
            return true;
        }

    }

}
