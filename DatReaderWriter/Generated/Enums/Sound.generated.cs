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
    public enum Sound : uint {
        Invalid = 0x00000000,

        Speak1 = 0x00000001,

        Random = 0x00000002,

        Attack1 = 0x00000003,

        Attack2 = 0x00000004,

        Attack3 = 0x00000005,

        SpecialAttack1 = 0x00000006,

        SpecialAttack2 = 0x00000007,

        SpecialAttack3 = 0x00000008,

        Damage1 = 0x00000009,

        Damage2 = 0x0000000A,

        Damage3 = 0x0000000B,

        Wound1 = 0x0000000C,

        Wound2 = 0x0000000D,

        Wound3 = 0x0000000E,

        Death1 = 0x0000000F,

        Death2 = 0x00000010,

        Death3 = 0x00000011,

        Grunt1 = 0x00000012,

        Grunt2 = 0x00000013,

        Grunt3 = 0x00000014,

        Oh1 = 0x00000015,

        Oh2 = 0x00000016,

        Oh3 = 0x00000017,

        Heave1 = 0x00000018,

        Heave2 = 0x00000019,

        Heave3 = 0x0000001A,

        Knockdown1 = 0x0000001B,

        Knockdown2 = 0x0000001C,

        Knockdown3 = 0x0000001D,

        Swoosh1 = 0x0000001E,

        Swoosh2 = 0x0000001F,

        Swoosh3 = 0x00000020,

        Thump1 = 0x00000021,

        Smash1 = 0x00000022,

        Scratch1 = 0x00000023,

        Spear = 0x00000024,

        Sling = 0x00000025,

        Dagger = 0x00000026,

        ArrowWhiz1 = 0x00000027,

        ArrowWhiz2 = 0x00000028,

        CrossbowPull = 0x00000029,

        CrossbowRelease = 0x0000002A,

        BowPull = 0x0000002B,

        BowRelease = 0x0000002C,

        ThrownWeaponRelease1 = 0x0000002D,

        ArrowLand = 0x0000002E,

        Collision = 0x0000002F,

        HitFlesh1 = 0x00000030,

        HitLeather1 = 0x00000031,

        HitChain1 = 0x00000032,

        HitPlate1 = 0x00000033,

        HitMissile1 = 0x00000034,

        HitMissile2 = 0x00000035,

        HitMissile3 = 0x00000036,

        Footstep1 = 0x00000037,

        Footstep2 = 0x00000038,

        Walk1 = 0x00000039,

        Dance1 = 0x0000003A,

        Dance2 = 0x0000003B,

        Dance3 = 0x0000003C,

        Hidden1 = 0x0000003D,

        Hidden2 = 0x0000003E,

        Hidden3 = 0x0000003F,

        Eat1 = 0x00000040,

        Drink1 = 0x00000041,

        Open = 0x00000042,

        Close = 0x00000043,

        OpenSlam = 0x00000044,

        CloseSlam = 0x00000045,

        Ambient1 = 0x00000046,

        Ambient2 = 0x00000047,

        Ambient3 = 0x00000048,

        Ambient4 = 0x00000049,

        Ambient5 = 0x0000004A,

        Ambient6 = 0x0000004B,

        Ambient7 = 0x0000004C,

        Ambient8 = 0x0000004D,

        Waterfall = 0x0000004E,

        LogOut = 0x0000004F,

        LogIn = 0x00000050,

        LifestoneOn = 0x00000051,

        AttribUp = 0x00000052,

        AttribDown = 0x00000053,

        SkillUp = 0x00000054,

        SkillDown = 0x00000055,

        HealthUp = 0x00000056,

        HealthDown = 0x00000057,

        ShieldUp = 0x00000058,

        ShieldDown = 0x00000059,

        EnchantUp = 0x0000005A,

        EnchantDown = 0x0000005B,

        VisionUp = 0x0000005C,

        VisionDown = 0x0000005D,

        Fizzle = 0x0000005E,

        Launch = 0x0000005F,

        Explode = 0x00000060,

        TransUp = 0x00000061,

        TransDown = 0x00000062,

        BreatheFlaem = 0x00000063,

        BreatheAcid = 0x00000064,

        BreatheFrost = 0x00000065,

        BreatheLightning = 0x00000066,

        Create = 0x00000067,

        Destroy = 0x00000068,

        Lockpicking = 0x00000069,

        UI_EnterPortal = 0x0000006A,

        UI_ExitPortal = 0x0000006B,

        UI_GeneralQuery = 0x0000006C,

        UI_GeneralError = 0x0000006D,

        UI_TransientMessage = 0x0000006E,

        UI_IconPickUp = 0x0000006F,

        UI_IconSuccessfulDrop = 0x00000070,

        UI_IconInvalid_Drop = 0x00000071,

        UI_ButtonPress = 0x00000072,

        UI_GrabSlider = 0x00000073,

        UI_ReleaseSlider = 0x00000074,

        UI_NewTargetSelected = 0x00000075,

        UI_Roar = 0x00000076,

        UI_Bell = 0x00000077,

        UI_Chant1 = 0x00000078,

        UI_Chant2 = 0x00000079,

        UI_DarkWhispers1 = 0x0000007A,

        UI_DarkWhispers2 = 0x0000007B,

        UI_DarkLaugh = 0x0000007C,

        UI_DarkWind = 0x0000007D,

        UI_DarkSpeech = 0x0000007E,

        UI_Drums = 0x0000007F,

        UI_GhostSpeak = 0x00000080,

        UI_Breathing = 0x00000081,

        UI_Howl = 0x00000082,

        UI_LostSouls = 0x00000083,

        UI_Squeal = 0x00000084,

        UI_Thunder1 = 0x00000085,

        UI_Thunder2 = 0x00000086,

        UI_Thunder3 = 0x00000087,

        UI_Thunder4 = 0x00000088,

        UI_Thunder5 = 0x00000089,

        UI_Thunder6 = 0x0000008A,

        RaiseTrait = 0x0000008B,

        WieldObject = 0x0000008C,

        UnwieldObject = 0x0000008D,

        ReceiveItem = 0x0000008E,

        PickUpItem = 0x0000008F,

        DropItem = 0x00000090,

        ResistSpell = 0x00000091,

        PicklockFail = 0x00000092,

        LockSuccess = 0x00000093,

        OpenFailDueToLock = 0x00000094,

        TriggerActivated = 0x00000095,

        SpellExpire = 0x00000096,

        ItemManaDepleted = 0x00000097,

        TriggerActivated1 = 0x00000098,

        TriggerActivated2 = 0x00000099,

        TriggerActivated3 = 0x0000009A,

        TriggerActivated4 = 0x0000009B,

        TriggerActivated5 = 0x0000009C,

        TriggerActivated6 = 0x0000009D,

        TriggerActivated7 = 0x0000009E,

        TriggerActivated8 = 0x0000009F,

        TriggerActivated9 = 0x000000A0,

        TriggerActivated10 = 0x000000A1,

        TriggerActivated11 = 0x000000A2,

        TriggerActivated12 = 0x000000A3,

        TriggerActivated13 = 0x000000A4,

        TriggerActivated14 = 0x000000A5,

        TriggerActivated15 = 0x000000A6,

        TriggerActivated16 = 0x000000A7,

        TriggerActivated17 = 0x000000A8,

        TriggerActivated18 = 0x000000A9,

        TriggerActivated19 = 0x000000AA,

        TriggerActivated20 = 0x000000AB,

        TriggerActivated21 = 0x000000AC,

        TriggerActivated22 = 0x000000AD,

        TriggerActivated23 = 0x000000AE,

        TriggerActivated24 = 0x000000AF,

        TriggerActivated25 = 0x000000B0,

        TriggerActivated26 = 0x000000B1,

        TriggerActivated27 = 0x000000B2,

        TriggerActivated28 = 0x000000B3,

        TriggerActivated29 = 0x000000B4,

        TriggerActivated30 = 0x000000B5,

        TriggerActivated31 = 0x000000B6,

        TriggerActivated32 = 0x000000B7,

        TriggerActivated33 = 0x000000B8,

        TriggerActivated34 = 0x000000B9,

        TriggerActivated35 = 0x000000BA,

        TriggerActivated36 = 0x000000BB,

        TriggerActivated37 = 0x000000BC,

        TriggerActivated38 = 0x000000BD,

        TriggerActivated39 = 0x000000BE,

        TriggerActivated40 = 0x000000BF,

        TriggerActivated41 = 0x000000C0,

        TriggerActivated42 = 0x000000C1,

        TriggerActivated43 = 0x000000C2,

        TriggerActivated44 = 0x000000C3,

        TriggerActivated45 = 0x000000C4,

        TriggerActivated46 = 0x000000C5,

        TriggerActivated47 = 0x000000C6,

        TriggerActivated48 = 0x000000C7,

        TriggerActivated49 = 0x000000C8,

        TriggerActivated50 = 0x000000C9,

        HealthDownVoid = 0x000000CA,

        RegenDownVoid = 0x000000CB,

        SkillDownVoid = 0x000000CC,

    };
}
