using System;
using System.Buffers.Binary;

namespace ACDatReader.IO {
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
        /// Advance the buffer position without reading any data
        /// </summary>
        /// <param name="numBytes">The number of bytes to skip</param>
        public void Skip(int numBytes) {
            _offset += numBytes;
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
    }
}
