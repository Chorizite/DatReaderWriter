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
    public partial class StateDesc : IDatObjType {
        public uint StateId;

        public bool PassToChildren;

        public IncorporationFlags IncorporationFlags;

        public List<BaseProperty> Properties = [];

        public List<MediaDesc> Media = [];

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            if (reader.Database?.DatCollection is null) {
                throw new Exception("reader.Database.DatCollection is null! Unable to read MasterProperties and unpack StateDesc. Use DatCollection instead of creating a standalone DatDatabase");
            }
            if (!reader.Database.DatCollection.TryReadFile<MasterProperty>(0x39000001u, out var masterProperty)) {
                throw new Exception("Unable to read MasterProperty (0x39000001)");
            }

            StateId = reader.ReadUInt32();
            PassToChildren = reader.ReadBool(1);
            IncorporationFlags = (IncorporationFlags)reader.ReadUInt32();
            var _numBuckets = reader.ReadByte();
            var _numProperties = reader.ReadCompressedUInt();

            for (var i = 0; i < _numProperties; i++) {
                var _peekedValue = reader.ReadUInt32();
                var _peekedPropType = masterProperty.Properties[_peekedValue].Type;
                Properties.Add(BaseProperty.Unpack(reader, _peekedPropType, true));
            }
            var _numMedia = reader.ReadCompressedUInt();
            for (var i = 0; i < _numMedia; i++) {
                var _peekedValue = (MediaType)reader.ReadInt32();
                reader.Skip(-sizeof(MediaType));
                Media.Add(MediaDesc.Unpack(reader, _peekedValue));
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteUInt32(StateId);
            writer.WriteBool(PassToChildren, 1);
            writer.WriteUInt32((uint)IncorporationFlags);
            writer.WriteByte(0);
            writer.WriteCompressedUInt((uint)Properties.Count());
            foreach (var item in Properties) {
                writer.WriteItem<BaseProperty>(item);
            }
            writer.WriteCompressedUInt((uint)Media.Count());
            foreach (var item in Media) {
                writer.WriteItem<MediaDesc>(item);
            }
            return true;
        }

    }

}
