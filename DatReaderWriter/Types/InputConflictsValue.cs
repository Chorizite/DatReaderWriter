using System;
using System.Numerics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DatReaderWriter.Enums;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.DBObjs;

namespace DatReaderWriter.Types {
    /// <summary>
    /// Represents a conflict value in an action map
    /// </summary>
    public partial class InputsConflictsValue : IDatObjType {
        /// <summary>
        /// InputMap
        /// </summary>
        public uint InputMap { get; set; }

        /// <summary>
        /// Conflicts
        /// </summary>
        public List<uint> ConflictingInputMaps { get; set; } = [];

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            InputMap = reader.ReadUInt32();
            var _numConflicts = reader.ReadUInt32();
            for (var i = 0; i < _numConflicts; i++) {
                ConflictingInputMaps.Add(reader.ReadUInt32());
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(InputMap);
            writer.WriteUInt32((uint)ConflictingInputMaps.Count);
            foreach (var item in ConflictingInputMaps) {
                writer.WriteUInt32(item);
            }
            return true;
        }

    }

}
