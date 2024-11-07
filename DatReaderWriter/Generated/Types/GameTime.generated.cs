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
    public partial class GameTime : IDatObjType {
        public double ZeroTimeOfYear;

        public uint ZeroYear;

        public float DayLength;

        public uint DaysPerYear;

        public string YearSpec;

        public List<TimeOfDay> TimesOfDay = [];

        public List<string> DaysOfWeek = [];

        public List<Season> Seasons = [];

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            ZeroTimeOfYear = reader.ReadDouble();
            ZeroYear = reader.ReadUInt32();
            DayLength = reader.ReadSingle();
            DaysPerYear = reader.ReadUInt32();
            YearSpec = reader.ReadString16L();
            reader.Align(4);
            var _numTimesOfDay = reader.ReadUInt32();
            for (var i=0; i < _numTimesOfDay; i++) {
                TimesOfDay.Add(reader.ReadItem<TimeOfDay>());
            }
            var _numDaysOfWeek = reader.ReadUInt32();
            for (var i=0; i < _numDaysOfWeek; i++) {
                DaysOfWeek.Add(reader.ReadString16L());
            }
            var _numSeasons = reader.ReadUInt32();
            for (var i=0; i < _numSeasons; i++) {
                Seasons.Add(reader.ReadItem<Season>());
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteDouble(ZeroTimeOfYear);
            writer.WriteUInt32(ZeroYear);
            writer.WriteSingle(DayLength);
            writer.WriteUInt32(DaysPerYear);
            writer.WriteString16L(YearSpec);
            writer.Align(4);
            writer.WriteUInt32((uint)TimesOfDay.Count());
            foreach (var item in TimesOfDay) {
                writer.WriteItem<TimeOfDay>(item);
            }
            writer.WriteUInt32((uint)DaysOfWeek.Count());
            foreach (var item in DaysOfWeek) {
                writer.WriteString16L(item);
            }
            writer.WriteUInt32((uint)Seasons.Count());
            foreach (var item in Seasons) {
                writer.WriteItem<Season>(item);
            }
            return true;
        }

    }

}
