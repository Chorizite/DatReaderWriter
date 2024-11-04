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
    public enum SpellCategory : uint {
        Undef = 0x00000000,

        StrengthRaising = 0x00000001,

        StrengthLowering = 0x00000002,

        EnduranceRaising = 0x00000003,

        EnduranceLowering = 0x00000004,

        QuicknessRaising = 0x00000005,

        QuicknessLowering = 0x00000006,

        CoordinationRaising = 0x00000007,

        CoordinationLowering = 0x00000008,

        FocusRaising = 0x00000009,

        FocusLowering = 0x0000000A,

        SelfRaising = 0x0000000B,

        SelfLowering = 0x0000000C,

        FocusConcentration = 0x0000000D,

        FocusDisruption = 0x0000000E,

        FocusBrilliance = 0x0000000F,

        FocusDullness = 0x00000010,

        AxeRaising = 0x00000011,

        AxeLowering = 0x00000012,

        BowRaising = 0x00000013,

        BowLowering = 0x00000014,

        CrossbowRaising = 0x00000015,

        CrossbowLowering = 0x00000016,

        DaggerRaising = 0x00000017,

        DaggerLowering = 0x00000018,

        MaceRaising = 0x00000019,

        MaceLowering = 0x0000001A,

        SpearRaising = 0x0000001B,

        SpearLowering = 0x0000001C,

        StaffRaising = 0x0000001D,

        StaffLowering = 0x0000001E,

        SwordRaising = 0x0000001F,

        SwordLowering = 0x00000020,

        ThrownWeaponsRaising = 0x00000021,

        ThrownWeaponsLowering = 0x00000022,

        UnarmedCombatRaising = 0x00000023,

        UnarmedCombatLowering = 0x00000024,

        MeleeDefenseRaising = 0x00000025,

        MeleeDefenseLowering = 0x00000026,

        MissileDefenseRaising = 0x00000027,

        MissileDefenseLowering = 0x00000028,

        MagicDefenseRaising = 0x00000029,

        MagicDefenseLowering = 0x0000002A,

        CreatureEnchantmentRaising = 0x0000002B,

        CreatureEnchantmentLowering = 0x0000002C,

        ItemEnchantmentRaising = 0x0000002D,

        ItemEnchantmentLowering = 0x0000002E,

        LifeMagicRaising = 0x0000002F,

        LifeMagicLowering = 0x00000030,

        WarMagicRaising = 0x00000031,

        WarMagicLowering = 0x00000032,

        ManaConversionRaising = 0x00000033,

        ManaConversionLowering = 0x00000034,

        ArcaneLoreRaising = 0x00000035,

        ArcaneLoreLowering = 0x00000036,

        AppraiseArmorRaising = 0x00000037,

        AppraiseArmorLowering = 0x00000038,

        AppraiseItemRaising = 0x00000039,

        AppraiseItemLowering = 0x0000003A,

        AppraiseMagicItemRaising = 0x0000003B,

        AppraiseMagicItemLowering = 0x0000003C,

        AppraiseWeaponRaising = 0x0000003D,

        AppraiseWeaponLowering = 0x0000003E,

        AssessMonsterRaising = 0x0000003F,

        AssessMonsterLowering = 0x00000040,

        DeceptionRaising = 0x00000041,

        DeceptionLowering = 0x00000042,

        HealingRaising = 0x00000043,

        HealingLowering = 0x00000044,

        JumpRaising = 0x00000045,

        JumpLowering = 0x00000046,

        LeadershipRaising = 0x00000047,

        LeadershipLowering = 0x00000048,

        LockpickRaising = 0x00000049,

        LockpickLowering = 0x0000004A,

        LoyaltyRaising = 0x0000004B,

        LoyaltyLowering = 0x0000004C,

        RunRaising = 0x0000004D,

        RunLowering = 0x0000004E,

        HealthRaising = 0x0000004F,

        HealthLowering = 0x00000050,

        StaminaRaising = 0x00000051,

        StaminaLowering = 0x00000052,

        ManaRaising = 0x00000053,

        ManaLowering = 0x00000054,

        ManaRemedy = 0x00000055,

        ManaMalediction = 0x00000056,

        HealthTransfertocaster = 0x00000057,

        HealthTransferfromcaster = 0x00000058,

        StaminaTransfertocaster = 0x00000059,

        StaminaTransferfromcaster = 0x0000005A,

        ManaTransfertocaster = 0x0000005B,

        ManaTransferfromcaster = 0x0000005C,

        HealthAccelerating = 0x0000005D,

        HealthDecelerating = 0x0000005E,

        StaminaAccelerating = 0x0000005F,

        StaminaDecelerating = 0x00000060,

        ManaAccelerating = 0x00000061,

        ManaDecelerating = 0x00000062,

        VitaeRaising = 0x00000063,

        VitaeLowering = 0x00000064,

        AcidProtection = 0x00000065,

        AcidVulnerability = 0x00000066,

        BludgeonProtection = 0x00000067,

        BludgeonVulnerability = 0x00000068,

        ColdProtection = 0x00000069,

        ColdVulnerability = 0x0000006A,

        ElectricProtection = 0x0000006B,

        ElectricVulnerability = 0x0000006C,

        FireProtection = 0x0000006D,

        FireVulnerability = 0x0000006E,

        PierceProtection = 0x0000006F,

        PierceVulnerability = 0x00000070,

        SlashProtection = 0x00000071,

        SlashVulnerability = 0x00000072,

        ArmorRaising = 0x00000073,

        ArmorLowering = 0x00000074,

        AcidMissile = 0x00000075,

        BludgeoningMissile = 0x00000076,

        ColdMissile = 0x00000077,

        ElectricMissile = 0x00000078,

        FireMissile = 0x00000079,

        PiercingMissile = 0x0000007A,

        SlashingMissile = 0x0000007B,

        AcidSeeker = 0x0000007C,

        BludgeoningSeeker = 0x0000007D,

        ColdSeeker = 0x0000007E,

        ElectricSeeker = 0x0000007F,

        FireSeeker = 0x00000080,

        PiercingSeeker = 0x00000081,

        SlashingSeeker = 0x00000082,

        AcidBurst = 0x00000083,

        BludgeoningBurst = 0x00000084,

        ColdBurst = 0x00000085,

        ElectricBurst = 0x00000086,

        FireBurst = 0x00000087,

        PiercingBurst = 0x00000088,

        SlashingBurst = 0x00000089,

        AcidBlast = 0x0000008A,

        BludgeoningBlast = 0x0000008B,

        ColdBlast = 0x0000008C,

        ElectricBlast = 0x0000008D,

        FireBlast = 0x0000008E,

        PiercingBlast = 0x0000008F,

        SlashingBlast = 0x00000090,

        AcidScatter = 0x00000091,

        BludgeoningScatter = 0x00000092,

        ColdScatter = 0x00000093,

        ElectricScatter = 0x00000094,

        FireScatter = 0x00000095,

        PiercingScatter = 0x00000096,

        SlashingScatter = 0x00000097,

        AttackModRaising = 0x00000098,

        AttackModLowering = 0x00000099,

        DamageRaising = 0x0000009A,

        DamageLowering = 0x0000009B,

        DefenseModRaising = 0x0000009C,

        DefenseModLowering = 0x0000009D,

        WeaponTimeRaising = 0x0000009E,

        WeaponTimeLowering = 0x0000009F,

        ArmorValueRaising = 0x000000A0,

        ArmorValueLowering = 0x000000A1,

        AcidResistanceRaising = 0x000000A2,

        AcidResistanceLowering = 0x000000A3,

        BludgeonResistanceRaising = 0x000000A4,

        BludgeonResistanceLowering = 0x000000A5,

        ColdResistanceRaising = 0x000000A6,

        ColdResistanceLowering = 0x000000A7,

        ElectricResistanceRaising = 0x000000A8,

        ElectricResistanceLowering = 0x000000A9,

        FireResistanceRaising = 0x000000AA,

        FireResistanceLowering = 0x000000AB,

        PierceResistanceRaising = 0x000000AC,

        PierceResistanceLowering = 0x000000AD,

        SlashResistanceRaising = 0x000000AE,

        SlashResistanceLowering = 0x000000AF,

        BludgeoningResistanceRaising = 0x000000B0,

        BludgeoningResistanceLowering = 0x000000B1,

        SlashingResistanceRaising = 0x000000B2,

        SlashingResistanceLowering = 0x000000B3,

        PiercingResistanceRaising = 0x000000B4,

        PiercingResistanceLowering = 0x000000B5,

        ElectricalResistanceRaising = 0x000000B6,

        ElectricalResistanceLowering = 0x000000B7,

        FrostResistanceRaising = 0x000000B8,

        FrostResistanceLowering = 0x000000B9,

        FlameResistanceRaising = 0x000000BA,

        FlameResistanceLowering = 0x000000BB,

        AcidicResistanceRaising = 0x000000BC,

        AcidicResistanceLowering = 0x000000BD,

        ArmorLevelRaising = 0x000000BE,

        ArmorLevelLowering = 0x000000BF,

        LockpickResistanceRaising = 0x000000C0,

        LockpickResistanceLowering = 0x000000C1,

        ManaConversionModLowering = 0x000000C2,

        ManaConversionModRaising = 0x000000C3,

        VisionRaising = 0x000000C4,

        VisionLowering = 0x000000C5,

        TransparencyRaising = 0x000000C6,

        TransparencyLowering = 0x000000C7,

        PortalTie = 0x000000C8,

        PortalRecall = 0x000000C9,

        PortalCreation = 0x000000CA,

        PortalItemCreation = 0x000000CB,

        Vitae = 0x000000CC,

        AssessPersonRaising = 0x000000CD,

        AssessPersonLowering = 0x000000CE,

        AcidVolley = 0x000000CF,

        BludgeoningVolley = 0x000000D0,

        FrostVolley = 0x000000D1,

        LightningVolley = 0x000000D2,

        FlameVolley = 0x000000D3,

        ForceVolley = 0x000000D4,

        BladeVolley = 0x000000D5,

        PortalSending = 0x000000D6,

        LifestoneSending = 0x000000D7,

        CookingRaising = 0x000000D8,

        CookingLowering = 0x000000D9,

        FletchingRaising = 0x000000DA,

        FletchingLowering = 0x000000DB,

        AlchemyLowering = 0x000000DC,

        AlchemyRaising = 0x000000DD,

        AcidRing = 0x000000DE,

        BludgeoningRing = 0x000000DF,

        ColdRing = 0x000000E0,

        ElectricRing = 0x000000E1,

        FireRing = 0x000000E2,

        PiercingRing = 0x000000E3,

        SlashingRing = 0x000000E4,

        AcidWall = 0x000000E5,

        BludgeoningWall = 0x000000E6,

        ColdWall = 0x000000E7,

        ElectricWall = 0x000000E8,

        FireWall = 0x000000E9,

        PiercingWall = 0x000000EA,

        SlashingWall = 0x000000EB,

        AcidStrike = 0x000000EC,

        BludgeoningStrike = 0x000000ED,

        ColdStrike = 0x000000EE,

        ElectricStrike = 0x000000EF,

        FireStrike = 0x000000F0,

        PiercingStrike = 0x000000F1,

        SlashingStrike = 0x000000F2,

        AcidStreak = 0x000000F3,

        BludgeoningStreak = 0x000000F4,

        ColdStreak = 0x000000F5,

        ElectricStreak = 0x000000F6,

        FireStreak = 0x000000F7,

        PiercingStreak = 0x000000F8,

        SlashingStreak = 0x000000F9,

        Dispel = 0x000000FA,

        CreatureMysticRaising = 0x000000FB,

        CreatureMysticLowering = 0x000000FC,

        ItemMysticRaising = 0x000000FD,

        ItemMysticLowering = 0x000000FE,

        WarMysticRaising = 0x000000FF,

        WarMysticLowering = 0x00000100,

        HealthRestoring = 0x00000101,

        HealthDepleting = 0x00000102,

        ManaRestoring = 0x00000103,

        ManaDepleting = 0x00000104,

        StrengthIncrease = 0x00000105,

        StrengthDecrease = 0x00000106,

        EnduranceIncrease = 0x00000107,

        EnduranceDecrease = 0x00000108,

        QuicknessIncrease = 0x00000109,

        QuicknessDecrease = 0x0000010A,

        CoordinationIncrease = 0x0000010B,

        CoordinationDecrease = 0x0000010C,

        FocusIncrease = 0x0000010D,

        FocusDecrease = 0x0000010E,

        SelfIncrease = 0x0000010F,

        SelfDecrease = 0x00000110,

        GreatVitalityRaising = 0x00000111,

        PoorVitalityLowering = 0x00000112,

        GreatVigorRaising = 0x00000113,

        PoorVigorLowering = 0x00000114,

        GreaterIntellectRaising = 0x00000115,

        LessorIntellectLowering = 0x00000116,

        LifeGiverRaising = 0x00000117,

        LifeTakerLowering = 0x00000118,

        StaminaGiverRaising = 0x00000119,

        StaminaTakerLowering = 0x0000011A,

        ManaGiverRaising = 0x0000011B,

        ManaTakerLowering = 0x0000011C,

        AcidWardProtection = 0x0000011D,

        AcidWardVulnerability = 0x0000011E,

        FireWardProtection = 0x0000011F,

        FireWardVulnerability = 0x00000120,

        ColdWardProtection = 0x00000121,

        ColdWardVulnerability = 0x00000122,

        ElectricWardProtection = 0x00000123,

        ElectricWardVulnerability = 0x00000124,

        LeadershipObedienceRaising = 0x00000125,

        LeadershipObedienceLowering = 0x00000126,

        MeleeDefenseShelterRaising = 0x00000127,

        MeleeDefenseShelterLowering = 0x00000128,

        MissileDefenseShelterRaising = 0x00000129,

        MissileDefenseShelterLowering = 0x0000012A,

        MagicDefenseShelterRaising = 0x0000012B,

        MagicDefenseShelterLowering = 0x0000012C,

        HuntersAcumenRaising = 0x0000012D,

        HuntersAcumenLowering = 0x0000012E,

        StillWaterRaising = 0x0000012F,

        StillWaterLowering = 0x00000130,

        StrengthofEarthRaising = 0x00000131,

        StrengthofEarthLowering = 0x00000132,

        TorrentRaising = 0x00000133,

        TorrentLowering = 0x00000134,

        GrowthRaising = 0x00000135,

        GrowthLowering = 0x00000136,

        CascadeAxeRaising = 0x00000137,

        CascadeAxeLowering = 0x00000138,

        CascadeDaggerRaising = 0x00000139,

        CascadeDaggerLowering = 0x0000013A,

        CascadeMaceRaising = 0x0000013B,

        CascadeMaceLowering = 0x0000013C,

        CascadeSpearRaising = 0x0000013D,

        CascadeSpearLowering = 0x0000013E,

        CascadeStaffRaising = 0x0000013F,

        CascadeStaffLowering = 0x00000140,

        StoneCliffsRaising = 0x00000141,

        StoneCliffsLowering = 0x00000142,

        MaxDamageRaising = 0x00000143,

        MaxDamageLowering = 0x00000144,

        BowDamageRaising = 0x00000145,

        BowDamageLowering = 0x00000146,

        BowRangeRaising = 0x00000147,

        BowRangeLowering = 0x00000148,

        ExtraDefenseModRaising = 0x00000149,

        ExtraDefenseModLowering = 0x0000014A,

        ExtraBowSkillRaising = 0x0000014B,

        ExtraBowSkillLowering = 0x0000014C,

        ExtraAlchemySkillRaising = 0x0000014D,

        ExtraAlchemySkillLowering = 0x0000014E,

        ExtraArcaneLoreSkillRaising = 0x0000014F,

        ExtraArcaneLoreSkillLowering = 0x00000150,

        ExtraAppraiseArmorSkillRaising = 0x00000151,

        ExtraAppraiseArmorSkillLowering = 0x00000152,

        ExtraCookingSkillRaising = 0x00000153,

        ExtraCookingSkillLowering = 0x00000154,

        ExtraCrossbowSkillRaising = 0x00000155,

        ExtraCrossbowSkillLowering = 0x00000156,

        ExtraDeceptionSkillRaising = 0x00000157,

        ExtraDeceptionSkillLowering = 0x00000158,

        ExtraLoyaltySkillRaising = 0x00000159,

        ExtraLoyaltySkillLowering = 0x0000015A,

        ExtraFletchingSkillRaising = 0x0000015B,

        ExtraFletchingSkillLowering = 0x0000015C,

        ExtraHealingSkillRaising = 0x0000015D,

        ExtraHealingSkillLowering = 0x0000015E,

        ExtraMeleeDefenseSkillRaising = 0x0000015F,

        ExtraMeleeDefenseSkillLowering = 0x00000160,

        ExtraAppraiseItemSkillRaising = 0x00000161,

        ExtraAppraiseItemSkillLowering = 0x00000162,

        ExtraJumpingSkillRaising = 0x00000163,

        ExtraJumpingSkillLowering = 0x00000164,

        ExtraLifeMagicSkillRaising = 0x00000165,

        ExtraLifeMagicSkillLowering = 0x00000166,

        ExtraLockpickSkillRaising = 0x00000167,

        ExtraLockpickSkillLowering = 0x00000168,

        ExtraAppraiseMagicItemSkillRaising = 0x00000169,

        ExtraAppraiseMagicItemSkillLowering = 0x0000016A,

        ExtraManaConversionSkillRaising = 0x0000016B,

        ExtraManaConversionSkillLowering = 0x0000016C,

        ExtraAssessCreatureSkillRaising = 0x0000016D,

        ExtraAssessCreatureSkillLowering = 0x0000016E,

        ExtraAssessPersonSkillRaising = 0x0000016F,

        ExtraAssessPersonSkillLowering = 0x00000170,

        ExtraRunSkillRaising = 0x00000171,

        ExtraRunSkillLowering = 0x00000172,

        ExtraSwordSkillRaising = 0x00000173,

        ExtraSwordSkillLowering = 0x00000174,

        ExtraThrownWeaponsSkillRaising = 0x00000175,

        ExtraThrownWeaponsSkillLowering = 0x00000176,

        ExtraUnarmedCombatSkillRaising = 0x00000177,

        ExtraUnarmedCombatSkillLowering = 0x00000178,

        ExtraAppraiseWeaponSkillRaising = 0x00000179,

        ExtraAppraiseWeaponSkillLowering = 0x0000017A,

        ArmorIncrease = 0x0000017B,

        ArmorDecrease = 0x0000017C,

        ExtraAcidResistanceRaising = 0x0000017D,

        ExtraAcidResistanceLowering = 0x0000017E,

        ExtraBludgeonResistanceRaising = 0x0000017F,

        ExtraBludgeonResistanceLowering = 0x00000180,

        ExtraFireResistanceRaising = 0x00000181,

        ExtraFireResistanceLowering = 0x00000182,

        ExtraColdResistanceRaising = 0x00000183,

        ExtraColdResistanceLowering = 0x00000184,

        ExtraAttackModRaising = 0x00000185,

        ExtraAttackModLowering = 0x00000186,

        ExtraArmorValueRaising = 0x00000187,

        ExtraArmorValueLowering = 0x00000188,

        ExtraPierceResistanceRaising = 0x00000189,

        ExtraPierceResistanceLowering = 0x0000018A,

        ExtraSlashResistanceRaising = 0x0000018B,

        ExtraSlashResistanceLowering = 0x0000018C,

        ExtraElectricResistanceRaising = 0x0000018D,

        ExtraElectricResistanceLowering = 0x0000018E,

        ExtraWeaponTimeRaising = 0x0000018F,

        ExtraWeaponTimeLowering = 0x00000190,

        BludgeonWardProtection = 0x00000191,

        BludgeonWardVulnerability = 0x00000192,

        SlashWardProtection = 0x00000193,

        SlashWardVulnerability = 0x00000194,

        PierceWardProtection = 0x00000195,

        PierceWardVulnerability = 0x00000196,

        StaminaRestoring = 0x00000197,

        StaminaDepleting = 0x00000198,

        Fireworks = 0x00000199,

        HealthDivide = 0x0000019A,

        StaminaDivide = 0x0000019B,

        ManaDivide = 0x0000019C,

        CoordinationIncrease2 = 0x0000019D,

        StrengthIncrease2 = 0x0000019E,

        FocusIncrease2 = 0x0000019F,

        EnduranceIncrease2 = 0x000001A0,

        SelfIncrease2 = 0x000001A1,

        MeleeDefenseMultiply = 0x000001A2,

        MissileDefenseMultiply = 0x000001A3,

        MagicDefenseMultiply = 0x000001A4,

        AttributesDecrease = 0x000001A5,

        LifeGiverRaising2 = 0x000001A6,

        ItemEnchantmentRaising2 = 0x000001A7,

        SkillsDecrease = 0x000001A8,

        ExtraManaConversionBonus = 0x000001A9,

        WarMysticRaising2 = 0x000001AA,

        WarMysticLowering2 = 0x000001AB,

        MagicDefenseShelterRaising2 = 0x000001AC,

        ExtraLifeMagicSkillRaising2 = 0x000001AD,

        CreatureMysticRaising2 = 0x000001AE,

        ItemMysticRaising2 = 0x000001AF,

        ManaRaising2 = 0x000001B0,

        SelfRaising2 = 0x000001B1,

        CreatureEnchantmentRaising2 = 0x000001B2,

        SalvagingRaising = 0x000001B3,

        ExtraSalvagingRaising = 0x000001B4,

        ExtraSalvagingRaising2 = 0x000001B5,

        CascadeAxeRaising2 = 0x000001B6,

        ExtraBowSkillRaising2 = 0x000001B7,

        ExtraThrownWeaponsSkillRaising2 = 0x000001B8,

        ExtraCrossbowSkillRaising2 = 0x000001B9,

        CascadeDaggerRaising2 = 0x000001BA,

        CascadeMaceRaising2 = 0x000001BB,

        ExtraUnarmedCombatSkillRaising2 = 0x000001BC,

        CascadeSpearRaising2 = 0x000001BD,

        CascadeStaffRaising2 = 0x000001BE,

        ExtraSwordSkillRaising2 = 0x000001BF,

        AcidProtectionRare = 0x000001C0,

        AcidResistanceRaisingRare = 0x000001C1,

        AlchemyRaisingRare = 0x000001C2,

        AppraisalResistanceLoweringRare = 0x000001C3,

        AppraiseArmorRaisingRare = 0x000001C4,

        AppraiseItemRaisingRare = 0x000001C5,

        AppraiseMagicItemRaisingRare = 0x000001C6,

        AppraiseWeaponRaisingRare = 0x000001C7,

        ArcaneLoreRaisingRare = 0x000001C8,

        ArmorRaisingRare = 0x000001C9,

        ArmorValueRaisingRare = 0x000001CA,

        AssessMonsterRaisingRare = 0x000001CB,

        AssessPersonRaisingRare = 0x000001CC,

        AttackModRaisingRare = 0x000001CD,

        AxeRaisingRare = 0x000001CE,

        BludgeonProtectionRare = 0x000001CF,

        BludgeonResistanceRaisingRare = 0x000001D0,

        BowRaisingRare = 0x000001D1,

        ColdProtectionRare = 0x000001D2,

        ColdResistanceRaisingRare = 0x000001D3,

        CookingRaisingRare = 0x000001D4,

        CoordinationRaisingRare = 0x000001D5,

        CreatureEnchantmentRaisingRare = 0x000001D6,

        CrossbowRaisingRare = 0x000001D7,

        DaggerRaisingRare = 0x000001D8,

        DamageRaisingRare = 0x000001D9,

        DeceptionRaisingRare = 0x000001DA,

        DefenseModRaisingRare = 0x000001DB,

        ElectricProtectionRare = 0x000001DC,

        ElectricResistanceRaisingRare = 0x000001DD,

        EnduranceRaisingRare = 0x000001DE,

        FireProtectionRare = 0x000001DF,

        FireResistanceRaisingRare = 0x000001E0,

        FletchingRaisingRare = 0x000001E1,

        FocusRaisingRare = 0x000001E2,

        HealingRaisingRare = 0x000001E3,

        HealthAcceleratingRare = 0x000001E4,

        ItemEnchantmentRaisingRare = 0x000001E5,

        JumpRaisingRare = 0x000001E6,

        LeadershipRaisingRare = 0x000001E7,

        LifeMagicRaisingRare = 0x000001E8,

        LockpickRaisingRare = 0x000001E9,

        LoyaltyRaisingRare = 0x000001EA,

        MaceRaisingRare = 0x000001EB,

        MagicDefenseRaisingRare = 0x000001EC,

        ManaAcceleratingRare = 0x000001ED,

        ManaConversionRaisingRare = 0x000001EE,

        MeleeDefenseRaisingRare = 0x000001EF,

        MissileDefenseRaisingRare = 0x000001F0,

        PierceProtectionRare = 0x000001F1,

        PierceResistanceRaisingRare = 0x000001F2,

        QuicknessRaisingRare = 0x000001F3,

        RunRaisingRare = 0x000001F4,

        SelfRaisingRare = 0x000001F5,

        SlashProtectionRare = 0x000001F6,

        SlashResistanceRaisingRare = 0x000001F7,

        SpearRaisingRare = 0x000001F8,

        StaffRaisingRare = 0x000001F9,

        StaminaAcceleratingRare = 0x000001FA,

        StrengthRaisingRare = 0x000001FB,

        SwordRaisingRare = 0x000001FC,

        ThrownWeaponsRaisingRare = 0x000001FD,

        UnarmedCombatRaisingRare = 0x000001FE,

        WarMagicRaisingRare = 0x000001FF,

        WeaponTimeRaisingRare = 0x00000200,

        ArmorIncreaseInkyArmor = 0x00000201,

        MagicDefenseShelterRaisingFiun = 0x00000202,

        ExtraRunSkillRaisingFiun = 0x00000203,

        ExtraManaConversionSkillRaisingFiun = 0x00000204,

        AttributesIncreaseCantrip1 = 0x00000205,

        ExtraMeleeDefenseSkillRaising2 = 0x00000206,

        ACTDPurchaseRewardSpell = 0x00000207,

        ACTDPurchaseRewardSpellHealth = 0x00000208,

        SaltAshAttackModRaising = 0x00000209,

        QuicknessIncrease2 = 0x0000020A,

        ExtraAlchemySkillRaising2 = 0x0000020B,

        ExtraCookingSkillRaising2 = 0x0000020C,

        ExtraFletchingSkillRaising2 = 0x0000020D,

        ExtraLockpickSkillRaising2 = 0x0000020E,

        MucorManaWell = 0x0000020F,

        StaminaRestoring2 = 0x00000210,

        AllegianceRaising = 0x00000211,

        HealthDoT = 0x00000212,

        HealthDoTSecondary = 0x00000213,

        HealthDoTTertiary = 0x00000214,

        HealthHoT = 0x00000215,

        HealthHoTSecondary = 0x00000216,

        HealthHoTTertiary = 0x00000217,

        HealthDivideSecondary = 0x00000218,

        HealthDivideTertiary = 0x00000219,

        SetSwordRaising = 0x0000021A,

        SetAxeRaising = 0x0000021B,

        SetDaggerRaising = 0x0000021C,

        SetMaceRaising = 0x0000021D,

        SetSpearRaising = 0x0000021E,

        SetStaffRaising = 0x0000021F,

        SetUnarmedRaising = 0x00000220,

        SetBowRaising = 0x00000221,

        SetCrossbowRaising = 0x00000222,

        SetThrownRaising = 0x00000223,

        SetItemEnchantmentRaising = 0x00000224,

        SetCreatureEnchantmentRaising = 0x00000225,

        SetWarMagicRaising = 0x00000226,

        SetLifeMagicRaising = 0x00000227,

        SetMeleeDefenseRaising = 0x00000228,

        SetMissileDefenseRaising = 0x00000229,

        SetMagicDefenseRaising = 0x0000022A,

        SetStaminaAccelerating = 0x0000022B,

        SetCookingRaising = 0x0000022C,

        SetFletchingRaising = 0x0000022D,

        SetLockpickRaising = 0x0000022E,

        SetAlchemyRaising = 0x0000022F,

        SetSalvagingRaising = 0x00000230,

        SetArmorExpertiseRaising = 0x00000231,

        SetWeaponExpertiseRaising = 0x00000232,

        SetItemTinkeringRaising = 0x00000233,

        SetMagicItemExpertiseRaising = 0x00000234,

        SetLoyaltyRaising = 0x00000235,

        SetStrengthRaising = 0x00000236,

        SetEnduranceRaising = 0x00000237,

        SetCoordinationRaising = 0x00000238,

        SetQuicknessRaising = 0x00000239,

        SetFocusRaising = 0x0000023A,

        SetWillpowerRaising = 0x0000023B,

        SetHealthRaising = 0x0000023C,

        SetStaminaRaising = 0x0000023D,

        SetManaRaising = 0x0000023E,

        SetSprintRaising = 0x0000023F,

        SetJumpingRaising = 0x00000240,

        SetSlashResistanceRaising = 0x00000241,

        SetBludgeonResistanceRaising = 0x00000242,

        SetPierceResistanceRaising = 0x00000243,

        SetFlameResistanceRaising = 0x00000244,

        SetAcidResistanceRaising = 0x00000245,

        SetFrostResistanceRaising = 0x00000246,

        SetLightningResistanceRaising = 0x00000247,

        CraftingLockPickRaising = 0x00000248,

        CraftingFletchingRaising = 0x00000249,

        CraftingCookingRaising = 0x0000024A,

        CraftingAlchemyRaising = 0x0000024B,

        CraftingArmorTinkeringRaising = 0x0000024C,

        CraftingWeaponTinkeringRaising = 0x0000024D,

        CraftingMagicTinkeringRaising = 0x0000024E,

        CraftingItemTinkeringRaising = 0x0000024F,

        SkillPercentAlchemyRaising = 0x00000250,

        TwoHandedRaising = 0x00000251,

        TwoHandedLowering = 0x00000252,

        ExtraTwoHandedSkillRaising = 0x00000253,

        ExtraTwoHandedSkillLowering = 0x00000254,

        ExtraTwoHandedSkillRaising2 = 0x00000255,

        TwoHandedRaisingRare = 0x00000256,

        SetTwoHandedRaising = 0x00000257,

        GearCraftRaising = 0x00000258,

        GearCraftLowering = 0x00000259,

        ExtraGearCraftSkillRaising = 0x0000025A,

        ExtraGearCraftSkillLowering = 0x0000025B,

        ExtraGearCraftSkillRaising2 = 0x0000025C,

        GearCraftRaisingRare = 0x0000025D,

        SetGearCraftRaising = 0x0000025E,

        LoyaltyManaRaising = 0x0000025F,

        LoyaltyStaminaRaising = 0x00000260,

        LeadershipHealthRaising = 0x00000261,

        TrinketDamageRaising = 0x00000262,

        TrinketDamageLowering = 0x00000263,

        TrinketHealthRaising = 0x00000264,

        TrinketStaminaRaising = 0x00000265,

        TrinketManaRaising = 0x00000266,

        TrinketXPRaising = 0x00000267,

        DeceptionArcaneLoreRaising = 0x00000268,

        HealOverTimeRaising = 0x00000269,

        DamageOverTimeRaising = 0x0000026A,

        HealingResistRatingRaising = 0x0000026B,

        AetheriaDamageRatingRaising = 0x0000026C,

        AetheriaDamageReductionRaising = 0x0000026D,

        AetheriaHealthRaising = 0x0000026F,

        AetheriaStaminaRaising = 0x00000270,

        AetheriaManaRaising = 0x00000271,

        AetheriaCriticalDamageRaising = 0x00000272,

        AetheriaHealingAmplificationRaising = 0x00000273,

        AetheriaProcDamageRatingRaising = 0x00000274,

        AetheriaProcDamageReductionRaising = 0x00000275,

        AetheriaProcHealthOverTimeRaising = 0x00000276,

        AetheriaProcDamageOverTimeRaising = 0x00000277,

        AetheriaProcHealingReductionRaising = 0x00000278,

        RareDamageRatingRaising = 0x00000279,

        RareDamageReductionRatingRaising = 0x0000027A,

        AetheriaEnduranceRaising = 0x0000027B,

        NetherDamageOverTimeRaising = 0x0000027C,

        NetherDamageOverTimeRaising2 = 0x0000027D,

        NetherDamageOverTimeRaising3 = 0x0000027E,

        NetherStreak = 0x0000027F,

        NetherMissile = 0x00000280,

        NetherRing = 0x00000281,

        NetherDamageRatingLowering = 0x00000282,

        NetherDamageHealingReductionRaising = 0x00000283,

        VoidMagicLowering = 0x00000284,

        VoidMagicRaising = 0x00000285,

        VoidMysticRaising = 0x00000286,

        SetVoidMagicRaising = 0x00000287,

        VoidMagicRaisingRare = 0x00000288,

        VoidMysticRaising2 = 0x00000289,

        LuminanceDamageRatingRaising = 0x0000028A,

        LuminanceDamageReductionRaising = 0x0000028B,

        LuminanceHealthRaising = 0x0000028C,

        AetheriaCriticalReductionRaising = 0x0000028D,

        ExtraMissileDefenseSkillRaising = 0x0000028E,

        ExtraMissileDefenseSkillLowering = 0x0000028F,

        ExtraMissileDefenseSkillRaising2 = 0x00000290,

        AetheriaHealthResistanceRaising = 0x00000291,

        AetheriaDotResistanceRaising = 0x00000292,

        CloakSkillRaising = 0x00000293,

        CloakAllSkillRaising = 0x00000294,

        CloakMagicDefenseLowering = 0x00000295,

        CloakMeleeDefenseLowering = 0x00000296,

        CloakMissileDefenseLowering = 0x00000297,

        DirtyFightingLowering = 0x00000298,

        DirtyFightingRaising = 0x00000299,

        ExtraDirtyFightingRaising = 0x0000029A,

        DualWieldLowering = 0x0000029B,

        DualWieldRaising = 0x0000029C,

        ExtraDualWieldRaising = 0x0000029D,

        RecklessnessLowering = 0x0000029E,

        RecklessnessRaising = 0x0000029F,

        ExtraRecklessnessRaising = 0x000002A0,

        ShieldLowering = 0x000002A1,

        ShieldRaising = 0x000002A2,

        ExtraShieldRaising = 0x000002A3,

        SneakAttackLowering = 0x000002A4,

        SneakAttackRaising = 0x000002A5,

        ExtraSneakAttackRaising = 0x000002A6,

        RareDirtyFightingRaising = 0x000002A7,

        RareDualWieldRaising = 0x000002A8,

        RareRecklessnessRaising = 0x000002A9,

        RareShieldRaising = 0x000002AA,

        RareSneakAttackRaising = 0x000002AB,

        DFAttackSkillDebuff = 0x000002AC,

        DFBleedDamage = 0x000002AD,

        DFDefenseSkillDebuff = 0x000002AE,

        DFHealingDebuff = 0x000002AF,

        SetDirtyFightingRaising = 0x000002B0,

        SetDualWieldRaising = 0x000002B1,

        SetRecklessnessRaising = 0x000002B2,

        SetShieldRaising = 0x000002B3,

        SetSneakAttackRaising = 0x000002B4,

        LifeGiverMhoire = 0x000002B5,

        RareDamageRatingRaising2 = 0x000002B6,

        SpellDamageRaising = 0x000002B7,

        SummoningRaising = 0x000002B8,

        SummoningLowering = 0x000002B9,

        ExtraSummoningSkillRaising = 0x000002BA,

        SetSummoningRaising = 0x000002BB,

        ParagonEnduranceRaising = 0x000002C0,

        ParagonManaRaising = 0x000002C1,

        ParagonStaminaRaising = 0x000002C2,

        ParagonDirtyFightingRaising = 0x000002C3,

        ParagonDualWieldRaising = 0x000002C4,

        ParagonRecklessnessRaising = 0x000002C5,

        ParagonSneakAttackRaising = 0x000002C6,

        ParagonDamageRatingRaising = 0x000002C7,

        ParagonDamageReductionRatingRaising = 0x000002C8,

        ParagonCriticalDamageRatingRaising = 0x000002C9,

        ParagonCriticalDamageReductionRatingRaising = 0x000002CA,

        ParagonAxeRaising = 0x000002CB,

        ParagonDaggerRaising = 0x000002CC,

        ParagonSwordRaising = 0x000002CD,

        ParagonWarMagicRaising = 0x000002CE,

        ParagonLifeMagicRaising = 0x000002CF,

        ParagonVoidMagicRaising = 0x000002D0,

        ParagonBowRaising = 0x000002D1,

        ParagonStrengthRaising = 0x000002D2,

        ParagonCoordinationRaising = 0x000002D3,

        ParagonQuicknessRaising = 0x000002D4,

        ParagonFocusRaising = 0x000002D5,

        ParagonWillpowerRaising = 0x000002D6,

        ParagonTwoHandedRaising = 0x000002D7,

        GauntletDamageReductionRatingRaising = 0x000002D8,

        GauntletDamageRatingRaising = 0x000002D9,

        GauntletHealingRatingRaising = 0x000002DA,

        GauntletVitalityRaising = 0x000002DB,

        GauntletCriticalDamageRatingRaising = 0x000002DC,

        GauntletCriticalDamageReductionRatingRaising = 0x000002DD,

    };
}
