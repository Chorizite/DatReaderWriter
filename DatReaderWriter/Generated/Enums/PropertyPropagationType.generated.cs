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
    public enum PropertyPropagationType : byte {
        NetPredictedSharedVisually = 0x0,

        NetPredictedSharedPrivately = 0x1,

        NetSharedVisually = 0x2,

        NetSharedPrivately = 0x3,

        NetNotShared = 0x4,

        WorldSharedWithServers = 0x5,

        WorldSharedWithServersAndClients = 0x6,

    };
}
