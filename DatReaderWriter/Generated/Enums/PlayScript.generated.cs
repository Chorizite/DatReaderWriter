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
    public enum PlayScript : uint {
        Invalid = 0x00000000,

        Test1 = 0x00000001,

        Test2 = 0x00000002,

        Test3 = 0x00000003,

        Launch = 0x00000004,

        Explode = 0x00000005,

        AttribUpRed = 0x00000006,

        AttribDownRed = 0x00000007,

        AttribUpOrange = 0x00000008,

        AttribDownOrange = 0x00000009,

        AttribUpYellow = 0x0000000A,

        AttribDownYellow = 0x0000000B,

        AttribUpGreen = 0x0000000C,

        AttribDownGreen = 0x0000000D,

        AttribUpBlue = 0x0000000E,

        AttribDownBlue = 0x0000000F,

        AttribUpPurple = 0x00000010,

        AttribDownPurple = 0x00000011,

        SkillUpRed = 0x00000012,

        SkillDownRed = 0x00000013,

        SkillUpOrange = 0x00000014,

        SkillDownOrange = 0x00000015,

        SkillUpYellow = 0x00000016,

        SkillDownYellow = 0x00000017,

        SkillUpGreen = 0x00000018,

        SkillDownGreen = 0x00000019,

        SkillUpBlue = 0x0000001A,

        SkillDownBlue = 0x0000001B,

        SkillUpPurple = 0x0000001C,

        SkillDownPurple = 0x0000001D,

        SkillDownBlack = 0x0000001E,

        HealthUpRed = 0x0000001F,

        HealthDownRed = 0x00000020,

        HealthUpBlue = 0x00000021,

        HealthDownBlue = 0x00000022,

        HealthUpYellow = 0x00000023,

        HealthDownYellow = 0x00000024,

        RegenUpRed = 0x00000025,

        RegenDownREd = 0x00000026,

        RegenUpBlue = 0x00000027,

        RegenDownBlue = 0x00000028,

        RegenUpYellow = 0x00000029,

        RegenDownYellow = 0x0000002A,

        ShieldUpRed = 0x0000002B,

        ShieldDownRed = 0x0000002C,

        ShieldUpOrange = 0x0000002D,

        ShieldDownOrange = 0x0000002E,

        ShieldUpYellow = 0x0000002F,

        ShieldDownYellow = 0x00000030,

        ShieldUpGreen = 0x00000031,

        ShieldDownGreen = 0x00000032,

        ShieldUpBlue = 0x00000033,

        ShieldDownBlue = 0x00000034,

        ShieldUpPurple = 0x00000035,

        ShieldDownPurple = 0x00000036,

        ShieldUpGrey = 0x00000037,

        ShieldDownGrey = 0x00000038,

        EnchantUpRed = 0x00000039,

        EnchantDownRed = 0x0000003A,

        EnchantUpOrange = 0x0000003B,

        EnchantDownOrange = 0x0000003C,

        EnchantUpYellow = 0x0000003D,

        EnchantDownYellow = 0x0000003E,

        EnchantUpGreen = 0x0000003F,

        EnchantDownGreen = 0x00000040,

        EnchantUpBlue = 0x00000041,

        EnchantDownBlue = 0x00000042,

        EnchantUpPurple = 0x00000043,

        EnchantDownPurple = 0x00000044,

        VitaeUpWhite = 0x00000045,

        VitaeDownBlack = 0x00000046,

        VisionUpWhite = 0x00000047,

        VisionDownBlack = 0x00000048,

        SwapHealth_Red_To_Yellow = 0x00000049,

        SwapHealth_Red_To_Blue = 0x0000004A,

        SwapHealth_Yellow_To_Red = 0x0000004B,

        SwapHealth_Yellow_To_Blue = 0x0000004C,

        SwapHealth_Blue_To_Red = 0x0000004D,

        SwapHealth_Blue_To_Yellow = 0x0000004E,

        TransUpWhite = 0x0000004F,

        TransDownBlack = 0x00000050,

        Fizzle = 0x00000051,

        PortalEntry = 0x00000052,

        PortalExit = 0x00000053,

        BreatheFlame = 0x00000054,

        BreatheFrost = 0x00000055,

        BreatheAcid = 0x00000056,

        BreatheLightning = 0x00000057,

        Create = 0x00000058,

        Destroy = 0x00000059,

        ProjectileCollision = 0x0000005A,

        SplatterLowLeftBack = 0x0000005B,

        SplatterLowLeftFront = 0x0000005C,

        SplatterLowRightBack = 0x0000005D,

        SplatterLowRightFront = 0x0000005E,

        SplatterMidLeftBack = 0x0000005F,

        SplatterMidLeftFront = 0x00000060,

        SplatterMidRightBack = 0x00000061,

        SplatterMidRightFront = 0x00000062,

        SplatterUpLeftBack = 0x00000063,

        SplatterUpLeftFront = 0x00000064,

        SplatterUpRightBack = 0x00000065,

        SplatterUpRightFront = 0x00000066,

        SparkLowLeftBack = 0x00000067,

        SparkLowLeftFront = 0x00000068,

        SparkLowRightBack = 0x00000069,

        SparkLowRightFront = 0x0000006A,

        SparkMidLeftBack = 0x0000006B,

        SparkMidLeftFront = 0x0000006C,

        SparkMidRightBack = 0x0000006D,

        SparkMidRightFront = 0x0000006E,

        SparkUpLeftBack = 0x0000006F,

        SparkUpLeftFront = 0x00000070,

        SparkUpRightBack = 0x00000071,

        SparkUpRightFront = 0x00000072,

        PortalStorm = 0x00000073,

        Hide = 0x00000074,

        UnHide = 0x00000075,

        Hidden = 0x00000076,

        DisappearDestroy = 0x00000077,

        SpecialState1 = 0x00000078,

        SpecialState2 = 0x00000079,

        SpecialState3 = 0x0000007A,

        SpecialState4 = 0x0000007B,

        SpecialState5 = 0x0000007C,

        SpecialState6 = 0x0000007D,

        SpecialState7 = 0x0000007E,

        SpecialState8 = 0x0000007F,

        SpecialState9 = 0x00000080,

        SpecialState0 = 0x00000081,

        SpecialStateRed = 0x00000082,

        SpecialStateOrange = 0x00000083,

        SpecialStateYellow = 0x00000084,

        SpecialStateGreen = 0x00000085,

        SpecialStateBlue = 0x00000086,

        SpecialStatePurple = 0x00000087,

        SpecialStateWhite = 0x00000088,

        SpecialStateBlack = 0x00000089,

        LevelUp = 0x0000008A,

        EnchantUpGrey = 0x0000008B,

        EnchantDownGrey = 0x0000008C,

        WeddingBliss = 0x0000008D,

        EnchantUpWhite = 0x0000008E,

        EnchantDownWhite = 0x0000008F,

        CampingMastery = 0x00000090,

        CampingIneptitude = 0x00000091,

        DispelLife = 0x00000092,

        DispelCreature = 0x00000093,

        DispelAll = 0x00000094,

        BunnySmite = 0x00000095,

        BaelZharonSmite = 0x00000096,

        WeddingSteele = 0x00000097,

        RestrictionEffectBlue = 0x00000098,

        RestrictionEffectGreen = 0x00000099,

        RestrictionEffectGold = 0x0000009A,

        LayingofHands = 0x0000009B,

        AugmentationUseAttribute = 0x0000009C,

        AugmentationUseSkill = 0x0000009D,

        AugmentationUseResistances = 0x0000009E,

        AugmentationUseOther = 0x0000009F,

        BlackMadness = 0x000000A0,

        AetheriaLevelUp = 0x000000A1,

        AetheriaSurgeDestruction = 0x000000A2,

        AetheriaSurgeProtection = 0x000000A3,

        AetheriaSurgeRegeneration = 0x000000A4,

        AetheriaSurgeAffliction = 0x000000A5,

        AetheriaSurgeFestering = 0x000000A6,

        HealthDownVoid = 0x000000A7,

        RegenDownVoid = 0x000000A8,

        SkillDownVoid = 0x000000A9,

        DirtyFightingHealDebuff = 0x000000AA,

        DirtyFightingAttackDebuff = 0x000000AB,

        DirtyFightingDefenseDebuff = 0x000000AC,

        DirtyFightingDamageOverTime = 0x000000AD,

    };
}
