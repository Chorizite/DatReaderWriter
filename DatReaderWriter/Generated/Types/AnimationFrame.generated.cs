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
using DatReaderWriter.Enums;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.Types {
    public partial class AnimationFrame : IDatObjType {
        public List<Frame> Frames = [];

        public List<AnimationHook> Hooks = [];

        private uint NumParts;
        
        public AnimationFrame(uint NumParts) {
            this.NumParts = NumParts;
        }
        
        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            for (var i=0; i < NumParts; i++) {
                Frames.Add(reader.ReadItem<Frame>());
            }
            var _numHooks = reader.ReadUInt32();
            for (var i=0; i < _numHooks; i++) {
                var _peekedValue = (AnimationHookType)reader.ReadUInt32();
                reader.Skip(-sizeof(AnimationHookType));
                Hooks.Add(AnimationHook.Unpack(reader, _peekedValue));
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            foreach (var item in Frames) {
                writer.WriteItem<Frame>(item);
            }
            writer.WriteUInt32((uint)Hooks.Count());
            foreach (var item in Hooks) {
                writer.WriteItem<AnimationHook>(item);
            }
            return true;
        }

    }

}
