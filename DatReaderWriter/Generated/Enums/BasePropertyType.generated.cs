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
    /// <summary>
    /// This enum comes from MasterProperty.Properties
    /// </summary>
    public enum BasePropertyType : int {
        Invalid = 0x00000000,

        Bool = 0x00000001,

        Integer = 0x00000002,

        LongInteger = 0x00000003,

        Float = 0x00000004,

        Vector = 0x00000005,

        Color = 0x00000006,

        String = 0x00000007,

        StringInfo = 0x00000008,

        Enum = 0x00000009,

        DataId = 0x0000000A,

        Waveform = 0x0000000B,

        InstanceId = 0x0000000C,

        Position = 0x0000000D,

        TimeStamp = 0x0000000E,

        Bitfield32 = 0x0000000F,

        Bitfield64 = 0x00000010,

        Array = 0x00000011,

        Struct = 0x00000012,

        StringToken = 0x00000013,

        PropertyName = 0x00000014,

        TriState = 0x00000015,

    };
}
