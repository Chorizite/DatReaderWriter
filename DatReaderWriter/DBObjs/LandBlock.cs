using System;
using System.Numerics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DatReaderWriter.Enums;
using DatReaderWriter.Types;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.DBObjs {
    /// <summary>
    /// DB_TYPE_LAND_BLOCK in the client.
    /// </summary>
    [DBObjType(typeof(LandBlock), DatFileType.Cell, DBObjType.LandBlock, DBObjHeaderFlags.HasId, 0x00000000, 0x00000000, 0x0000FFFF)]
    public partial class LandBlock : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;
        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.LandBlock;

        public bool HasObjects;
        public TerrainInfo[] Terrain = new TerrainInfo[81];
        public byte[] Height = new byte[81];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);

            HasObjects = reader.ReadBool();

            // Read raw ushorts and convert to TerrainInfo
            for (int i = 0; i < 81; i++) {
                ushort raw = reader.ReadUInt16();
                Terrain[i] = raw; // implicit conversion
            }

            for (int i = 0; i < 81; i++) {
                Height[i] = reader.ReadByte();
            }

            reader.Align(4);
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);

            writer.WriteBool(HasObjects);

            // Write backing ushort values
            for (int i = 0; i < 81; i++) {
                writer.WriteUInt16(Terrain[i]); // implicit conversion
            }

            for (int i = 0; i < 81; i++) {
                writer.WriteByte(Height[i]);
            }

            writer.Align(4);
            return true;
        }
    }
}