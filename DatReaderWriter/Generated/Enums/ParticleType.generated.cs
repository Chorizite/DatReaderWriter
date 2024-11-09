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
    public enum ParticleType : int {
        Unknown = 0x00000000,

        Still = 0x00000001,

        LocalVelocity = 0x00000002,

        ParabolicLVGA = 0x00000003,

        ParabolicLVGAGR = 0x00000004,

        Swarm = 0x00000005,

        Explode = 0x00000006,

        Implode = 0x00000007,

        ParabolicLVLA = 0x00000008,

        ParabolicLVLALR = 0x00000009,

        ParabolicGVGA = 0x0000000A,

        ParabolicGVGAGR = 0x0000000B,

        GlobalVelocity = 0x0000000C,

        NumParticleType = 0x0000000D,

    };
}
