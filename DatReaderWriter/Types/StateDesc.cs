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

        public Dictionary<uint, BaseProperty> Properties = [];

        public List<MediaDesc> Media = [];

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            StateId = reader.ReadUInt32();
            PassToChildren = reader.ReadBool(1);
            IncorporationFlags = (IncorporationFlags)reader.ReadUInt32();
            var _numBuckets = reader.ReadByte();
            var _numProperties = reader.ReadCompressedUInt();

            for (var i = 0; i < _numProperties; i++) {
                var key = reader.ReadUInt32();
                Properties.Add(key, BaseProperty.UnpackGeneric(reader));
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
                writer.WriteUInt32(item.Key);
                writer.WriteItem<BaseProperty>(item.Value);
            }
            writer.WriteCompressedUInt((uint)Media.Count());
            foreach (var item in Media) {
                writer.WriteItem<MediaDesc>(item);
            }
            return true;
        }

    }

}
