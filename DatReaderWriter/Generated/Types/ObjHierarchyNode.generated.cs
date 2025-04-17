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
    public partial class ObjHierarchyNode : IDatObjType {
        public string MenuName;

        public uint WCID;

        /// <summary>
        /// Child hierarchy nodes
        /// </summary>
        public List<ObjHierarchyNode> Children = [];

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            MenuName = reader.ReadObfuscatedString();
            reader.Align(4);
            WCID = reader.ReadUInt32();
            var _numChildren = reader.ReadInt32();
            for (var i=0; i < _numChildren; i++) {
                Children.Add(reader.ReadItem<ObjHierarchyNode>());
            }
            reader.Align(4);
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteObfuscatedString(MenuName);
            writer.Align(4);
            writer.WriteUInt32(WCID);
            writer.WriteInt32((int)Children.Count());
            foreach (var item in Children) {
                writer.WriteItem<ObjHierarchyNode>(item);
            }
            writer.Align(4);
            return true;
        }

    }

}
