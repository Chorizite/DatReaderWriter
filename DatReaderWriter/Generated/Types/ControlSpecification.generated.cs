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
    /// Key/Mouse binding information
    /// </summary>
    public partial class ControlSpecification : IDatObjType {
        /// <summary>
        /// The index of the device to use
        /// </summary>
        public byte DeviceIndex;

        public byte SubControl ;

        /// <summary>
        /// Keyboard Device / Mouse Device Enumeration (DIK_ / DIMOFS_)
        /// </summary>
        public ushort Key;

        /// <summary>
        /// Flags that determine what modifier/meta keys are defined for this binding. These correspond to KeyMap.MetaKeys.
        /// </summary>
        public uint Modifiers;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            DeviceIndex = reader.ReadByte();
            SubControl  = reader.ReadByte();
            Key = reader.ReadUInt16();
            Modifiers = reader.ReadUInt32();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteByte(DeviceIndex);
            writer.WriteByte(SubControl );
            writer.WriteUInt16(Key);
            writer.WriteUInt32(Modifiers);
            return true;
        }

    }

}
