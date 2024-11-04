//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//
//                                                            //
//                          WARNING                           //
//                                                            //
//           DO NOT MAKE LOCAL CHANGES TO THIS FILE           //
//               EDIT THE .tt TEMPLATE INSTEAD                //
//                                                            //
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//


using System;
using System.Numerics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.IO;

namespace ACClientLib.DatReaderWriter.Types {
    public partial class CylSphere : IDatObjType {
        public Vector3 Origin;

        public float Radius;

        public float Height;

        /// <inheritdoc />
        public bool Unpack(DatFileReader reader) {
            Origin = reader.ReadVector3();
            Radius = reader.ReadSingle();
            Height = reader.ReadSingle();
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatFileWriter writer) {
            writer.WriteVector3(Origin);
            writer.WriteSingle(Radius);
            writer.WriteSingle(Height);
            return true;
        }

    }

}
