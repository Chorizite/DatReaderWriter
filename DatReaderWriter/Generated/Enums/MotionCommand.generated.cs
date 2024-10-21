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
    public enum MotionCommand : uint {
        Invalid = 0x00000000,

        Cancel = 0x080000A2,

        CreateShortcutToSelected = 0x080000A9,

        EnterChat = 0x080000B5,

        ToggleChat = 0x080000B6,

        SavePosition = 0x080000B7,

        UseSelected = 0x090000A3,

        AutosortSelected = 0x090000A4,

        DropSelected = 0x090000A5,

        GiveSelected = 0x090000A6,

        SplitSelected = 0x090000A7,

        ExamineSelected = 0x090000A8,

        PreviousCompassItem = 0x090000AA,

        NextCompassItem = 0x090000AB,

        ClosestCompassItem = 0x090000AC,

        PreviousSelection = 0x090000AD,

        LastAttacker = 0x090000AE,

        PreviousFellow = 0x090000AF,

        NextFellow = 0x090000B0,

        ToggleCombat = 0x090000B1,

        OptionsPanel = 0x090000B8,

        ResetView = 0x090000B9,

        FloorView = 0x090000C0,

        PreviousItem = 0x090000C2,

        NextItem = 0x090000C3,

        ClosestItem = 0x090000C4,

        MapView = 0x090000C6,

        AutoRun = 0x090000C7,

        DecreasePowerSetting = 0x090000C8,

        IncreasePowerSetting = 0x090000C9,

        FirstPersonView = 0x090000D5,

        AllegiancePanel = 0x090000D6,

        FellowshipPanel = 0x090000D7,

        SpellbookPanel = 0x090000D8,

        SpellComponentsPanel = 0x090000D9,

        HousePanel = 0x090000DA,

        AttributesPanel = 0x090000DB,

        SkillsPanel = 0x090000DC,

        MapPanel = 0x090000DD,

        InventoryPanel = 0x090000DE,

        CaptureScreenshotToFile = 0x090000E7,

        AutoCreateShortcuts = 0x090000FE,

        AutoRepeatAttacks = 0x090000FF,

        AutoTarget = 0x09000100,

        AdvancedCombatInterface = 0x09000101,

        IgnoreAllegianceRequests = 0x09000102,

        IgnoreFellowshipRequests = 0x09000103,

        InvertMouseLook = 0x09000104,

        LetPlayersGiveYouItems = 0x09000105,

        AutoTrackCombatTargets = 0x09000106,

        DisplayTooltips = 0x09000107,

        AttemptToDeceivePlayers = 0x09000108,

        RunAsDefaultMovement = 0x09000109,

        StayInChatModeAfterSend = 0x0900010A,

        RightClickToMouseLook = 0x0900010B,

        VividTargetIndicator = 0x0900010C,

        SelectSelf = 0x0900010D,

        PreviousMonster = 0x09000110,

        ClosestMonster = 0x09000111,

        NextPlayer = 0x09000112,

        PreviousPlayer = 0x09000113,

        ClosestPlayer = 0x09000114,

        TradePanel = 0x0900011D,

        CharacterOptionsPanel = 0x09000154,

        SoundAndGraphicsPanel = 0x09000155,

        HelpfulSpellsPanel = 0x09000156,

        HarmfulSpellsPanel = 0x09000157,

        CharacterInformationPanel = 0x09000158,

        LinkStatusPanel = 0x09000159,

        VitaePanel = 0x0900015A,

        ShareFellowshipXP = 0x0900015B,

        ShareFellowshipLoot = 0x0900015C,

        AcceptCorpseLooting = 0x0900015D,

        IgnoreTradeRequests = 0x0900015E,

        DisableWeather = 0x0900015F,

        DisableHouseEffect = 0x09000160,

        SideBySideVitals = 0x09000161,

        ShowRadarCoordinates = 0x09000162,

        ShowSpellDurations = 0x09000163,

        MuteOnLosingFocus = 0x09000164,

        AllegianceChat = 0x09000168,

        AutomaticallyAcceptFellowshipRequests = 0x09000169,

        Reply = 0x0900016A,

        MonarchReply = 0x0900016B,

        PatronReply = 0x0900016C,

        ToggleCraftingChanceOfSuccessDialog = 0x0900016D,

        UseClosestUnopenedCorpse = 0x0900016E,

        UseNextUnopenedCorpse = 0x0900016F,

        IssueSlashCommand = 0x09000170,

        MouseLook = 0x0C0000C1,

        HighAttack = 0x0D0000B2,

        MediumAttack = 0x0D0000B3,

        LowAttack = 0x0D0000B4,

        CameraLeftRotate = 0x0D0000BA,

        CameraRightRotate = 0x0D0000BB,

        CameraRaise = 0x0D0000BC,

        CameraLower = 0x0D0000BD,

        CameraCloser = 0x0D0000BE,

        CameraFarther = 0x0D0000BF,

        ShiftView = 0x0D0000C5,

        Hop = 0x1000004A,

        Jumpup = 0x1000004B,

        ChestBeat = 0x1000004D,

        TippedLeft = 0x1000004E,

        TippedRight = 0x1000004F,

        FallDown = 0x10000050,

        Twitch1 = 0x10000051,

        Twitch2 = 0x10000052,

        Twitch3 = 0x10000053,

        Twitch4 = 0x10000054,

        StaggerBackward = 0x10000055,

        StaggerForward = 0x10000056,

        Sanctuary = 0x10000057,

        ThrustMed = 0x10000058,

        ThrustLow = 0x10000059,

        ThrustHigh = 0x1000005A,

        SlashHigh = 0x1000005B,

        SlashMed = 0x1000005C,

        SlashLow = 0x1000005D,

        BackhandHigh = 0x1000005E,

        BackhandMed = 0x1000005F,

        BackhandLow = 0x10000060,

        Shoot = 0x10000061,

        AttackHigh1 = 0x10000062,

        AttackMed1 = 0x10000063,

        AttackLow1 = 0x10000064,

        AttackHigh2 = 0x10000065,

        AttackMed2 = 0x10000066,

        AttackLow2 = 0x10000067,

        AttackHigh3 = 0x10000068,

        AttackMed3 = 0x10000069,

        AttackLow3 = 0x1000006A,

        HeadThrow = 0x1000006B,

        FistSlam = 0x1000006C,

        BreatheFlame = 0x1000006D,

        SpinAttack = 0x1000006E,

        MagicPowerUp01 = 0x1000006F,

        MagicPowerUp02 = 0x10000070,

        MagicPowerUp03 = 0x10000071,

        MagicPowerUp04 = 0x10000072,

        MagicPowerUp05 = 0x10000073,

        MagicPowerUp06 = 0x10000074,

        MagicPowerUp07 = 0x10000075,

        MagicPowerUp08 = 0x10000076,

        MagicPowerUp09 = 0x10000077,

        MagicPowerUp10 = 0x10000078,

        EnterGame = 0x1000009C,

        ExitGame = 0x1000009D,

        OnCreation = 0x1000009E,

        OnDestruction = 0x1000009F,

        EnterPortal = 0x100000A0,

        ExitPortal = 0x100000A1,

        SpecialAttack1 = 0x100000CD,

        SpecialAttack2 = 0x100000CE,

        SpecialAttack3 = 0x100000CF,

        MissileAttack1 = 0x100000D0,

        MissileAttack2 = 0x100000D1,

        MissileAttack3 = 0x100000D2,

        Blink = 0x100000E2,

        Bite = 0x100000E3,

        SkillHealSelf = 0x1000010E,

        SkillHealOther = 0x1000010F,

        LogOut = 0x1000011E,

        DoubleSlashLow = 0x1000011F,

        DoubleSlashMed = 0x10000120,

        DoubleSlashHigh = 0x10000121,

        TripleSlashLow = 0x10000122,

        TripleSlashMed = 0x10000123,

        TripleSlashHigh = 0x10000124,

        DoubleThrustLow = 0x10000125,

        DoubleThrustMed = 0x10000126,

        DoubleThrustHigh = 0x10000127,

        TripleThrustLow = 0x10000128,

        TripleThrustMed = 0x10000129,

        TripleThrustHigh = 0x1000012A,

        MagicPowerUp01Purple = 0x1000012B,

        MagicPowerUp02Purple = 0x1000012C,

        MagicPowerUp03Purple = 0x1000012D,

        MagicPowerUp04Purple = 0x1000012E,

        MagicPowerUp05Purple = 0x1000012F,

        MagicPowerUp06Purple = 0x10000130,

        MagicPowerUp07Purple = 0x10000131,

        MagicPowerUp08Purple = 0x10000132,

        MagicPowerUp09Purple = 0x10000133,

        MagicPowerUp10Purple = 0x10000134,

        HouseRecall = 0x1000013A,

        LifestoneRecall = 0x10000153,

        Fishing = 0x10000165,

        MarketplaceRecall = 0x10000166,

        EnterPKLite = 0x10000167,

        AllegianceHometownRecall = 0x10000171,

        PKArenaRecall = 0x10000172,

        OffhandSlashHigh = 0x10000173,

        OffhandSlashMed = 0x10000174,

        OffhandSlashLow = 0x10000175,

        OffhandThrustHigh = 0x10000176,

        OffhandThrustMed = 0x10000177,

        OffhandThrustLow = 0x10000178,

        OffhandDoubleSlashLow = 0x10000179,

        OffhandDoubleSlashMed = 0x1000017A,

        OffhandDoubleSlashHigh = 0x1000017B,

        OffhandTripleSlashLow = 0x1000017C,

        OffhandTripleSlashMed = 0x1000017D,

        OffhandTripleSlashHigh = 0x1000017E,

        OffhandDoubleThrustLow = 0x1000017F,

        OffhandDoubleThrustMed = 0x10000180,

        OffhandDoubleThrustHigh = 0x10000181,

        OffhandTripleThrustLow = 0x10000182,

        OffhandTripleThrustMed = 0x10000183,

        OffhandTripleThrustHigh = 0x10000184,

        OffhandKick = 0x10000185,

        AttackHigh4 = 0x10000186,

        AttackMed4 = 0x10000187,

        AttackLow4 = 0x10000188,

        AttackHigh5 = 0x10000189,

        AttackMed5 = 0x1000018A,

        AttackLow5 = 0x1000018B,

        AttackHigh6 = 0x1000018C,

        AttackMed6 = 0x1000018D,

        AttackLow6 = 0x1000018E,

        PunchFastHigh = 0x1000018F,

        PunchFastMed = 0x10000190,

        PunchFastLow = 0x10000191,

        PunchSlowHigh = 0x10000192,

        PunchSlowMed = 0x10000193,

        PunchSlowLow = 0x10000194,

        OffhandPunchFastHigh = 0x10000195,

        OffhandPunchFastMed = 0x10000196,

        OffhandPunchFastLow = 0x10000197,

        OffhandPunchSlowHigh = 0x10000198,

        OffhandPunchSlowMed = 0x10000199,

        OffhandPunchSlowLow = 0x1000019A,

        WoahDuplicate2 = 0x1000019B,

        YMCA = 0x1200009B,

        Flatulence = 0x120000D4,

        Demonet = 0x120000DF,

        Cheer = 0x1300004C,

        ShakeFist = 0x13000079,

        Beckon = 0x1300007A,

        BeSeeingYou = 0x1300007B,

        BlowKiss = 0x1300007C,

        BowDeep = 0x1300007D,

        ClapHands = 0x1300007E,

        Cry = 0x1300007F,

        Laugh = 0x13000080,

        MimeEat = 0x13000081,

        MimeDrink = 0x13000082,

        Nod = 0x13000083,

        Point = 0x13000084,

        ShakeHead = 0x13000085,

        Shrug = 0x13000086,

        Wave = 0x13000087,

        Akimbo = 0x13000088,

        HeartyLaugh = 0x13000089,

        Salute = 0x1300008A,

        ScratchHead = 0x1300008B,

        SmackHead = 0x1300008C,

        TapFoot = 0x1300008D,

        WaveHigh = 0x1300008E,

        WaveLow = 0x1300008F,

        YawnStretch = 0x13000090,

        Cringe = 0x13000091,

        Kneel = 0x13000092,

        Plead = 0x13000093,

        Shiver = 0x13000094,

        Shoo = 0x13000095,

        Slouch = 0x13000096,

        Spit = 0x13000097,

        Surrender = 0x13000098,

        Woah = 0x13000099,

        Winded = 0x1300009A,

        Pray = 0x130000CA,

        Mock = 0x130000CB,

        Teapot = 0x130000CC,

        WarmHands = 0x13000119,

        Helper = 0x13000135,

        NudgeLeft = 0x1300014A,

        NudgeRight = 0x1300014B,

        PointLeft = 0x1300014C,

        PointRight = 0x1300014D,

        PointDown = 0x1300014E,

        Knock = 0x1300014F,

        ScanHorizon = 0x13000150,

        DrudgeDance = 0x13000151,

        HaveASeat = 0x13000152,

        StopTurning = 0x2000003A,

        Jump = 0x2500003B,

        Stop = 0x40000004,

        Fallen = 0x40000008,

        Interpolating = 0x40000009,

        Hover = 0x4000000A,

        On = 0x4000000B,

        Off = 0x4000000C,

        Dead = 0x40000011,

        Falling = 0x40000015,

        Reload = 0x40000016,

        Unload = 0x40000017,

        Pickup = 0x40000018,

        StoreInBackpack = 0x40000019,

        Eat = 0x4000001A,

        Drink = 0x4000001B,

        Reading = 0x4000001C,

        JumpCharging = 0x4000001D,

        AimLevel = 0x4000001E,

        AimHigh15 = 0x4000001F,

        AimHigh30 = 0x40000020,

        AimHigh45 = 0x40000021,

        AimHigh60 = 0x40000022,

        AimHigh75 = 0x40000023,

        AimHigh90 = 0x40000024,

        AimLow15 = 0x40000025,

        AimLow30 = 0x40000026,

        AimLow45 = 0x40000027,

        AimLow60 = 0x40000028,

        AimLow75 = 0x40000029,

        AimLow90 = 0x4000002A,

        MagicBlast = 0x4000002B,

        MagicSelfHead = 0x4000002C,

        MagicSelfHeart = 0x4000002D,

        MagicBonus = 0x4000002E,

        MagicClap = 0x4000002F,

        MagicHarm = 0x40000030,

        MagicHeal = 0x40000031,

        MagicThrowMissile = 0x40000032,

        MagicRecoilMissile = 0x40000033,

        MagicPenalty = 0x40000034,

        MagicTransfer = 0x40000035,

        MagicVision = 0x40000036,

        MagicEnchantItem = 0x40000037,

        MagicPortal = 0x40000038,

        MagicPray = 0x40000039,

        CastSpell = 0x400000D3,

        UseMagicStaff = 0x400000E0,

        UseMagicWand = 0x400000E1,

        TwitchSubstate1 = 0x400000E4,

        TwitchSubstate2 = 0x400000E5,

        TwitchSubstate3 = 0x400000E6,

        Pickup5 = 0x40000136,

        Pickup10 = 0x40000137,

        Pickup15 = 0x40000138,

        Pickup20 = 0x40000139,

        Ready = 0x41000003,

        Crouch = 0x41000012,

        Sitting = 0x41000013,

        Sleeping = 0x41000014,

        ATOYOT = 0x420000F9,

        ShakeFistState = 0x430000EA,

        PrayState = 0x430000EB,

        BowDeepState = 0x430000EC,

        ClapHandsState = 0x430000ED,

        CrossArmsState = 0x430000EE,

        ShiverState = 0x430000EF,

        PointState = 0x430000F0,

        WaveState = 0x430000F1,

        AkimboState = 0x430000F2,

        SaluteState = 0x430000F3,

        ScratchHeadState = 0x430000F4,

        TapFootState = 0x430000F5,

        LeanState = 0x430000F6,

        KneelState = 0x430000F7,

        PleadState = 0x430000F8,

        SlouchState = 0x430000FA,

        SurrenderState = 0x430000FB,

        WoahState = 0x430000FC,

        WindedState = 0x430000FD,

        SnowAngelState = 0x43000118,

        CurtseyState = 0x4300011A,

        AFKState = 0x4300011B,

        MeditateState = 0x4300011C,

        SitState = 0x4300013D,

        SitCrossleggedState = 0x4300013E,

        SitBackState = 0x4300013F,

        PointLeftState = 0x43000140,

        PointRightState = 0x43000141,

        TalktotheHandState = 0x43000142,

        PointDownState = 0x43000143,

        DrudgeDanceState = 0x43000144,

        PossumState = 0x43000145,

        ReadState = 0x43000146,

        ThinkerState = 0x43000147,

        HaveASeatState = 0x43000148,

        AtEaseState = 0x43000149,

        RunForward = 0x44000007,

        WalkForward = 0x45000005,

        WalkBackwards = 0x45000006,

        TurnRight = 0x6500000D,

        TurnLeft = 0x6500000E,

        SideStepRight = 0x6500000F,

        SideStepLeft = 0x65000010,

        HandCombat = 0x8000003C,

        NonCombat = 0x8000003D,

        SwordCombat = 0x8000003E,

        BowCombat = 0x8000003F,

        SwordShieldCombat = 0x80000040,

        CrossbowCombat = 0x80000041,

        UnusedCombat = 0x80000042,

        SlingCombat = 0x80000043,

        TwoHandedSwordCombat = 0x80000044,

        TwoHandedStaffCombat = 0x80000045,

        DualWieldCombat = 0x80000046,

        ThrownWeaponCombat = 0x80000047,

        Graze = 0x80000048,

        Magic = 0x80000049,

        BowNoAmmo = 0x800000E8,

        CrossBowNoAmmo = 0x800000E9,

        AtlatlCombat = 0x8000013B,

        ThrownShieldCombat = 0x8000013C,

        HoldRun = 0x85000001,

        HoldSidestep = 0x85000002,

    };
}
