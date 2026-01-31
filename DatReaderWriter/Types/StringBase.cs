using DatReaderWriter.Lib.IO;
using System;

namespace DatReaderWriter.Types {
    public abstract class StringBase : IEquatable<string> {
        /// <summary>
        /// The string value.
        /// </summary>
        public string Value { get; set; } = String.Empty;
        
        // implicit string conversion
        public static implicit operator string(StringBase pString) => pString.Value;
        
        /// <inheritdoc />
        public bool Equals(string other)
        {
            return Value == other;
        }
    }

    public abstract class StringBase<TValue> : StringBase, IUnpackable, IPackable where TValue : unmanaged {
        
        /// <inheritdoc />
        public abstract bool Pack(DatBinWriter writer);
        
        /// <inheritdoc />
        public abstract bool Unpack(DatBinReader reader);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(Value))
                return 0;
    
            uint hash = 0;
    
            foreach (char c in Value)
            {
                hash = (uint)c + (hash << 4); // c + 16 * hash
        
                uint highBits = hash & 0xF0000000;
                if (highBits != 0)
                {
                    hash = (hash ^ (highBits >> 24)) & 0x0FFFFFFF;
                }
            }
    
            // Avoid using -1 as hash value (0xFFFFFFFF in unsigned, -1 in signed)
            if (hash == 0xFFFFFFFF)
                return unchecked((int)0xFFFFFFFE); // Return -2
    
            return unchecked((int)hash);
        }

        /// <inheritdoc />
        public override string ToString() => Value;
    }
}