using System;
using System.Buffers.Binary;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Text;

namespace DatReaderWriter.Lib.IO {
    /// <summary>
    /// A dat file writer. Used for writing dat block contents to a buffer.
    /// </summary>
    public class DatBinWriter {
        private readonly Memory<byte> _data;
        private int _offset;

        /// <summary>
        /// The current offset into the buffer
        /// </summary>
        public int Offset => _offset;

        /// <summary>
        /// Create a new instance of this DatBinWriter
        /// </summary>
        /// <param name="data">The file data being written</param>
        public DatBinWriter(Memory<byte> data) {
            _data = data;
        }

        unsafe private Span<byte> GetSpanAndAdvanceOffset(int numBytes) {
            _offset += numBytes;
            return _data.Span.Slice(_offset - numBytes, numBytes);
        }

        /// <summary>
        /// Align the underlying buffer position
        /// </summary>
        /// <param name="v"></param>
        public void Align(int v) {
            var alignDelta = _offset % v;

            if (alignDelta > 0) {
                Skip(v - alignDelta);
            }
        }

        /// <summary>
        /// Advance the buffer position without writing any data
        /// </summary>
        /// <param name="numBytes">The number of bytes to skip</param>
        public void Skip(int numBytes) {
            _offset += numBytes;
        }

        /// <summary>
        /// Write an <see cref="IPackable"/> and advance the buffer position accordingly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public void WriteItem<T>(T item) where T : IPackable {
            item.Pack(this);
        }

        /// <summary>
        /// Write a byte and advance the buffer position accordingly
        /// </summary>
        public void WriteByte(byte value) {
            GetSpanAndAdvanceOffset(1)[0] = value;
        }

        /// <summary>
        /// Write the specified number of bytes and advance the buffer position accordingly
        /// </summary>
        public void WriteBytes(byte[] buffer, int numBytes) {
            buffer.AsSpan().Slice(0, numBytes).CopyTo(GetSpanAndAdvanceOffset(numBytes));
        }

        /// <summary>
        /// Write an bool and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        public void WriteBool(bool value, int size = 4) {
            switch (size) {
                case 8:
                    WriteUInt64(value ? 1u : 0u);
                    break;
                case 4:
                    WriteUInt32(value ? 1u : 0u);
                    break;
                case 2:
                    WriteUInt16((ushort)(value ? 1u : 0u));
                    break;
                case 1:
                    WriteByte((byte)(value ? 1 : 0));
                    break;
                default:
                    throw new NotSupportedException($"Unsupported bool size: {size}");
            }
        }

        /// <summary>
        /// Write an int64 and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        public void WriteInt64(long value) {
            BinaryPrimitives.WriteInt64LittleEndian(GetSpanAndAdvanceOffset(8), value);
        }

        /// <summary>
        /// Write an uint64 and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        public void WriteUInt64(ulong value) {
            BinaryPrimitives.WriteUInt64LittleEndian(GetSpanAndAdvanceOffset(8), value);
        }

        /// <summary>
        /// Write an int32 and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        public void WriteInt32(int value) {
            BinaryPrimitives.WriteInt32LittleEndian(GetSpanAndAdvanceOffset(4), value);
        }

        /// <summary>
        /// Write an uint32 and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        public void WriteUInt32(uint value) {
            BinaryPrimitives.WriteUInt32LittleEndian(GetSpanAndAdvanceOffset(4), value);
        }

        /// <summary>
        /// Write an int16 and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        public void WriteInt16(short value) {
            BinaryPrimitives.WriteInt16LittleEndian(GetSpanAndAdvanceOffset(2), value);
        }

        /// <summary>
        /// Write an uint32 and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        public void WriteUInt16(ushort value) {
            BinaryPrimitives.WriteUInt16LittleEndian(GetSpanAndAdvanceOffset(2), value);
        }

        /// <summary>
        /// Write an single (float) and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        public void WriteSingle(float value) {
#if (NETFRAMEWORK || NETSTANDARD2_0)
            BitConverter.GetBytes(value).CopyTo(GetSpanAndAdvanceOffset(4));
#else
            BinaryPrimitives.WriteSingleLittleEndian(GetSpanAndAdvanceOffset(4), value);
#endif
        }

        /// <summary>
        /// Write an double and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        public void WriteDouble(double value) {
#if (NETFRAMEWORK || NETSTANDARD2_0)
            BitConverter.GetBytes(value).CopyTo(GetSpanAndAdvanceOffset(8));
#else
            BinaryPrimitives.WriteDoubleLittleEndian(GetSpanAndAdvanceOffset(8), value);
#endif
        }

        /// <summary>
        /// Write a Vector3 and advance the buffer position accordingly
        /// </summary>
        /// <param name="vec"></param>
        public void WriteVector3(Vector3 vec) {
            WriteSingle(vec.X);
            WriteSingle(vec.Y);
            WriteSingle(vec.Z);
        }

        /// <summary>
        /// Write a Vector3 and advance the buffer position accordingly
        /// </summary>
        /// <param name="quat"></param>
        public void WriteQuaternion(Quaternion quat) {
            WriteSingle(quat.W);
            WriteSingle(quat.X);
            WriteSingle(quat.Y);
            WriteSingle(quat.Z);
        }

        /// <summary>
        /// Write a compressed uint (UInt32) to the buffer. Logic mirrors ACE format.<para />
        /// 
        /// A Compressed UInt32 can be 1, 2, or 4 bytes.<para />
        /// If the value fits in 1 byte (0-127), it is written as a single byte.<para />
        /// If the value fits in 2 bytes (128-16,383), it is written as 2 bytes, with the first byte MSB set (0x80).<para />
        /// If the value requires 4 bytes (16,384 or higher), it is written as 4 bytes, with both first and second MSB set (0x80 and 0x40).
        /// </summary>
        /// <param name="value">The UInt32 value to compress and write.</param>
        public void WriteCompressedUInt(uint value) {
            if (value < 0x80) {
                // Fits in 1 byte (7 bits)
                WriteByte((byte)value);
            }
            else if (value < 0x4000) {
                // Fits in 2 bytes (14 bits)
                WriteByte((byte)(value >> 8 | 0x80));  // Set MSB (0x80)
                WriteByte((byte)(value & 0xFF));
            }
            else {
                // Requires 4 bytes (up to 32 bits)
                WriteByte((byte)(value >> 24 | 0xC0));  // Set both MSB (0x80) and 2nd MSB (0x40)
                WriteByte((byte)(value >> 16 & 0xFF));
                WriteByte((byte)(value >> 8 & 0xFF));
                WriteByte((byte)(value & 0xFF));
            }
        }

        /// <summary>
        /// Write a <see cref="Plane"/> and advance the buffer position accordingly.
        /// </summary>
        /// <param name="value">The <see cref="Plane"/> to write.</param>
        public void WritePlane(Plane value) {
            WriteVector3(value.Normal);
            WriteSingle(value.D);
        }

        /// <summary>
        /// Writes the data ID based on knownType, reversing the logic of ReadDataIdOfKnownType.
        /// If the dataID minus knownType is larger than 0x7FFF, it writes it in two UInt16 values.
        /// If it is smaller or equal to 0x7FFF, it writes it as a single UInt16 value.
        /// </summary>
        public void WriteDataIdOfKnownType(uint dataID, uint knownType) {
            // Subtract the known type to get the raw value
            var rawValue = dataID - knownType;

            if (rawValue > 0x7FFF) { // If the value is larger than 15 bits (0x7FFF)
                                     // Write the higher 16 bits with MSB set, masked with 0x3FFF
                var higher = (ushort)(0x8000 | rawValue >> 16 & 0x3FFF); // Set MSB and mask to 14 bits
                WriteUInt16(higher);

                // Write the lower 16 bits
                var lower = (ushort)(rawValue & 0xFFFF);
                WriteUInt16(lower);
            }
            else {
                // Write the raw value directly as a 16-bit value
                WriteUInt16((ushort)rawValue);
            }
        }

        public void WriteString16L(string value, int sizeOfLength = 2, bool align = true) {
            var bytes = Encoding.Default.GetBytes(value);
            switch (sizeOfLength) {
                case 1:
                    WriteByte((byte)bytes.Length);
                    break;
                case 2:
                default:
                    WriteUInt16((ushort)bytes.Length);
                    break;
            }
            WriteBytes(bytes, bytes.Length);
            if (align) Align(4);
        }

        public void WriteString16LByte(string value) {
            WriteString16L(value, 1, false);
        }

        /// <summary>
        /// Writes a string to a binary stream in an obfuscated format.
        /// The string is stored as a length-prefixed sequence of bytes where each byte has been bit-rotated.
        /// </summary>
        public void WriteObfuscatedString(string value) {
#if NET8_0_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            // Convert string to bytes using Windows-1252 encoding
            byte[] bytes = Encoding.GetEncoding(1252).GetBytes(value);

            // Write the string length as UInt16
            WriteUInt16((ushort)bytes.Length);

            // Obfuscate and write each byte
            for (int i = 0; i < bytes.Length; i++) {
                WriteByte((byte)(bytes[i] >> 4 | bytes[i] << 4));
            }
        }

        /// <summary>
        ///  Write a string from the current stream. The string is prefixed with the length,
        //     encoded as an integer seven bits at a time.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public void WriteString(string value) {
#if NET8_0_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            var strBytes = Encoding.GetEncoding(1252).GetBytes(value);
            WriteCompressedUInt((uint)strBytes.Length);
            WriteBytes(strBytes, strBytes.Length);
        }

        /// <summary>
        /// Writes a <see cref="Guid"/> and advance the buffer position accordingly.
        /// </summary>
        /// <param name="value"></param>
        public void WriteGuid(Guid value) {
            WriteBytes(value.ToByteArray(), 16);
        }

        /// <summary>
        /// Writes a string from the current stream. The string is prefixed with a compressed uint length.
        /// </summary>
        /// <param name="value"></param>
        public void WriteStringCompressed(string value) {
#if NET8_0_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            WriteCompressedUInt((uint)value.Length);
            if (value.Length > 0) {
                var strBytes = Encoding.GetEncoding(1252).GetBytes(value);
                WriteBytes(strBytes, strBytes.Length);
            }
        }
    }
}
