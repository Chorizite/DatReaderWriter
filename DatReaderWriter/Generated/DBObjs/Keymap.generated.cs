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
using DatReaderWriter.Enums;
using DatReaderWriter.Types;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.DBObjs {
    /// <summary>
    /// DB_TYPE_KEYMAP in the client.
    /// </summary>
    [DBObjType(typeof(Keymap), DatFileType.Portal, DBObjType.Keymap, DBObjHeaderFlags.HasId, 0x14000000, 0x1400FFFF, 0x00000000)]
    public partial class Keymap : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.Keymap;

        public string Name;

        public Guid GuidMap;

        public List<DeviceKeyMapEntry> DeviceKeyMapEntries = [];

        public List<ControlSpecification> MetaKeys = [];

        public List<UserBindingValue> UserBindings = [];

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            Name = reader.ReadString();
            GuidMap = reader.ReadGuid();
            var _numDeviceKeyMapEntries = reader.ReadUInt32();
            for (var i=0; i < _numDeviceKeyMapEntries; i++) {
                DeviceKeyMapEntries.Add(reader.ReadItem<DeviceKeyMapEntry>());
            }
            var _numMetaKeys = reader.ReadUInt32();
            for (var i=0; i < _numMetaKeys; i++) {
                MetaKeys.Add(reader.ReadItem<ControlSpecification>());
            }
            var _numUserBindings = reader.ReadUInt32();
            for (var i=0; i < _numUserBindings; i++) {
                UserBindings.Add(reader.ReadItem<UserBindingValue>());
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteString(Name);
            writer.WriteGuid(GuidMap);
            writer.WriteUInt32((uint)DeviceKeyMapEntries.Count());
            foreach (var item in DeviceKeyMapEntries) {
                writer.WriteItem<DeviceKeyMapEntry>(item);
            }
            writer.WriteUInt32((uint)MetaKeys.Count());
            foreach (var item in MetaKeys) {
                writer.WriteItem<ControlSpecification>(item);
            }
            writer.WriteUInt32((uint)UserBindings.Count());
            foreach (var item in UserBindings) {
                writer.WriteItem<UserBindingValue>(item);
            }
            return true;
        }

    }

}
