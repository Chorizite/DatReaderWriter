using DatReaderWriter.Lib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Types {
    /// <summary>
    /// Represents a portal reference in a Drawing BSP tree.
    /// </summary>
    public struct PortalRef : IDatObjType {
        public ushort PolyId { get; set; }
        public ushort PortalIndex { get; set; }

        public bool Unpack(DatBinReader reader) {
            PolyId = reader.ReadUInt16();
            PortalIndex = reader.ReadUInt16();
            return true;
        }

        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt16(PolyId);
            writer.WriteUInt16(PortalIndex);
            return true;
        }
    }
}
