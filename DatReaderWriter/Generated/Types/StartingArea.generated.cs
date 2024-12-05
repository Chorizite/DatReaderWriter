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
    /// Information about a starting area available during character creation
    /// </summary>
    public partial class StartingArea : IDatObjType {
        public string Name;

        public List<Position> Locations = [];

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Name = reader.ReadString();
            var _numLocations = reader.ReadCompressedUInt();
            for (var i=0; i < _numLocations; i++) {
                Locations.Add(reader.ReadItem<Position>());
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteString(Name);
            writer.WriteCompressedUInt((uint)Locations.Count());
            foreach (var item in Locations) {
                writer.WriteItem<Position>(item);
            }
            return true;
        }

    }

}
