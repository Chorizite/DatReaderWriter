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
    [Flags]
    public enum PartsMask : uint {
        HasSoundInfo = 0x01,

        HasSceneInfo = 0x02,

        HasSkyInfo = 0x10,

        HasRegionMisc = 0x200,

    };
}
