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
    public enum SetupFlags : uint {
        HasParent = 0x00000001,

        HasDefaultScale = 0x00000002,

        AllowFreeHeading = 0x00000004,

        HasPhysicsBSP = 0x00000008,

    };
}
