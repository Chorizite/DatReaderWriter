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
    public enum AnimationHookType : uint {
        NoOp = 0x00000000,

        Sound = 0x00000001,

        SoundTable = 0x00000002,

        Attack = 0x00000003,

        AnimationDone = 0x00000004,

        ReplaceObject = 0x00000005,

        Ethereal = 0x00000006,

        TransparentPart = 0x00000007,

        Luminous = 0x00000008,

        LuminousPart = 0x00000009,

        Diffuse = 0x0000000A,

        DiffusePart = 0x0000000B,

        Scale = 0x0000000C,

        CreateParticle = 0x0000000D,

        DestroyParticle = 0x0000000E,

        StopParticle = 0x0000000F,

        NoDraw = 0x00000010,

        DefaultScript = 0x00000011,

        DefaultScriptPart = 0x00000012,

        CallPES = 0x00000013,

        Transparent = 0x00000014,

        SoundTweaked = 0x00000015,

        SetOmega = 0x00000016,

        TextureVelocity = 0x00000017,

        TextureVelocityPart = 0x00000018,

        SetLight = 0x00000019,

        CreateBlockingParticle = 0x0000001A,

        Unknown = 0xFFFFFFFF,

    };
}
