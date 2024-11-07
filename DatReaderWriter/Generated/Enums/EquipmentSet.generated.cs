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
    public enum EquipmentSet : int {
        Invalid = 0x00000000,

        Test = 0x00000001,

        Test2 = 0x00000002,

        Unknown3 = 0x00000003,

        CarraidasBenediction = 0x00000004,

        NobleRelic = 0x00000005,

        AncientRelic = 0x00000006,

        AlduressaRelic = 0x00000007,

        Ninja = 0x00000008,

        EmpyreanRings = 0x00000009,

        ArmMindHeart = 0x0000000A,

        ArmorPerfectLight = 0x0000000B,

        ArmorPerfectLight2 = 0x0000000C,

        Soldiers = 0x0000000D,

        Adepts = 0x0000000E,

        Archers = 0x0000000F,

        Defenders = 0x00000010,

        Tinkers = 0x00000011,

        Crafters = 0x00000012,

        Hearty = 0x00000013,

        Dexterous = 0x00000014,

        Wise = 0x00000015,

        Swift = 0x00000016,

        Hardened = 0x00000017,

        Reinforced = 0x00000018,

        Interlocking = 0x00000019,

        Flameproof = 0x0000001A,

        Acidproof = 0x0000001B,

        Coldproof = 0x0000001C,

        Lightningproof = 0x0000001D,

        SocietyArmor = 0x0000001E,

        ColosseumClothing = 0x0000001F,

        GraveyardClothing = 0x00000020,

        OlthoiClothing = 0x00000021,

        NoobieArmor = 0x00000022,

        AetheriaDefense = 0x00000023,

        AetheriaDestruction = 0x00000024,

        AetheriaFury = 0x00000025,

        AetheriaGrowth = 0x00000026,

        AetheriaVigor = 0x00000027,

        RareDamageResistance = 0x00000028,

        RareDamageBoost = 0x00000029,

        OlthoiArmorDRed = 0x0000002A,

        OlthoiArmorCRat = 0x0000002B,

        OlthoiArmorCRed = 0x0000002C,

        OlthoiArmorDRat = 0x0000002D,

        AlduressaRelicUpgrade = 0x0000002E,

        AncientRelicUpgrade = 0x0000002F,

        NobleRelicUpgrade = 0x00000030,

        CloakAlchemy = 0x00000031,

        CloakArcaneLore = 0x00000032,

        CloakArmorTinkering = 0x00000033,

        CloakAssessPerson = 0x00000034,

        CloakLightWeapons = 0x00000035,

        CloakMissileWeapons = 0x00000036,

        CloakCooking = 0x00000037,

        CloakCreatureEnchantment = 0x00000038,

        CloakCrossbow = 0x00000039,

        CloakFinesseWeapons = 0x0000003A,

        CloakDeception = 0x0000003B,

        CloakFletching = 0x0000003C,

        CloakHealing = 0x0000003D,

        CloakItemEnchantment = 0x0000003E,

        CloakItemTinkering = 0x0000003F,

        CloakLeadership = 0x00000040,

        CloakLifeMagic = 0x00000041,

        CloakLoyalty = 0x00000042,

        CloakMace = 0x00000043,

        CloakMagicDefense = 0x00000044,

        CloakMagicItemTinkering = 0x00000045,

        CloakManaConversion = 0x00000046,

        CloakMeleeDefense = 0x00000047,

        CloakMissileDefense = 0x00000048,

        CloakSalvaging = 0x00000049,

        CloakSpear = 0x0000004A,

        CloakStaff = 0x0000004B,

        CloakHeavyWeapons = 0x0000004C,

        CloakThrownWeapon = 0x0000004D,

        CloakTwoHandedCombat = 0x0000004E,

        CloakUnarmedCombat = 0x0000004F,

        CloakVoidMagic = 0x00000050,

        CloakWarMagic = 0x00000051,

        CloakWeaponTinkering = 0x00000052,

        CloakAssessCreature = 0x00000053,

        CloakDirtyFighting = 0x00000054,

        CloakDualWield = 0x00000055,

        CloakRecklessness = 0x00000056,

        CloakShield = 0x00000057,

        CloakSneakAttack = 0x00000058,

        Ninja_New = 0x00000059,

        CloakSummoning = 0x0000005A,

        ShroudedSoul = 0x0000005B,

        DarkenedMind = 0x0000005C,

        CloudedSpirit = 0x0000005D,

        MinorStingingShroudedSoul = 0x0000005E,

        MinorSparkingShroudedSoul = 0x0000005F,

        MinorSmolderingShroudedSoul = 0x00000060,

        MinorShiveringShroudedSoul = 0x00000061,

        MinorStingingDarkenedMind = 0x00000062,

        MinorSparkingDarkenedMind = 0x00000063,

        MinorSmolderingDarkenedMind = 0x00000064,

        MinorShiveringDarkenedMind = 0x00000065,

        MinorStingingCloudedSpirit = 0x00000066,

        MinorSparkingCloudedSpirit = 0x00000067,

        MinorSmolderingCloudedSpirit = 0x00000068,

        MinorShiveringCloudedSpirit = 0x00000069,

        MajorStingingShroudedSoul = 0x0000006A,

        MajorSparkingShroudedSoul = 0x0000006B,

        MajorSmolderingShroudedSoul = 0x0000006C,

        MajorShiveringShroudedSoul = 0x0000006D,

        MajorStingingDarkenedMind = 0x0000006E,

        MajorSparkingDarkenedMind = 0x0000006F,

        MajorSmolderingDarkenedMind = 0x00000070,

        MajorShiveringDarkenedMind = 0x00000071,

        MajorStingingCloudedSpirit = 0x00000072,

        MajorSparkingCloudedSpirit = 0x00000073,

        MajorSmolderingCloudedSpirit = 0x00000074,

        MajorShiveringCloudedSpirit = 0x00000075,

        BlackfireStingingShroudedSoul = 0x00000076,

        BlackfireSparkingShroudedSoul = 0x00000077,

        BlackfireSmolderingShroudedSoul = 0x00000078,

        BlackfireShiveringShroudedSoul = 0x00000079,

        BlackfireStingingDarkenedMind = 0x0000007A,

        BlackfireSparkingDarkenedMind = 0x0000007B,

        BlackfireSmolderingDarkenedMind = 0x0000007C,

        BlackfireShiveringDarkenedMind = 0x0000007D,

        BlackfireStingingCloudedSpirit = 0x0000007E,

        BlackfireSparkingCloudedSpirit = 0x0000007F,

        BlackfireSmolderingCloudedSpirit = 0x00000080,

        BlackfireShiveringCloudedSpirit = 0x00000081,

        ShimmeringShadowsSet = 0x00000082,

        BrownSocietyLocket = 0x00000083,

        YellowSocietyLocket = 0x00000084,

        RedSocietyBand = 0x00000085,

        GreenSocietyBand = 0x00000086,

        PurpleSocietyBand = 0x00000087,

        BlueSocietyBand = 0x00000088,

        GauntletGarb = 0x00000089,

        ParagonMissile = 0x0000008A,

        ParagonCaster = 0x0000008B,

        ParagonMelee = 0x0000008C,

    };
}
