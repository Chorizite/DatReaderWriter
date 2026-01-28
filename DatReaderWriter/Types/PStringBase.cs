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
    /// A base class for packed strings with elements of type TValue.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class PStringBase<TValue> : IUnpackable, IPackable, IEquatable<string> where TValue : unmanaged {
        /// <summary>
        /// The string value.
        /// </summary>
        public string Value { get; set; } = String.Empty;
        
        // implicit string conversion
        public static implicit operator string(PStringBase<TValue> pString) => pString.Value;
        public static implicit operator PStringBase<TValue>(string str) => new() { Value = str };
        
        public bool Unpack(DatBinReader reader) {
            var length = (int)reader.ReadCompressedUInt();
            
            if (Marshal.SizeOf(typeof(TValue)) == 1) {
                Value = Encoding.Default.GetString(reader.ReadBytes(length));
            }
            else if (Marshal.SizeOf(typeof(TValue)) == 2) {
                var str = new StringBuilder();
                for (int i = 0; i < length; i++) {
                    str.Append(Convert.ToChar(reader.ReadUInt16()));
                }
                Value = str.ToString();
            }
            else {
                throw new NotSupportedException($"TValue of type {typeof(TValue)} is not supported for PStringBase.");
            }

            return true;
        }

        public bool Pack(DatBinWriter writer) {
            if (Marshal.SizeOf(typeof(TValue)) == 1) {
                var bytes = Encoding.Default.GetBytes(Value);
                writer.WriteCompressedUInt((uint)bytes.Length);
                writer.WriteBytes(bytes, bytes.Length);
            }
            else if (Marshal.SizeOf(typeof(TValue)) == 2) {
                writer.WriteCompressedUInt((uint)Value.Length);
                foreach (char c in Value) {
                    writer.WriteUInt16(c);
                }
            }
            else {
                throw new NotSupportedException($"TValue of type {typeof(TValue)} is not supported for PStringBase.");
            }

            return true;
        }

        public bool Equals(string other)
        {
            return Value == other;
        }

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

            return Equals((PStringBase<TValue>)obj);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }
    }
}
