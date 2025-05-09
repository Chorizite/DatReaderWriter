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
    public enum IncorporationFlags : uint {
        None = 0x0,

        PassToChildren = 0x1,

        X = 0x2,

        Y = 0x4,

        Width = 0x8,

        Height = 0x10,

        ZLevel = 0x20,

    };
}
