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
    /// <summary>
    /// Formula that dictates how to calculate a skill `(Attribute1 + Attribute2?) / Divisor`.
    /// </summary>
    public partial class SkillFormula : IDatObjType {
        public uint Unknown;

        /// <summary>
        /// True if this formula uses a second attribute for the calculation
        /// </summary>
        public bool HasSecondAttribute;

        /// <summary>
        /// If this is false, dont use this formula just use the base value of the skill
        /// </summary>
        public bool UseFormula;

        /// <summary>
        /// The divisor used in the formula.
        /// </summary>
        public int Divisor;

        /// <summary>
        /// The first attribute to use
        /// </summary>
        public AttributeId Attribute1;

        /// <summary>
        /// The second attribute to use. Only valid if HasSecondAttribute is true
        /// </summary>
        public AttributeId Attribute2;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Unknown = reader.ReadUInt32();
            HasSecondAttribute = reader.ReadBool(4);
            UseFormula = reader.ReadBool(4);
            Divisor = reader.ReadInt32();
            Attribute1 = (AttributeId)reader.ReadUInt32();
            Attribute2 = (AttributeId)reader.ReadUInt32();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(Unknown);
            writer.WriteBool(HasSecondAttribute, 4);
            writer.WriteBool(UseFormula, 4);
            writer.WriteInt32(Divisor);
            writer.WriteUInt32((uint)Attribute1);
            writer.WriteUInt32((uint)Attribute2);
            return true;
        }

    }

}
