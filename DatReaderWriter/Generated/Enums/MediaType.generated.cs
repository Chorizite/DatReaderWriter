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
    public enum MediaType : int {
        Undef = 0x00000000,

        Movie = 0x00000001,

        Alpha = 0x00000002,

        Animation = 0x00000003,

        Cursor = 0x00000004,

        Image = 0x00000005,

        Jump = 0x00000006,

        Message = 0x00000007,

        Pause = 0x00000008,

        Sound = 0x00000009,

        State = 0x0000000A,

        Fade = 0x0000000B,

        Stretch = 0x0000000C,

    };
}
