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
    public enum UIStateId : uint {
        Undef = 0x00000000,

        Normal = 0x00000001,

        Normal_rollover = 0x00000002,

        Normal_pressed = 0x00000003,

        Normal_focussed = 0x00000004,

        Normal_activated = 0x00000005,

        Highlight = 0x00000006,

        Highlight_rollover = 0x00000007,

        Highlight_pressed = 0x00000008,

        Drag_rollover_accept = 0x00000009,

        Drag_rollover_reject = 0x0000000A,

        Closed = 0x0000000B,

        Open = 0x0000000C,

        Ghosted = 0x0000000D,

        Unencumbered = 0x0000000E,

        Encumbered = 0x0000000F,

        Heavily_encumbered = 0x00000010,

        Connection_good = 0x00000011,

        Connection_uncertain = 0x00000012,

        Connection_bad = 0x00000013,

        Connection_disconnected = 0x00000014,

        Csm_highlight = 0x00000015,

        Csm_normal = 0x00000016,

        Csm_ghosted = 0x00000017,

        Dialog_pending_true = 0x00000018,

        Dialog_pending_false = 0x00000019,

        Talkfocus_highlight = 0x10000001,

        MeleeCombat = 0x10000003,

        MissileCombat = 0x10000004,

        HideDetail = 0x10000006,

        ShowDetail = 0x10000007,

        Abuse_PageOne = 0x10000008,

        Abuse_PageTwo = 0x10000009,

        Abuse_PageThree = 0x1000000A,

        ObjectSelected = 0x1000000B,

        StackedItemSelected = 0x1000000C,

        StackedItem = 0x1000000D,

        UrgentAssistance_PageOne = 0x1000000E,

        UrgentAssistance_PageTwo = 0x1000000F,

        UrgentAssistance_PageThree = 0x10000010,

        StatManagement_Footer_Default = 0x10000011,

        StatManagement_Footer_Text = 0x10000012,

        StatManagement_Footer_Meter = 0x10000013,

        Buffed = 0x10000014,

        Unselected = 0x10000016,

        Selected = 0x10000017,

        Unlocked = 0x10000018,

        Locked = 0x10000019,

        Inactive = 0x1000001A,

        Active = 0x1000001B,

        ItemSlot_Empty = 0x1000001C,

        ItemSlot_Filled = 0x1000001D,

        Aluvian = 0x10000021,

        Gharundim = 0x10000022,

        Sho = 0x10000023,

        Viamont = 0x10000024,

        Heritage = 0x10000025,

        Profession = 0x10000026,

        Skills = 0x10000027,

        Appearance = 0x10000028,

        Town = 0x10000029,

        Summary = 0x1000002A,

        Custom = 0x1000002B,

        Bow_hunter = 0x1000002C,

        Life_caster = 0x1000002D,

        War_mage = 0x1000002E,

        Wayfarer = 0x1000002F,

        Soldier = 0x10000030,

        Swashbuckler = 0x10000031,

        Create_normal = 0x10000032,

        Create_admin = 0x10000033,

        Holtburg = 0x10000034,

        Sanamar = 0x10000035,

        Yaraq = 0x10000036,

        Shoushi = 0x10000037,

        Frame1 = 0x10000038,

        Frame2 = 0x10000039,

        Frame3 = 0x1000003A,

        Inprogress = 0x1000003B,

        Done = 0x1000003C,

        IntroVideo = 0x1000003E,

        ItemSlot_DragOver_Normal = 0x1000003F,

        ItemSlot_DragOver_Accept = 0x10000040,

        ItemSlot_DragOver_Reject = 0x10000041,

        JumpMode = 0x10000042,

        MeleeMode = 0x10000043,

        MissileMode = 0x10000044,

        DDDMode = 0x10000045,

        ItemSlot_DragOver_DropIn = 0x10000046,

        Maximized = 0x10000047,

        Minimized = 0x10000048,

        Uninscribed = 0x10000050,

        Create_envoy = 0x10000053,

        Online = 0x10000054,

        Offline = 0x10000055,

        IsCharacter = 0x10000056,

        IsAccount = 0x10000057,

        Shadow = 0x10000058,

        Penumbraen = 0x10000059,

        Gearknight = 0x1000005A,

        Undead = 0x1000005B,

        Empyrean = 0x1000005C,

        Olthoi = 0x1000005D,

        Olthoiacid = 0x1000005E,

        Auntumerok = 0x1000005F,

        Lugian = 0x10000060,

        LockedUI = 0x10000063,

        UnlockedUI = 0x10000064,

    };
}
