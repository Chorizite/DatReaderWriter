using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACDatReader.FileTypes {
    /// <summary>
    /// A texture
    /// </summary>
    public class Texture : IDatFileType {
        /// <summary>
        /// The id of this texture
        /// </summary>
        public uint Id { get; set; }

        public int Unknown;

        /// <summary>
        /// The width of this texture
        /// </summary>
        public int Width;
        
        /// <summary>
        /// The height of this texture
        /// </summary>
        public int Height;
        /// <summary>
        /// The <see cref="SurfacePixelFormat"/>
        /// </summary>
        public SurfacePixelFormat Format;

        /// <summary>
        /// Data length of this texture
        /// </summary>
        public int Length;

        /// <summary>
        /// Source bytes of this texture
        /// </summary>
        public byte[] SourceData;

        /// <summary>
        /// The default palette id. Only valid when <see cref="SurfacePixelFormat"/> is
        /// <see cref="SurfacePixelFormat.PFID_INDEX16"/> or <see cref="SurfacePixelFormat.PFID_P8"/>
        /// </summary>
        public uint? DefaultPaletteId;

        ///<inheritdoc/>
        public bool Unpack(DatFileReader reader) {
            Id = reader.ReadUInt32();
            Unknown = reader.ReadInt32();
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            Format = (SurfacePixelFormat)reader.ReadUInt32();
            Length = reader.ReadInt32();
            SourceData = reader.ReadBytes(Length);

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

        ///<inheritdoc/>
        public bool Pack(DatFileWriter writer) {
            writer.WriteUInt32(Id);
            writer.WriteInt32(Unknown);
            writer.WriteInt32(Width);
            writer.WriteInt32(Height);
            writer.WriteUInt32((uint)Format);
            writer.WriteInt32(SourceData.Length);
            writer.WriteBytes(SourceData, SourceData.Length);

            switch (Format) {
                case SurfacePixelFormat.PFID_INDEX16:
                case SurfacePixelFormat.PFID_P8:
                    writer.WriteUInt32(DefaultPaletteId.GetValueOrDefault());
                    break;
            }

            return true;
        }

        ///<inheritdoc/>
        public int GetSize() {
            var size = sizeof(int) * 6;
            size += SourceData.Length;

            switch (Format) {
                case SurfacePixelFormat.PFID_INDEX16:
                case SurfacePixelFormat.PFID_P8:
                    size += sizeof(int);
                    break;
            }

            return size;
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

        //
        // Summary:
        //     Converts the byte array SourceData into color values per pixel
        public List<int> GetImageColorArray() {
            List<int> list = new List<int>();
            if (Length == 0) {
                return list;
            }

            switch (Format) {
                case SurfacePixelFormat.PFID_R8G8B8: {
                        using (BinaryReader binaryReader8 = new BinaryReader(new MemoryStream(SourceData))) {
                            for (uint num16 = 0u; num16 < Height; num16++) {
                                for (uint num17 = 0u; num17 < Width; num17++) {
                                    byte b4 = binaryReader8.ReadByte();
                                    byte b5 = binaryReader8.ReadByte();
                                    byte b6 = binaryReader8.ReadByte();
                                    int item6 = (b6 << 16) | (b5 << 8) | b4;
                                    list.Add(item6);
                                }
                            }
                        }

                        break;
                    }
                case SurfacePixelFormat.PFID_CUSTOM_LSCAPE_R8G8B8: {
                        using (BinaryReader binaryReader7 = new BinaryReader(new MemoryStream(SourceData))) {
                            for (uint num14 = 0u; num14 < Height; num14++) {
                                for (uint num15 = 0u; num15 < Width; num15++) {
                                    byte b = binaryReader7.ReadByte();
                                    byte b2 = binaryReader7.ReadByte();
                                    byte b3 = binaryReader7.ReadByte();
                                    int item5 = (b << 16) | (b2 << 8) | b3;
                                    list.Add(item5);
                                }
                            }
                        }

                        break;
                    }
                case SurfacePixelFormat.PFID_A8R8G8B8: {
                        using (BinaryReader binaryReader6 = new BinaryReader(new MemoryStream(SourceData))) {
                            for (uint num12 = 0u; num12 < Height; num12++) {
                                for (uint num13 = 0u; num13 < Width; num13++) {
                                    list.Add(binaryReader6.ReadInt32());
                                }
                            }
                        }

                        break;
                    }
                case SurfacePixelFormat.PFID_INDEX16: {
                        using (BinaryReader binaryReader5 = new BinaryReader(new MemoryStream(SourceData))) {
                            for (uint num10 = 0u; num10 < Height; num10++) {
                                for (uint num11 = 0u; num11 < Width; num11++) {
                                    list.Add(binaryReader5.ReadInt16());
                                }
                            }
                        }

                        break;
                    }
                case SurfacePixelFormat.PFID_A8:
                case SurfacePixelFormat.PFID_CUSTOM_LSCAPE_ALPHA: {
                        using (BinaryReader binaryReader4 = new BinaryReader(new MemoryStream(SourceData))) {
                            for (uint num8 = 0u; num8 < Height; num8++) {
                                for (uint num9 = 0u; num9 < Width; num9++) {
                                    list.Add(binaryReader4.ReadByte());
                                }
                            }
                        }

                        break;
                    }
                case SurfacePixelFormat.PFID_P8: {
                        using (BinaryReader binaryReader3 = new BinaryReader(new MemoryStream(SourceData))) {
                            for (uint num6 = 0u; num6 < Height; num6++) {
                                for (uint num7 = 0u; num7 < Width; num7++) {
                                    list.Add(binaryReader3.ReadByte());
                                }
                            }
                        }

                        break;
                    }
                case SurfacePixelFormat.PFID_R5G6B5: {
                        using (BinaryReader binaryReader2 = new BinaryReader(new MemoryStream(SourceData))) {
                            for (uint num4 = 0u; num4 < Height; num4++) {
                                for (uint num5 = 0u; num5 < Width; num5++) {
                                    ushort val = binaryReader2.ReadUInt16();
                                    List<int> list2 = get565RGB(val);
                                    list.Add(list2[0]);
                                    list.Add(list2[1]);
                                    list.Add(list2[2]);
                                }
                            }
                        }

                        break;
                    }
                case SurfacePixelFormat.PFID_A4R4G4B4: {
                        using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(SourceData))) {
                            for (uint num = 0u; num < Height; num++) {
                                for (uint num2 = 0u; num2 < Width; num2++) {
                                    ushort num3 = binaryReader.ReadUInt16();
                                    int item = (num3 >> 12) / 15 * 255;
                                    int item2 = ((num3 >> 8) & 0xF) / 15 * 255;
                                    int item3 = ((num3 >> 4) & 0xF) / 15 * 255;
                                    int item4 = (num3 & 0xF) / 15 * 255;
                                    list.Add(item);
                                    list.Add(item2);
                                    list.Add(item3);
                                    list.Add(item4);
                                }
                            }
                        }

                        break;
                    }
                default:
                    Console.WriteLine("Unhandled SurfacePixelFormat (" + Format.ToString() + ") in RenderSurface ");
                    break;
            }

            return list;
        }

        //
        // Summary:
        //     Generates Bitmap data from colorArray.
        private List<int> get565RGB(ushort val) {
            List<int> list = new List<int>();
            int num = 63488;
            int num2 = 2016;
            int num3 = 31;
            int item = (val & num) >> 11 << 3;
            int item2 = (val & num2) >> 5 << 2;
            int item3 = (val & num3) << 3;
            list.Add(item);
            list.Add(item2);
            list.Add(item3);
            return list;
        }
    }
}
