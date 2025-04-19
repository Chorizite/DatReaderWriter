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
    public enum AttackType : int {
        Undef = 0x00000000,

        Punch = 0x00000001,

        Thrust = 0x00000002,

        Slash = 0x00000004,

        Kick = 0x00000008,

        OffhandPunch = 0x00000010,

        Punches = 0x00000011,

        Unarmed = 0x00000019,

        DoubleSlash = 0x00000020,

        TripleSlash = 0x00000040,

        DoubleThrust = 0x00000080,

        TripleThrust = 0x00000100,

        OffhandThrust = 0x00000200,

        OffhandSlash = 0x00000400,

        OffhandDoubleSlash = 0x00000800,

        OffhandTripleSlash = 0x00001000,

        Slashes = 0x00001C64,

        OffhandDoubleThrust = 0x00002000,

        DoubleStrike = 0x000028A0,

        OffhandTripleThrust = 0x00004000,

        TripleStrike = 0x00005140,

        Thrusts = 0x00006382,

        MultiStrike = 0x000079E0,

        Offhand = 0x00007E00,

    };
}
