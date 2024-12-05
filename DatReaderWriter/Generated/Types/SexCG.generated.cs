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
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.Types {
    public partial class SexCG : IDatObjType {
        public string Name;

        public uint Scale;

        public uint SetupId;

        public uint SoundTable;

        public uint IconId;

        public uint BasePalette;

        public uint SkinPalSet;

        public uint PhysicsTable;

        public uint MotionTable;

        public uint CombatTable;

        public ObjDesc BaseObjDesc;

        public List<uint> HairColors = [];

        public List<HairStyleCG> HairStyles = [];

        public List<uint> EyeColors = [];

        public List<EyeStripCG> EyeStrips = [];

        public List<FaceStripCG> NoseStrips = [];

        public List<FaceStripCG> MouthStrips = [];

        public List<GearCG> Headgears = [];

        public List<GearCG> Shirts = [];

        public List<GearCG> Pants = [];

        public List<GearCG> Footwear = [];

        public List<uint> ClothingColors = [];

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Name = reader.ReadString();
            Scale = reader.ReadUInt32();
            SetupId = reader.ReadUInt32();
            SoundTable = reader.ReadUInt32();
            IconId = reader.ReadUInt32();
            BasePalette = reader.ReadUInt32();
            SkinPalSet = reader.ReadUInt32();
            PhysicsTable = reader.ReadUInt32();
            MotionTable = reader.ReadUInt32();
            CombatTable = reader.ReadUInt32();
            BaseObjDesc = reader.ReadItem<ObjDesc>();
            var _numHairColors = reader.ReadCompressedUInt();
            for (var i=0; i < _numHairColors; i++) {
                HairColors.Add(reader.ReadUInt32());
            }
            var _numHairStyles = reader.ReadCompressedUInt();
            for (var i=0; i < _numHairStyles; i++) {
                HairStyles.Add(reader.ReadItem<HairStyleCG>());
            }
            var _numEyeColors = reader.ReadCompressedUInt();
            for (var i=0; i < _numEyeColors; i++) {
                EyeColors.Add(reader.ReadUInt32());
            }
            var _numEyeStrips = reader.ReadCompressedUInt();
            for (var i=0; i < _numEyeStrips; i++) {
                EyeStrips.Add(reader.ReadItem<EyeStripCG>());
            }
            var _numNoseStrips = reader.ReadCompressedUInt();
            for (var i=0; i < _numNoseStrips; i++) {
                NoseStrips.Add(reader.ReadItem<FaceStripCG>());
            }
            var _numMouthStrips = reader.ReadCompressedUInt();
            for (var i=0; i < _numMouthStrips; i++) {
                MouthStrips.Add(reader.ReadItem<FaceStripCG>());
            }
            var _numHeadgears = reader.ReadCompressedUInt();
            for (var i=0; i < _numHeadgears; i++) {
                Headgears.Add(reader.ReadItem<GearCG>());
            }
            var _numShirts = reader.ReadCompressedUInt();
            for (var i=0; i < _numShirts; i++) {
                Shirts.Add(reader.ReadItem<GearCG>());
            }
            var _numPants = reader.ReadCompressedUInt();
            for (var i=0; i < _numPants; i++) {
                Pants.Add(reader.ReadItem<GearCG>());
            }
            var _numFootwear = reader.ReadCompressedUInt();
            for (var i=0; i < _numFootwear; i++) {
                Footwear.Add(reader.ReadItem<GearCG>());
            }
            var _numClothingColors = reader.ReadCompressedUInt();
            for (var i=0; i < _numClothingColors; i++) {
                ClothingColors.Add(reader.ReadUInt32());
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteString(Name);
            writer.WriteUInt32(Scale);
            writer.WriteUInt32(SetupId);
            writer.WriteUInt32(SoundTable);
            writer.WriteUInt32(IconId);
            writer.WriteUInt32(BasePalette);
            writer.WriteUInt32(SkinPalSet);
            writer.WriteUInt32(PhysicsTable);
            writer.WriteUInt32(MotionTable);
            writer.WriteUInt32(CombatTable);
            writer.WriteItem<ObjDesc>(BaseObjDesc);
            writer.WriteCompressedUInt((uint)HairColors.Count());
            foreach (var item in HairColors) {
                writer.WriteUInt32(item);
            }
            writer.WriteCompressedUInt((uint)HairStyles.Count());
            foreach (var item in HairStyles) {
                writer.WriteItem<HairStyleCG>(item);
            }
            writer.WriteCompressedUInt((uint)EyeColors.Count());
            foreach (var item in EyeColors) {
                writer.WriteUInt32(item);
            }
            writer.WriteCompressedUInt((uint)EyeStrips.Count());
            foreach (var item in EyeStrips) {
                writer.WriteItem<EyeStripCG>(item);
            }
            writer.WriteCompressedUInt((uint)NoseStrips.Count());
            foreach (var item in NoseStrips) {
                writer.WriteItem<FaceStripCG>(item);
            }
            writer.WriteCompressedUInt((uint)MouthStrips.Count());
            foreach (var item in MouthStrips) {
                writer.WriteItem<FaceStripCG>(item);
            }
            writer.WriteCompressedUInt((uint)Headgears.Count());
            foreach (var item in Headgears) {
                writer.WriteItem<GearCG>(item);
            }
            writer.WriteCompressedUInt((uint)Shirts.Count());
            foreach (var item in Shirts) {
                writer.WriteItem<GearCG>(item);
            }
            writer.WriteCompressedUInt((uint)Pants.Count());
            foreach (var item in Pants) {
                writer.WriteItem<GearCG>(item);
            }
            writer.WriteCompressedUInt((uint)Footwear.Count());
            foreach (var item in Footwear) {
                writer.WriteItem<GearCG>(item);
            }
            writer.WriteCompressedUInt((uint)ClothingColors.Count());
            foreach (var item in ClothingColors) {
                writer.WriteUInt32(item);
            }
            return true;
        }

    }

}
