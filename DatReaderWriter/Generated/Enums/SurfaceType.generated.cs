//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//
//                                                            //
//                          WARNING                           //
//                                                            //
//           DO NOT MAKE LOCAL CHANGES TO THIS FILE           //
//               EDIT THE .tt TEMPLATE INSTEAD                //
//                                                            //
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//


using System;
namespace ACClientLib.DatReaderWriter.Enums {
    [Flags]
    public enum SurfaceType : uint {
        Base1Solid = 0x00000001,

        Base1Image = 0x00000002,

        Base1ClipMap = 0x00000004,

        Translucent = 0x00000010,

        Diffuse = 0x00000020,

        Luminous = 0x00000040,

        Alpha = 0x00000100,

        InvAlpha = 0x00000200,

        Additive = 0x00010000,

        Detail = 0x00020000,

        Gouraud = 0x10000000,

        Stippled = 0x40000000,

        Perspective = 0x80000000,

    };
}
