using ACClientLib.DatReaderWriter.DBObjs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACClientLib.DatReaderWriter.Types {
    public partial class SpellBase {
        // S_CONSTANT: Type: 0x108E, Value: (LF_ULONG) 303068800, SPELLBASE_NAME_HASH_KEY
        // S_CONSTANT: Type: 0x108E, Value: (LF_LONG) -1095905467, SPELLBASE_DESC_HASH_KEY
        private const uint SPELLBASE_NAME_HASH_KEY = 0x12107680u;
        private const uint SPELLBASE_DESC_HASH_KEY = 0xBEADCF45u;


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
        public uint GetHashKey() {
            uint nameHash = GetStringHash(Name);
            uint descHash = GetStringHash(Description);
            return (nameHash % SPELLBASE_NAME_HASH_KEY) + (descHash % SPELLBASE_DESC_HASH_KEY);
        }

        /// <summary>
        /// Returns a list of decrypted spell component ids.
        /// </summary>
        /// <remarks>From ACE</remarks>
        public uint[] DecryptedComponents() {
            var key = GetHashKey();
            var comps = new uint[RawComponents.Length];

            for (int i = 0; i < RawComponents.Length; i++) {
                uint comp = (RawComponents[i] - key);

                // This seems to correct issues with certain spells with extended characters.
                // highest comp ID is 198 - "Essence of Kemeroi", for Void Spells
                if (comp > 198) {
                    comp &= 0xFF;
                }

                comps[i] = RawComponents[i] == 0 ? 0 : comp;
            }

            return comps;
        }

        /// <summary>
        /// Encrypts and updates <see cref="RawComponents"/> with the new encrypted values.
        /// </summary>
        /// <param name="components">List of component IDs to encrypt. Should always be 8 values. Use 0 for empty slots.</param>
        public void SetRawComponents(uint[] components) {
            if (components.Length != 8) {
                throw new ArgumentException("Components array must contain exactly 8 values.", nameof(components));
            }

            var key = GetHashKey();
            var encryptedComps = new uint[components.Length];

            for (int i = 0; i < components.Length; i++) {
                if (components[i] > 198 && components[i] != 0) {
                    throw new ArgumentException($"Component ID at index {i} is {components[i]}. Must be <= 198 or 0.", nameof(components));
                }

                encryptedComps[i] = components[i] == 0 ? 0 : (components[i] + key);
            }

            RawComponents = encryptedComps;
        }
    }
}
