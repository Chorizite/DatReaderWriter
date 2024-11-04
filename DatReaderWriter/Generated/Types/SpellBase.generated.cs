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
    public partial class SpellBase : IDatObjType {
        public string Name;

        public string Description;

        public MagicSchool School;

        public uint Icon;

        public SpellCategory Category;

        public uint Bitfield;

        public uint BaseMana;

        public float BaseRangeConstant;

        public float BaseRangeMod;

        public uint Power;

        public float SpellEconomyMod;

        public uint FormulaVersion;

        public float ComponentLoss;

        public SpellType MetaSpellType;

        public uint MetaSpellId;

        /// <summary>
        /// This is only set when MetaSpellType is SpellType.Enchantment | SpellType.FellowEnchantment.
        /// </summary>
        public double Duration;

        /// <summary>
        /// This is only set when MetaSpellType is SpellType.Enchantment | SpellType.FellowEnchantment.
        /// </summary>
        public float DegradeModifier;

        /// <summary>
        /// This is only set when MetaSpellType is SpellType.Enchantment | SpellType.FellowEnchantment.
        /// </summary>
        public float DegradeLimit;

        /// <summary>
        /// This is only set when MetaSpellType is SpellType.PortalSummon.
        /// </summary>
        public double PortalLifetime;

        /// <summary>
        /// This should always be a length of 8.
        /// </summary>
        public uint[] RawComponents = [];

        public uint CasterEffect;

        public uint TargetEffect;

        public uint FizzleEffect;

        public double RecoveryInterval;

        public float RecoveryAmount;

        public uint DisplayOrder;

        public uint NonComponentTargetType;

        public uint ManaMod;

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            Name = reader.ReadObfuscatedString();
            reader.Align(4);
            Description = reader.ReadObfuscatedString();
            reader.Align(4);
            School = (MagicSchool)reader.ReadInt32();
            Icon = reader.ReadUInt32();
            Category = (SpellCategory)reader.ReadUInt32();
            Bitfield = reader.ReadUInt32();
            BaseMana = reader.ReadUInt32();
            BaseRangeConstant = reader.ReadSingle();
            BaseRangeMod = reader.ReadSingle();
            Power = reader.ReadUInt32();
            SpellEconomyMod = reader.ReadSingle();
            FormulaVersion = reader.ReadUInt32();
            ComponentLoss = reader.ReadSingle();
            MetaSpellType = (SpellType)reader.ReadUInt32();
            MetaSpellId = reader.ReadUInt32();
            switch(MetaSpellType) {
                case SpellType.Enchantment:
                case SpellType.FellowEnchantment:
                    Duration = reader.ReadDouble();
                    DegradeModifier = reader.ReadSingle();
                    DegradeLimit = reader.ReadSingle();
                    break;
                case SpellType.PortalSummon:
                    PortalLifetime = reader.ReadDouble();
                    break;
            }
            RawComponents = new uint[8];
            for (var i=0; i < 8; i++) {
                RawComponents[i] = reader.ReadUInt32();
            }
            CasterEffect = reader.ReadUInt32();
            TargetEffect = reader.ReadUInt32();
            FizzleEffect = reader.ReadUInt32();
            RecoveryInterval = reader.ReadDouble();
            RecoveryAmount = reader.ReadSingle();
            DisplayOrder = reader.ReadUInt32();
            NonComponentTargetType = reader.ReadUInt32();
            ManaMod = reader.ReadUInt32();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteObfuscatedString(Name);
            writer.Align(4);
            writer.WriteObfuscatedString(Description);
            writer.Align(4);
            writer.WriteInt32((int)School);
            writer.WriteUInt32(Icon);
            writer.WriteUInt32((uint)Category);
            writer.WriteUInt32(Bitfield);
            writer.WriteUInt32(BaseMana);
            writer.WriteSingle(BaseRangeConstant);
            writer.WriteSingle(BaseRangeMod);
            writer.WriteUInt32(Power);
            writer.WriteSingle(SpellEconomyMod);
            writer.WriteUInt32(FormulaVersion);
            writer.WriteSingle(ComponentLoss);
            writer.WriteUInt32((uint)MetaSpellType);
            writer.WriteUInt32(MetaSpellId);
            switch(MetaSpellType) {
                case SpellType.Enchantment:
                case SpellType.FellowEnchantment:
                    writer.WriteDouble(Duration);
                    writer.WriteSingle(DegradeModifier);
                    writer.WriteSingle(DegradeLimit);
                    break;
                case SpellType.PortalSummon:
                    writer.WriteDouble(PortalLifetime);
                    break;
            }
            for (var i=0; i < RawComponents.Count(); i++) {
                writer.WriteUInt32(RawComponents[i]);
            }
            writer.WriteUInt32(CasterEffect);
            writer.WriteUInt32(TargetEffect);
            writer.WriteUInt32(FizzleEffect);
            writer.WriteDouble(RecoveryInterval);
            writer.WriteSingle(RecoveryAmount);
            writer.WriteUInt32(DisplayOrder);
            writer.WriteUInt32(NonComponentTargetType);
            writer.WriteUInt32(ManaMod);
            return true;
        }

    }

}
