using ACClientLib.DatReaderWriter.Enums;

namespace ACClientLib.DatReaderWriter.IO {
    /// <summary>
    /// A file stored in the dat
    /// </summary>
    public interface IDBObj : IUnpackable, IPackable {
        /// <summary>
        /// Determines what header fields are present and will be packed/unpacked.
        /// </summary>
        DBObjHeaderFlags HeaderFlags { get; }

        /// <summary>
        /// The id of this texture
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// Data Category.These are unknown but only used for SurfaceTexture / RenderSurface / RenderTexture types.
        /// </summary>
        public uint DataCategory { get; set; }
    }
}