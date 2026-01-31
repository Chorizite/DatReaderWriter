using System;
using System.Numerics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DatReaderWriter.Enums;
using System.Text;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.Types {
    /// <summary>
    /// Information about a spell
    /// </summary>
    public partial class SpellBase : IDatObjType {
        // S_CONSTANT: Type: 0x108E, Value: (LF_ULONG) 303068800, SPELLBASE_NAME_HASH_KEY
        // S_CONSTANT: Type: 0x108E, Value: (LF_LONG) -1095905467, SPELLBASE_DESC_HASH_KEY
        private const uint SPELLBASE_NAME_HASH_KEY = 0x12107680u;
        private const uint SPELLBASE_DESC_HASH_KEY = 0xBEADCF45u;

        private uint[] _components = new uint[8];

        /// <summary>
        /// Name of the spell
        /// </summary>
        public ObfuscatedPStringBase Name = "";

        /// <summary>
        /// Description of the spell
        /// </summary>
        public ObfuscatedPStringBase Description = "";

        /// <summary>
        /// A list of component ids. You can look these up in the <see cref="SpellComponentTable"/>.
        /// This is limited to maximum 8 components.
        /// </summary>
        /// <remarks>
        /// These will transparently be encrypted / decrypted.
        /// The encryption key is based on the spell name and description.
        /// </remarks>
        public List<uint> Components = [];

        /// <summary>
        /// The magic school of the spell
        /// </summary>
        public MagicSchool School;

        /// <summary>
        /// The data id for the spell icon
        /// </summary>
        public uint Icon;

        /// <summary>
        /// Spell category
        /// </summary>
        public SpellCategory Category;

        public SpellIndex Bitfield;

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

        public PlayScript CasterEffect;

        public PlayScript TargetEffect;

        public PlayScript FizzleEffect;

        public double RecoveryInterval;

        public float RecoveryAmount;

        public uint DisplayOrder;

        public ItemType NonComponentTargetType;

        public uint ManaMod;



        /// <summary>
        /// Generates a hash based on the string. Used to decrypt spell formulas and calculate taper rotation for players.
        /// </summary>
        /// <remarks>From ACE</remarks>
        public static uint GetStringHash(string strToHash) {
            long result = 0;

            if (strToHash.Length > 0) {
#if NET8_0_OR_GREATER
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
                byte[] str = Encoding.GetEncoding(1252).GetBytes(strToHash);

                foreach (sbyte c in str) {
                    result = c + (result << 4);

                    if ((result & 0xF0000000) != 0)
                        result = (result ^ ((result & 0xF0000000) >> 24)) & 0x0FFFFFFF;
                }
            }

            return (uint)result;
        }

        /// <summary>
        /// Returns a hash key based on the spell's name and description
        /// </summary>
        /// <returns></returns>
        private uint GetHashKey() {
            uint nameHash = GetStringHash(Name);
            uint descHash = GetStringHash(Description);
            return (nameHash % SPELLBASE_NAME_HASH_KEY) + (descHash % SPELLBASE_DESC_HASH_KEY);
        }

        /// <summary>
        /// Returns a list of decrypted spell component ids.
        /// </summary>
        /// <remarks>From ACE</remarks>
        private List<uint> DecryptComponents(uint[] components) {
            if (components.Length > 8) {
                throw new ArgumentException("Components array must contain no more than 8 values.", nameof(components));
            }
            var key = GetHashKey();
            var comps = new uint[8];

            for (int i = 0; i < components.Length; i++) {
                uint comp = (components[i] - key);

                // This seems to correct issues with certain spells with extended characters.
                // highest comp ID is 198 - "Essence of Kemeroi", for Void Spells
                if (comp > 198) {
                    comp &= 0xFF;
                }

                comps[i] = components[i] == 0 ? 0 : comp;
            }

            return comps.Where(x => x > 0).ToList();
        }

        /// <summary>
        /// Encrypts component ids based on the spell's name and description
        /// </summary>
        /// <param name="components">List of component IDs to encrypt. Should always be 8 values. Use 0 for empty slots.</param>
        private List<uint> EncryptComponents(List<uint> components) {
            if (components.Count > 8) {
                throw new ArgumentException("Components array must contain no more than 8 values.", nameof(components));
            }

            var key = GetHashKey();
            var encryptedComps = new List<uint>(8);

            for (int i = 0; i < 8; i++) {
                if (components.Count <= i) {
                    encryptedComps.Add(0);
                    continue;
                }

                if (components[i] > 198 && components[i] != 0) {
                    throw new ArgumentException($"Component ID at index {i} is {components[i]}. Must be <= 198 or 0.", nameof(components));
                }
                encryptedComps.Add(components[i] == 0 ? 0 : (components[i] + key));
            }

            return encryptedComps;
        }

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            Name = reader.ReadItem<ObfuscatedPStringBase>();
            reader.Align(4);
            Description = reader.ReadItem<ObfuscatedPStringBase>();
            reader.Align(4);
            School = (MagicSchool)reader.ReadInt32();
            Icon = reader.ReadUInt32();
            Category = (SpellCategory)reader.ReadUInt32();
            Bitfield = (SpellIndex)reader.ReadInt32();
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
            var components = new uint[8];
            for (var i=0; i < 8; i++) {
                components[i] = reader.ReadUInt32();
            }
            Components = DecryptComponents(components);
            CasterEffect = (PlayScript)reader.ReadUInt32();
            TargetEffect = (PlayScript)reader.ReadUInt32();
            FizzleEffect = (PlayScript)reader.ReadUInt32();
            RecoveryInterval = reader.ReadDouble();
            RecoveryAmount = reader.ReadSingle();
            DisplayOrder = reader.ReadUInt32();
            NonComponentTargetType = (ItemType)reader.ReadUInt32();
            ManaMod = reader.ReadUInt32();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteItem(Name);
            writer.WriteItem(Description);
            writer.WriteInt32((int)School);
            writer.WriteUInt32(Icon);
            writer.WriteUInt32((uint)Category);
            writer.WriteInt32((int)Bitfield);
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
            var encryptedComponents = EncryptComponents(Components);
            for (var i=0; i < 8; i++) {
                writer.WriteUInt32(encryptedComponents[i]);
            }
            writer.WriteUInt32((uint)CasterEffect);
            writer.WriteUInt32((uint)TargetEffect);
            writer.WriteUInt32((uint)FizzleEffect);
            writer.WriteDouble(RecoveryInterval);
            writer.WriteSingle(RecoveryAmount);
            writer.WriteUInt32(DisplayOrder);
            writer.WriteUInt32((uint)NonComponentTargetType);
            writer.WriteUInt32(ManaMod);
            return true;
        }
    }
}
