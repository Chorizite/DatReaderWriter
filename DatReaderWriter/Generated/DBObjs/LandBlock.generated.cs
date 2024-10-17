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
using ACClientLib.DatReaderWriter.Types;
using ACClientLib.DatReaderWriter.Attributes;

namespace ACClientLib.DatReaderWriter.DBObjs {
    [DBObjType(DatFileType.Cell, false, 0x00000000, 0x00000000)]
    public class LandBlock : DBObj {
        /// <inheritdoc />
        public override bool HasDataCategory => false;

        public bool HasObjects;

        public TerrainInfo[] Terrain = [];

        public byte[] Height = [];

        /// <inheritdoc />
        public override bool Unpack(DatFileReader reader) {
            base.Unpack(reader);
            HasObjects = reader.ReadBool();
            Terrain = new TerrainInfo[81];
            for (var i=0; i < 81; i++) {
                Terrain[i] = reader.ReadItem<TerrainInfo>();
            }
            Height = new byte[81];
            for (var i=0; i < 81; i++) {
                Height[i] = reader.ReadByte();
            }
            reader.Align(4);
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatFileWriter writer) {
            base.Pack(writer);
            writer.WriteBool(HasObjects);
            for (var i=0; i < Terrain.Count(); i++) {
                writer.WriteItem<TerrainInfo>(Terrain[i]);
            }
            for (var i=0; i < Height.Count(); i++) {
                writer.WriteByte(Height[i]);
            }
            writer.Align(4);
            return true;
        }

    }

}
