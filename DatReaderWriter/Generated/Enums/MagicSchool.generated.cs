//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//
//                                                            //
//                          WARNING                           //
//                                                            //
//           DO NOT MAKE LOCAL CHANGES TO THIS FILE           //
//               EDIT THE .tt TEMPLATE INSTEAD                //
//                                                            //
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//


using System;
namespace DatReaderWriter.Enums {
    public enum MagicSchool : int {
        None = 0x00000000,

        WarMagic = 0x00000001,

        LifeMagic = 0x00000002,

        ItemEnchantment = 0x00000003,

        CreatureEnchantment = 0x00000004,

        VoidMagic = 0x00000005,

    };
}
