using ACClientLib.DatReaderWriter.Attributes;
using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.IO;
using ACClientLib.DatReaderWriter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACClientLib.DatReaderWriter.DBObjs {
    [DBObjType(DatFileType.All, DBObjHeaderFlags.None, 0xFFFF0001, 0xFFFF0001)]
    public class Iteration : DBObj {
        /// <inheritdoc/>
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.None;

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
        public override bool Unpack(DatFileReader reader) {
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
        public override bool Pack(DatFileWriter writer) {
            base.Pack(writer);

            // i dunno about this...
            writer.WriteInt32(CurrentIteration);
            writer.WriteInt32(-CurrentIteration);
            writer.WriteInt32(1);

            return true;
        }
    }
}
