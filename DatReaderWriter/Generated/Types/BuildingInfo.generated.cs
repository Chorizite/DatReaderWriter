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
    /// <summary>
    /// Building information.
    /// </summary>
    public class BuildingInfo : IDatObjType {
        /// <summary>
        /// Either a SetupModel or GfxObj id.
        /// </summary>
        public uint ModelId;

        /// <summary>
        /// The position information
        /// </summary>
        public Frame Frame;

        public uint NumLeaves;

        public List<BuildingPortal> Portals = [];

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            ModelId = reader.ReadUInt32();
            Frame = reader.ReadItem<Frame>();
            NumLeaves = reader.ReadUInt32();
            var _numPortals = reader.ReadUInt32();
            for (var i=0; i < _numPortals; i++) {
                Portals.Add(reader.ReadItem<BuildingPortal>());
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteUInt32(ModelId);
            writer.WriteItem<Frame>(Frame);
            writer.WriteUInt32(NumLeaves);
            writer.WriteUInt32((uint)Portals.Count());
            foreach (var item in Portals) {
                writer.WriteItem<BuildingPortal>(item);
            }
            return true;
        }

    }

}
