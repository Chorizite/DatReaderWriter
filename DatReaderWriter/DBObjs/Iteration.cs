using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Enums;
using DatReaderWriter.Types;
using DatReaderWriter.Lib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.DBObjs {
    /// <summary>
    /// Database iteration (versioning) data
    /// </summary>
    [DBObjType(typeof(Iteration), DatFileType.Undefined, DBObjType.Iteration, DBObjHeaderFlags.None, 0xFFFF0001, 0xFFFF0001, 0x00000000)]
    public class Iteration : DBObj {
        /// <inheritdoc/>
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.None;

        /// <inheritdoc/>
        public override DBObjType DBObjType => DBObjType.Iteration;

        /// <inheritdoc/>
        public override uint Id { get; set; } = 0xFFFF0001;

        /// <summary>
        /// The current iteration
        /// </summary>
        public int CurrentIteration { get; set; }


        /// <summary>
        /// Iteration data
        /// </summary>
        public Dictionary<int, int> Iterations = [];

        /// <inheritdoc/>
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            CurrentIteration = reader.ReadInt32();
            var _numIterations = CurrentIteration;

            while (_numIterations > 0) {
                var consecutiveIterations = reader.ReadInt32();
                var startingIteration = reader.ReadInt32();
                Iterations.Add(startingIteration, consecutiveIterations);
                _numIterations += consecutiveIterations;
            }

            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);

            // i dunno about this...
            writer.WriteInt32(CurrentIteration);
            writer.WriteInt32(-CurrentIteration);
            writer.WriteInt32(1);

            return true;
        }
    }
}
