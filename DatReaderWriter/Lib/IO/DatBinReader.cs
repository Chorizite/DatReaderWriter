using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace DatReaderWriter.Lib.IO {
    /// <summary>
    /// Reads / parses dat file bytes. This expects that the raw data buffer
    /// passed already has its blocks followed, and is a contiguous chunk of memory.
    /// </summary>
    public class DatBinReader {
        private readonly ReadOnlyMemory<byte> _data;
        private static readonly Encoding Windows1252 = Encoding.GetEncoding(1252);
        private int _offset;

        /// <summary>
        /// The current offset into the buffer
        /// </summary>
        public int Offset => _offset;

        /// <summary>
        /// The total length of the buffer
        /// </summary>
        public int Length => _data.Length;

        /// <summary>
        /// The current DatDatabase (may be null)
        /// </summary>
        public readonly DatDatabase? Database;

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="data">The file data being parsed</param>
        /// <param name="db"></param>
        public DatBinReader(ReadOnlyMemory<byte> data, DatDatabase? db = null) {
            _data = data;
            Database = db;
        }

        unsafe private ReadOnlySpan<byte> ReadBytesInternal(int numBytes) {
            _offset += numBytes;
            return _data.Span.Slice(_offset - numBytes, numBytes);
        }

        /// <summary>
        /// Align the underlying buffer position
        /// </summary>
        /// <param name="v"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Align(int v) {
            var alignDelta = _offset % v;

            if (alignDelta > 0) {
                Skip(v - alignDelta);
            }
        }

        /// <summary>
        /// Advance the buffer position without reading any data
        /// </summary>
        /// <param name="numBytes">The number of bytes to skip</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Skip(int numBytes) {
            _offset += numBytes;
        }

        /// <summary>
        /// Read an <see cref="IUnpackable"/> and advance the buffer position accordingly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ReadItem<T>(params object[] p) where T : IUnpackable {
            var item = (T)Activator.CreateInstance(typeof(T), p ?? []);
            item.Unpack(this);
            return item;
        }

        /// <summary>
        /// Read the specified number of bytes and advance the buffer position accordingly
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>The bytes that were read</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ReadBytes(int count) {
            return ReadBytesInternal(count).ToArray();
        }

        /// <summary>
        /// Read a byte and advance the buffer position accordingly
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte() {
            return ReadBytesInternal(1)[0];
        }

        /// <summary>
        /// Read a signed byte and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte() {
            return (sbyte)ReadBytesInternal(1)[0];
        }

        /// <summary>
        /// Read an bool and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBool(int size = 4) {
            switch (size) {
                case 8:
                    return ReadUInt64() != 0;
                case 4:
                    return ReadUInt32() != 0;
                case 2:
                    return ReadUInt16() != 0;
                case 1:
                    return ReadByte() != 0;
                default:
                    throw new NotSupportedException($"Unsupported bool size: {size}");
            }
        }

        /// <summary>
        /// Read an int64 and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64() {
            var span = ReadBytesInternal(8);
            return Unsafe.ReadUnaligned<long>(ref MemoryMarshal.GetReference(span));
        }


        /// <summary>
        /// Read an uint64 and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64() {
            var span = ReadBytesInternal(8);
            return Unsafe.ReadUnaligned<ulong>(ref MemoryMarshal.GetReference(span));
        }

        /// <summary>
        /// Read an int32 and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32() {
            var span = ReadBytesInternal(4);
            return Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span));
        }

        /// <summary>
        /// Read an uint32 and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32() {
            var span = ReadBytesInternal(4);
            return Unsafe.ReadUnaligned<uint>(ref MemoryMarshal.GetReference(span));
        }

        /// <summary>
        /// Read an int16 and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16() {
            return BinaryPrimitives.ReadInt16LittleEndian(ReadBytesInternal(2));
        }


        /// <summary>
        /// Read an uint16 and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16() {
            return BinaryPrimitives.ReadUInt16LittleEndian(ReadBytesInternal(2));
        }

        /// <summary>
        /// Read a float (single) and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadSingle() {
#if NETSTANDARD2_0 || NETFRAMEWORK
            return BitConverter.ToSingle(ReadBytesInternal(4).ToArray(), 0);
#else
            return BinaryPrimitives.ReadSingleLittleEndian(ReadBytesInternal(4));
#endif
        }

        /// <summary>
        /// Read a double and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ReadDouble() {
#if NETSTANDARD2_0 || NETFRAMEWORK
            return BitConverter.ToDouble(ReadBytesInternal(8).ToArray(), 0);
#else
            return BinaryPrimitives.ReadDoubleLittleEndian(ReadBytesInternal(8));
#endif
        }

        /// <summary>
        /// Read a <see cref="Vector3"/> and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 ReadVector3() {
            var x = ReadSingle();
            var y = ReadSingle();
            var z = ReadSingle();
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Read a <see cref="Quaternion"/> and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Quaternion ReadQuaternion() {
            var w = ReadSingle();
            var x = ReadSingle();
            var y = ReadSingle();
            var z = ReadSingle();
            return new Quaternion(x, y, z, w);
        }

        /// <summary>
        /// Read a compressed uint and advance the buffer position accordingly. Logic from ACE<para />
        /// 
        /// A Compressed UInt32 can be 1, 2, or 4 bytes.<para />
        /// If the first MSB (0x80) is 0, it is one byte.<para />
        /// If the first MSB (0x80) is set and the second MSB (0x40) is 0, it's 2 bytes.<para />
        /// If both (0x80) and (0x40) are set, it's 4 bytes.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadCompressedUInt() {
            var b0 = ReadByte();
            if ((b0 & 0x80) == 0)
                return b0;

            var b1 = ReadByte();
            if ((b0 & 0x40) == 0)
                return (uint)((b0 & 0x7F) << 8 | b1);

            var s = ReadUInt16();
            return (uint)(((b0 & 0x3F) << 8 | b1) << 16 | s);
        }

        /// <summary>
        /// Read a <see cref="Plane"/> and advance the buffer position accordingly.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Plane ReadPlane() {
            var normal = ReadVector3();
            var distance = ReadSingle();
            return new Plane(normal, distance);
        }

        /// <summary>
        /// From ACE: First reads a UInt16. If the MSB is set, it will be masked with 0x3FFF, shifted left 2 bytes, and then OR'd with the next UInt16. The sum is then added to knownType.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadDataIdOfKnownType(uint knownType) {
            var value = ReadUInt16();

            if ((value & 0x8000) != 0) {
                var lower = ReadUInt16();
                var higher = (value & 0x3FFF) << 16;

                return (uint)(knownType + (higher | lower));
            }

            return knownType + value;
        }

        /// <summary>
        /// Reads a <see cref="Guid"/> from the current stream.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Guid ReadGuid() {
            return new Guid(ReadBytes(16));
        }

        /// <summary>
        /// Reads a generic type from the current stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public T ReadGeneric<T>() {
            var type = typeof(T);
            if (type == typeof(uint)) return (T)(object)ReadUInt32();
            if (type == typeof(int)) return (T)(object)ReadInt32();
            if (type == typeof(ulong)) return (T)(object)ReadUInt64();
            if (type == typeof(long)) return (T)(object)ReadInt64();
            if (type == typeof(ushort)) return (T)(object)ReadUInt16();
            if (type == typeof(short)) return (T)(object)ReadInt16();
            if (type == typeof(byte)) return (T)(object)ReadByte();
            if (type == typeof(sbyte)) return (T)(object)ReadSByte();
            if (type == typeof(bool)) return (T)(object)ReadBool();
            if (type == typeof(float)) return (T)(object)ReadSingle();
            if (type == typeof(double)) return (T)(object)ReadDouble();
            if (type == typeof(Guid)) return (T)(object)ReadGuid();
            
            // Check if this is an enum, get the underlying type or default to int
            if (type.IsEnum) {
                var underlyingType = Enum.GetUnderlyingType(type);
                if (underlyingType == typeof(byte)) {
                    var val = ReadByte();
                    return (T)(object)Enum.ToObject(type, val);
                }
                else if (underlyingType == typeof(sbyte)) {
                    var val = ReadSByte();
                    return (T)(object)Enum.ToObject(type, val);
                }
                else if (underlyingType == typeof(ushort)) {
                    var val = ReadUInt16();
                    return (T)(object)Enum.ToObject(type, val);
                }
                else if (underlyingType == typeof(short)) {
                    var val = ReadInt16();
                    return (T)(object)Enum.ToObject(type, val);
                }
                else if (underlyingType == typeof(uint)) {
                    var val = ReadUInt32();
                    return (T)(object)Enum.ToObject(type, val);
                }
                else {
                    var val = ReadInt32();
                    return (T)(object)Enum.ToObject(type, val);
                }
            }

            if (typeof(IUnpackable).IsAssignableFrom(type)) {
                var item = (IUnpackable)Activator.CreateInstance(type);
                item.Unpack(this);
                return (T)item;
            }

            throw new NotSupportedException($"Type {type.Name} is not supported by ReadGeneric.");
        }
    }
}
