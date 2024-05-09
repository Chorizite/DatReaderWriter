using System;
using System.Buffers.Binary;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace ACDatReader.IO {
    /// <summary>
    /// Provides a convenient wrapper around reading dat file structures
    /// </summary>
    public class DatParser {
        private readonly ReadOnlyMemory<byte> _data;
        private int _offset;

        /// <summary>
        /// Create a new instance of this Dat Parser
        /// </summary>
        /// <param name="data">The file data being parsed</param>
        public DatParser(ReadOnlyMemory<byte> data) {
            _data = data;
        }

        unsafe private ReadOnlySpan<byte> ReadBytesInternal(int numBytes) {
            _offset += numBytes;
            return _data.Span.Slice(_offset - numBytes, numBytes);
        }

        /// <summary>
        /// Read a struct and advance the buffer position accordingly
        /// </summary>
        /// <typeparam name="T">The type of struct to read</typeparam>
        /// <returns></returns>
        unsafe public T ReadStruct<T>() where T : unmanaged {
            using var src = _data.Slice(_offset).Pin();
            _offset += sizeof(T);

            return Marshal.PtrToStructure<T>((nint)src.Pointer);
        }

        /// <summary>
        /// Read an array of structs and advance the buffer position accordingly
        /// </summary>
        /// <typeparam name="T">The type of struct to read</typeparam>
        /// <param name="count">The amount of structs in the array</param>
        /// <returns></returns>
        unsafe public T[] ReadStructArray<T>(int count) where T : unmanaged {
            var res = new T[count];
            var destPtr = (void*)Marshal.UnsafeAddrOfPinnedArrayElement(res, 0);
            using var dataHandle = _data.Slice(_offset).Pin();

            Span<int> size = [sizeof(T) * count];
            _offset += size[0];

            Buffer.MemoryCopy(dataHandle.Pointer, destPtr, size[0], size[0]);
            return res;
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
