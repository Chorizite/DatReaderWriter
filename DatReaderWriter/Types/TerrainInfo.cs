using DatReaderWriter.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Types {

    /// <summary>
    /// Represents terrain information with bit-packed fields: Road (2 bits), Type (5 bits), Scenery (5 bits).
    /// Total: 12 bits used, stored in a ushort.
    /// </summary>
    public struct TerrainInfo {
        private ushort _value;

        public TerrainInfo(ushort value) {
            _value = value;
        }

        // Bits 0-1: Road
        public byte Road {
            get => (byte)(_value & 0x3);
            set => _value = (ushort)((_value & ~0x3) | (value & 0x3));
        }

        // Bits 2-6: Type
        public TerrainTextureType Type {
            get => (TerrainTextureType)((_value & 0x7C) >> 2);
            set => _value = (ushort)((_value & ~0x7C) | (((byte)value & 0x1F) << 2));
        }

        // Bits 11-15: Scenery
        public byte Scenery {
            get => (byte)((_value & 0xF800) >> 11);
            set => _value = (ushort)((_value & ~0xF800) | ((value & 0x1F) << 11));
        }

        // Implicit conversion to/from ushort for serialization
        public static implicit operator ushort(TerrainInfo info) => info._value;
        public static implicit operator TerrainInfo(ushort value) => new TerrainInfo(value);
    }
}
