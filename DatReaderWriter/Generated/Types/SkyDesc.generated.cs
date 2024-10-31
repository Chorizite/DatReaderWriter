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
    public class SkyDesc : IDatObjType {
        public double TickSize;

        public double LightTickSize;

        public List<DayGroup> DayGroups = [];

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            TickSize = reader.ReadDouble();
            LightTickSize = reader.ReadDouble();
            var _numDayGroups = reader.ReadUInt32();
            for (var i=0; i < _numDayGroups; i++) {
                DayGroups.Add(reader.ReadItem<DayGroup>());
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteDouble(TickSize);
            writer.WriteDouble(LightTickSize);
            writer.WriteUInt32((uint)DayGroups.Count());
            foreach (var item in DayGroups) {
                writer.WriteItem<DayGroup>(item);
            }
            return true;
        }

    }

}
