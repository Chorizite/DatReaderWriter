using System;
using System.Buffers.Binary;

namespace ACDatReader.IO {
    /// <summary>
    /// A dat file writer. Used for writing dat block contents to a buffer.
    /// </summary>
    public class DatFileWriter {
        private readonly Memory<byte> _data;
        private int _offset;

        /// <summary>
        /// The current offset into the buffer
        /// </summary>
        public int Offset => _offset;

        /// <summary>
        /// Create a new instance of this DatFileWriter
        /// </summary>
        /// <param name="data">The file data being written</param>
        public DatFileWriter(Memory<byte> data) {
            _data = data;
        }

        unsafe private Span<byte> GetSpanAndAdvanceOffset(int numBytes) {
            _offset += numBytes;
            return _data.Span.Slice(_offset - numBytes, numBytes);
        }

        /// <summary>
        /// Advance the buffer position without writing any data
        /// </summary>
        /// <param name="numBytes">The number of bytes to skip</param>
        public void Skip(int numBytes) {
            _offset += numBytes;
        }

        /// <summary>
        /// Write the specified number of bytes and advance the buffer position accordingly
        /// </summary>
        public void WriteBytes(byte[] buffer, int numBytes) {
            buffer.AsSpan().Slice(0, numBytes).CopyTo(GetSpanAndAdvanceOffset(numBytes));
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
    }
}
