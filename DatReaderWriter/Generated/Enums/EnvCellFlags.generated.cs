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
    [Flags]
    public enum EnvCellFlags : int {
        SeenOutside = 0x00000001,

        HasStaticObjs = 0x00000002,

        HasRestrictionObj = 0x00000008,

    };
}
