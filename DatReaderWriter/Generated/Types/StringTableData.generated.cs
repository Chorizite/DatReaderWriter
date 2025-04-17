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
    public partial class StringTableData : IDatObjType {
        public List<string> VarNames = [];

        public List<string> Vars = [];

        public List<string> Strings = [];

        public List<uint> Comments = [];

        public byte Unknown;

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            var _numVarNames = reader.ReadUInt16();
            for (var i=0; i < _numVarNames; i++) {
                VarNames.Add(reader.ReadUShortString());
            }
            var _numVars = reader.ReadUInt16();
            for (var i=0; i < _numVars; i++) {
                Vars.Add(reader.ReadUShortString());
            }
            var _numStrings = reader.ReadUInt32();
            for (var i=0; i < _numStrings; i++) {
                Strings.Add(reader.ReadUShortString());
            }
            var _numComments = reader.ReadUInt32();
            for (var i=0; i < _numComments; i++) {
                Comments.Add(reader.ReadUInt32());
            }
            Unknown = reader.ReadByte();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt16((ushort)VarNames.Count());
            foreach (var item in VarNames) {
                writer.WriteUShortString(item);
            }
            writer.WriteUInt16((ushort)Vars.Count());
            foreach (var item in Vars) {
                writer.WriteUShortString(item);
            }
            writer.WriteUInt32((uint)Strings.Count());
            foreach (var item in Strings) {
                writer.WriteUShortString(item);
            }
            writer.WriteUInt32((uint)Comments.Count());
            foreach (var item in Comments) {
                writer.WriteUInt32(item);
            }
            writer.WriteByte(Unknown);
            return true;
        }

    }

}
