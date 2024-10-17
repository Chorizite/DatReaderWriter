using System;
using System.Buffers.Binary;
using System.Linq;
using System.Numerics;

namespace ACClientLib.DatReaderWriter.IO {
    /// <summary>
    /// Reads / parses dat file bytes. This expects that the raw data buffer
    /// passed already has its blocks followed, and is a contiguous chunk of memory.
    /// </summary>
    public class DatFileReader {
        private readonly ReadOnlyMemory<byte> _data;
        private int _offset;

        /// <summary>
        /// The current offset into the buffer
        /// </summary>
        public int Offset => _offset;

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="data">The file data being parsed</param>
        public DatFileReader(ReadOnlyMemory<byte> data) {
            _data = data;
        }

        unsafe private ReadOnlySpan<byte> ReadBytesInternal(int numBytes) {
            _offset += numBytes;
            return _data.Span.Slice(_offset - numBytes, numBytes);
        }

        /// <summary>
        /// Align the underlying buffer position
        /// </summary>
        /// <param name="v"></param>
        public void Align(int v) {
            var alignDelta  = _offset % v;

            if (alignDelta > 0) {
                Skip(v - alignDelta);
            }
        }

        /// <summary>
        /// Advance the buffer position without reading any data
        /// </summary>
        /// <param name="numBytes">The number of bytes to skip</param>
        public void Skip(int numBytes) {
            _offset += numBytes;
        }

        /// <summary>
        /// Read an <see cref="IUnpackable"/> and advance the buffer position accordingly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ReadItem<T>() where T : IUnpackable {
            var item = Activator.CreateInstance<T>();
            item.Unpack(this);
            return item;
        }

        /// <summary>
        /// Read the specified number of bytes and advance the buffer position accordingly
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>The bytes that were read</returns>
        public byte[] ReadBytes(int count) {
            return ReadBytesInternal(count).ToArray();
        }

        /// <summary>
        /// Read a byte and advance the buffer position accordingly
        /// </summary>
        public byte ReadByte() {
            return ReadBytesInternal(1).ToArray()[0];
        }

        /// <summary>
        /// Read an bool and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        public bool ReadBool() {
            return BinaryPrimitives.ReadUInt32LittleEndian(ReadBytesInternal(4)) != 0;
        }

        /// <summary>
        /// Read an int64 and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        public long ReadInt64() {
            return BinaryPrimitives.ReadInt64LittleEndian(ReadBytesInternal(8));
        }


        /// <summary>
        /// Read an uint64 and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        public ulong ReadUInt64() {
            return BinaryPrimitives.ReadUInt64LittleEndian(ReadBytesInternal(8));
        }

        /// <summary>
        /// Read an int32 and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        public int ReadInt32() {
            return BinaryPrimitives.ReadInt32LittleEndian(ReadBytesInternal(4));
        }

        /// <summary>
        /// Read an uint32 and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        public uint ReadUInt32() {
            return BinaryPrimitives.ReadUInt32LittleEndian(ReadBytesInternal(4));
        }

        /// <summary>
        /// Read an int16 and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        public short ReadInt16() {
            return BinaryPrimitives.ReadInt16LittleEndian(ReadBytesInternal(2));
        }


        /// <summary>
        /// Read an uint16 and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        public ushort ReadUInt16() {
            return BinaryPrimitives.ReadUInt16LittleEndian(ReadBytesInternal(2));
        }

        /// <summary>
        /// Read a float (single) and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        public float ReadSingle() {
#if NETSTANDARD2_0
            return BitConverter.ToSingle(ReadBytesInternal(4).ToArray(), 0);
#else
            return BinaryPrimitives.ReadSingleLittleEndian(ReadBytesInternal(4));
#endif
        }

        /// <summary>
        /// Read a double and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
        public double ReadDouble() {
#if NETSTANDARD2_0
            return BitConverter.ToDouble(ReadBytesInternal(8).ToArray(), 0);
#else
            return BinaryPrimitives.ReadDoubleLittleEndian(ReadBytesInternal(8));
#endif
        }

        /// <summary>
        /// Read a <see cref="Vector3"/> and advance the buffer position accordingly
        /// </summary>
        /// <returns></returns>
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
        public uint ReadCompressedUInt() {
            var b0 = ReadByte();
            if ((b0 & 0x80) == 0)
                return b0;

            var b1 = ReadByte();
            if ((b0 & 0x40) == 0)
                return (uint)(((b0 & 0x7F) << 8) | b1);

            var s = ReadUInt16();
            return (uint)(((((b0 & 0x3F) << 8) | b1) << 16) | s);
        }

        /// <summary>
        /// Read a <see cref="Plane"/> and advance the buffer position accordingly.
        /// </summary>
        /// <returns></returns>
        public Plane ReadPlane() {
            var normal = ReadVector3();
            var distance = ReadSingle();
            return new Plane(normal, distance);
        }
    }
}
