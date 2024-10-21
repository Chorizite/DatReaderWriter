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
    public enum ParentLocation : int {
        None = 0x00000000,

        RightHand = 0x00000001,

        LeftHand = 0x00000002,

        Shield = 0x00000003,

        Belt = 0x00000004,

        Quiver = 0x00000005,

        Hearldry = 0x00000006,

        Mouth = 0x00000007,

        LeftWeapon = 0x00000008,

        LeftUnarmed = 0x00000009,

    };
}
