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
    public enum GfxObjFlags : uint {
        HasPhysics = 0x00000001,

        HasDrawing = 0x00000002,

        Unknown = 0x00000004,

        HasDIDDegrade = 0x00000008,

    };
}
