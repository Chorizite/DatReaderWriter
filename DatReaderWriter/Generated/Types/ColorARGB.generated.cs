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
    /// <summary>
    /// A 32 bit color with alpha/red/green/blue
    /// </summary>
    public partial class ColorARGB : IDatObjType {
        /// <summary>
        /// The blue amount from 0-255
        /// </summary>
        public byte Blue;

        /// <summary>
        /// The green amount from 0-255
        /// </summary>
        public byte Green;

        /// <summary>
        /// The red amount from 0-255
        /// </summary>
        public byte Red;

        /// <summary>
        /// The alpha amount from 0-255
        /// </summary>
        public byte Alpha;

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            Blue = reader.ReadByte();
            Green = reader.ReadByte();
            Red = reader.ReadByte();
            Alpha = reader.ReadByte();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteByte(Blue);
            writer.WriteByte(Green);
            writer.WriteByte(Red);
            writer.WriteByte(Alpha);
            return true;
        }

    }

}
