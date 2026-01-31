using System;
using System.IO;
using System.Collections.Generic;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.IO;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;

namespace DatReaderWriter.Types {
    /// <summary>
    /// A base class for packed strings using obfuscated AC1Legacy pack format with elements of type TValue.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class ObfuscatedPStringBase : StringBase<byte> {
        private static readonly Encoding Windows1252 = Encoding.GetEncoding(1252);
        
        public static implicit operator ObfuscatedPStringBase(string str) => new() { Value = str };
        
        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
#if NET8_0_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            // Read the string length (stored as UInt16)
            int stringLength = reader.ReadUInt16();

            // Read the obfuscated bytes
            byte[] obfuscatedBytes = reader.ReadBytes(stringLength);

            // Deobfuscate each byte by rotating the bits
            for (int i = 0; i < stringLength; i++) {
                obfuscatedBytes[i] = (byte)(obfuscatedBytes[i] >> 4 | obfuscatedBytes[i] << 4);
            }

            // Convert bytes to string using Windows-1252 encoding
            Value = Windows1252.GetString(obfuscatedBytes);

            reader.Align(4);

            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
#if NET8_0_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            // Convert string to bytes using Windows-1252 encoding
            byte[] bytes = Windows1252.GetBytes(Value);

            // Write the string length as UInt16
            writer.WriteUInt16((ushort)bytes.Length);

            // Obfuscate and write each byte
            for (int i = 0; i < bytes.Length; i++) {
                writer.WriteByte((byte)(bytes[i] >> 4 | bytes[i] << 4));
            }
            writer.Align(4);

            return true;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }
            
            if (obj is string str) {
                return Equals(str);
            }

            if (obj.GetType() != GetType()) {
                return false;
            }

            return Equals((ObfuscatedPStringBase)obj);
        }
    }
}