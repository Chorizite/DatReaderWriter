using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
namespace ACDatReader.FileTypes
{
    public struct Texture : IDatFileType {
        private uint Id;
        public int Unknown;
        public int Width;
        public int Height;
        public SurfacePixelFormat Format;
        public int Length;
        public byte[] SourceData;
        public uint? DefaultPaletteId;
        public bool Unpack(DatReader reader) {
            Id = reader.ReadUInt32();
            Unknown = reader.ReadInt32();
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            Format = (SurfacePixelFormat)reader.ReadUInt32();
            Length = reader.ReadInt32();
            //SourceData = reader.ReadBytes(Length);

            switch (Format) {
                case SurfacePixelFormat.PFID_INDEX16:
                case SurfacePixelFormat.PFID_P8:
                    DefaultPaletteId = reader.ReadUInt32();
                    break;
                default:
                    DefaultPaletteId = null;
                    break;
            }

            return true;
        }

        public override string ToString() {
            var str = new StringBuilder();

            str.AppendLine($"Texture {Id:X8}");
            str.AppendLine($"\t Unknown: {Unknown}");
            str.AppendLine($"\t Width: {Width}");
            str.AppendLine($"\t Height: {Height}");
            str.AppendLine($"\t Format: {Format}");
            str.AppendLine($"\t Length: {Length}");

            return str.ToString();
        }
    }
}
*/