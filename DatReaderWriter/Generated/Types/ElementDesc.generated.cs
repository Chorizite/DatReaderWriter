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
    public partial class ElementDesc : IDatObjType {
        public StateDesc StateDesc;

        public uint ReadOrder;

        public uint ElementId;

        public uint Type;

        /// <summary>
        /// ElementId that this inherits from, if any. BaseLayoutId contains the parent layout.
        /// </summary>
        public uint BaseElement;

        /// <summary>
        /// The parent layout for the BaseElement, if any
        /// </summary>
        public uint BaseLayoutId;

        /// <summary>
        /// The default state this element is in.
        /// </summary>
        public StateId DefaultState;

        /// <summary>
        /// The X position of this element, relative to the parent.
        /// </summary>
        public uint X;

        /// <summary>
        /// The Y position of this element, relative to the parent.
        /// </summary>
        public uint Y;

        /// <summary>
        /// The width of this element.
        /// </summary>
        public uint Width;

        /// <summary>
        /// The height of this element.
        /// </summary>
        public uint Height;

        /// <summary>
        /// Determines draw order, higher is on top
        /// </summary>
        public uint ZLevel;

        public uint LeftEdge;

        public uint TopEdge;

        public uint RightEdge;

        public uint BottomEdge;

        public Dictionary<StateId, StateDesc> States = [];

        public Dictionary<uint, ElementDesc> Children = [];

        /// <inheritdoc />
        public bool Unpack(DatBinReader reader) {
            StateDesc = reader.ReadItem<StateDesc>();
            ReadOrder = reader.ReadUInt32();
            ElementId = reader.ReadUInt32();
            Type = reader.ReadUInt32();
            BaseElement = reader.ReadUInt32();
            BaseLayoutId = reader.ReadUInt32();
            DefaultState = (StateId)reader.ReadUInt32();
            if (StateDesc.IncorporationFlags.HasFlag(IncorporationFlags.X)) {
                X = reader.ReadUInt32();
            }
            if (StateDesc.IncorporationFlags.HasFlag(IncorporationFlags.Y)) {
                Y = reader.ReadUInt32();
            }
            if (StateDesc.IncorporationFlags.HasFlag(IncorporationFlags.Width)) {
                Width = reader.ReadUInt32();
            }
            if (StateDesc.IncorporationFlags.HasFlag(IncorporationFlags.Height)) {
                Height = reader.ReadUInt32();
            }
            if (StateDesc.IncorporationFlags.HasFlag(IncorporationFlags.ZLevel)) {
                ZLevel = reader.ReadUInt32();
            }
            LeftEdge = reader.ReadUInt32();
            TopEdge = reader.ReadUInt32();
            RightEdge = reader.ReadUInt32();
            BottomEdge = reader.ReadUInt32();
            var _statesBucketSize = reader.ReadByte();
            var _numStates = reader.ReadCompressedUInt();
            for (var i=0; i < _numStates; i++) {
                var _key = (StateId)reader.ReadUInt32();
                var _val = reader.ReadItem<StateDesc>();
                States.Add(_key, _val);
            }
            var _childrenBucketSize = reader.ReadByte();
            var _numChildren = reader.ReadCompressedUInt();
            for (var i=0; i < _numChildren; i++) {
                var _key = reader.ReadUInt32();
                var _val = reader.ReadItem<ElementDesc>();
                Children.Add(_key, _val);
            }
            return true;
        }

        /// <inheritdoc />
        public bool Pack(DatBinWriter writer) {
            writer.WriteItem<StateDesc>(StateDesc);
            writer.WriteUInt32(ReadOrder);
            writer.WriteUInt32(ElementId);
            writer.WriteUInt32(Type);
            writer.WriteUInt32(BaseElement);
            writer.WriteUInt32(BaseLayoutId);
            writer.WriteUInt32((uint)DefaultState);
            if (StateDesc.IncorporationFlags.HasFlag(IncorporationFlags.X)) {
                writer.WriteUInt32(X);
            }
            if (StateDesc.IncorporationFlags.HasFlag(IncorporationFlags.Y)) {
                writer.WriteUInt32(Y);
            }
            if (StateDesc.IncorporationFlags.HasFlag(IncorporationFlags.Width)) {
                writer.WriteUInt32(Width);
            }
            if (StateDesc.IncorporationFlags.HasFlag(IncorporationFlags.Height)) {
                writer.WriteUInt32(Height);
            }
            if (StateDesc.IncorporationFlags.HasFlag(IncorporationFlags.ZLevel)) {
                writer.WriteUInt32(ZLevel);
            }
            writer.WriteUInt32(LeftEdge);
            writer.WriteUInt32(TopEdge);
            writer.WriteUInt32(RightEdge);
            writer.WriteUInt32(BottomEdge);
            writer.WriteByte(1);
            writer.WriteCompressedUInt((uint)States.Count());
            foreach (var kv in States) {
                writer.WriteUInt32((uint)kv.Key);
                writer.WriteItem<StateDesc>(kv.Value);
            }
            writer.WriteByte(1);
            writer.WriteCompressedUInt((uint)Children.Count());
            foreach (var kv in Children) {
                writer.WriteUInt32(kv.Key);
                writer.WriteItem<ElementDesc>(kv.Value);
            }
            return true;
        }

    }

}
