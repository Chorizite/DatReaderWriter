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
    /// A base class for packed strings using AC1Legacy pack format with elements of type TValue.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class AC1LegacyPStringBase<TValue> : StringBase<TValue> where TValue : unmanaged {
        public static implicit operator AC1LegacyPStringBase<TValue>(string str) => new() { Value = str };
        
        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            // Read 16-bit length marker
            ushort lengthMarker = reader.ReadUInt16();
            
            uint length;
            if (lengthMarker == 0xFFFF) {
                // Large string: read actual 32-bit length
                length = reader.ReadUInt32();
            }
            else {
                // Small/medium string: length is the marker value
                length = lengthMarker;
            }
            
            if (Marshal.SizeOf(typeof(TValue)) == 1) {
                Value = Encoding.Default.GetString(reader.ReadBytes((int)length));
            }
            else if (Marshal.SizeOf(typeof(TValue)) == 2) {
                var str = new StringBuilder((int)length);
                for (int i = 0; i < length; i++) {
                    str.Append(Convert.ToChar(reader.ReadUInt16()));
                }
                Value = str.ToString();
            }
            else {
                throw new NotSupportedException($"TValue of type {typeof(TValue)} is not supported for AC1LegacyPStringBase.");
            }

            reader.Align(4);

            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            int elementSize = Marshal.SizeOf(typeof(TValue));
            byte[] data;
            uint length;
            
            if (elementSize == 1) {
                data = Encoding.Default.GetBytes(Value);
                length = (uint)data.Length;
            }
            else if (elementSize == 2) {
                length = (uint)Value.Length;
                data = new byte[length * 2];
                for (int i = 0; i < Value.Length; i++) {
                    ushort charValue = Value[i];
                    data[i * 2] = (byte)(charValue & 0xFF);
                    data[i * 2 + 1] = (byte)((charValue >> 8) & 0xFF);
                }
            }
            else {
                throw new NotSupportedException($"TValue of type {typeof(TValue)} is not supported for AC1LegacyPStringBase.");
            }

            // Write length
            if (length >= 0xFFFF) {
                writer.WriteUInt16(0xFFFF);
                writer.WriteUInt32(length);
            }
            else {
                writer.WriteUInt16((ushort)length);
            }

            writer.WriteBytes(data, data.Length);

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

            return Equals((AC1LegacyPStringBase<TValue>)obj);
        }
    }
}