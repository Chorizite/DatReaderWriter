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
    /// <summary>
    /// DB_TYPE_CELL in the client.
    /// </summary>
    [DBObjType(DatFileType.Cell, DBObjHeaderFlags.HasId, 0x00000000, 0x00000000)]
    public class EnvCell : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        public EnvCellFlags Flags;

        /// <summary>
        /// A list of surface ids. These are not fully qualified so you need to or with 0x08000000 to get the file id.
        /// </summary>
        public List<ushort> Surfaces = [];

        /// <summary>
        /// The environment id. This is not fully qualified so you need to or with 0x0D000000 to get the file id.
        /// </summary>
        public ushort EnvironmentId;

        public ushort CellStructure;

        public Frame Position;

        /// <summary>
        /// A list of cell portals
        /// </summary>
        public List<CellPortal> CellPortals = [];

        /// <summary>
        /// A list of visible cell ids
        /// </summary>
        public List<ushort> VisibleCells = [];

        /// <summary>
        /// A list of static objects
        /// </summary>
        public List<Stab> StaticObjects = [];

        public uint RestrictionObj;

        /// <inheritdoc />
        public override bool Unpack(DatFileReader reader) {
            base.Unpack(reader);
            Flags = (EnvCellFlags)reader.ReadInt32();
            var _cellId = reader.ReadUInt32();
            var _numSurfaces = reader.ReadByte();
            var _numPortals = reader.ReadByte();
            var _numVisibleCells = reader.ReadUInt16();
            for (var i=0; i < _numSurfaces; i++) {
                Surfaces.Add(reader.ReadUInt16());
            }
            EnvironmentId = reader.ReadUInt16();
            CellStructure = reader.ReadUInt16();
            Position = reader.ReadItem<Frame>();
            for (var i=0; i < _numPortals; i++) {
                CellPortals.Add(reader.ReadItem<CellPortal>());
            }
            for (var i=0; i < _numVisibleCells; i++) {
                VisibleCells.Add(reader.ReadUInt16());
            }
            if (Flags.HasFlag(EnvCellFlags.HasStaticObjs)) {
                var _numStabs = reader.ReadUInt16();
                for (var i=0; i < _numStabs; i++) {
                    StaticObjects.Add(reader.ReadItem<Stab>());
                }
            }
            if (Flags.HasFlag(EnvCellFlags.HasRestrictionObj)) {
                RestrictionObj = reader.ReadUInt32();
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatFileWriter writer) {
            base.Pack(writer);
            writer.WriteInt32((int)Flags);
            writer.WriteUInt32(Id);
            writer.WriteByte((byte)Surfaces.Count());
            writer.WriteByte((byte)CellPortals.Count());
            writer.WriteUInt16((ushort)VisibleCells.Count());
            foreach (var item in Surfaces) {
                writer.WriteUInt16(item);
            }
            writer.WriteUInt16(EnvironmentId);
            writer.WriteUInt16(CellStructure);
            writer.WriteItem<Frame>(Position);
            foreach (var item in CellPortals) {
                writer.WriteItem<CellPortal>(item);
            }
            foreach (var item in VisibleCells) {
                writer.WriteUInt16(item);
            }
            if (Flags.HasFlag(EnvCellFlags.HasStaticObjs)) {
                writer.WriteUInt16((ushort)StaticObjects.Count());
                foreach (var item in StaticObjects) {
                    writer.WriteItem<Stab>(item);
                }
            }
            if (Flags.HasFlag(EnvCellFlags.HasRestrictionObj)) {
                writer.WriteUInt32(RestrictionObj);
            }
            return true;
        }

    }

}
