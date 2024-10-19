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
using ACClientLib.DatReaderWriter.Types;
using ACClientLib.DatReaderWriter.Attributes;

namespace ACClientLib.DatReaderWriter.DBObjs {
    /// <summary>
    /// DB_TYPE_SETUP in the client.
    /// </summary>
    [DBObjType(DatFileType.Portal, false, 0x02000000, 0x0200FFFF)]
    public class Setup : DBObj {
        /// <inheritdoc />
        public override bool HasDataCategory => false;

        public SetupFlags Flags;

        public uint NumParts;

        public List<uint> Parts = [];

        public List<uint> ParentIndex = [];

        public List<Vector3> DefaultScale = [];

        public Dictionary<ParentLocation, LocationType> HoldingLocations = [];

        public Dictionary<ParentLocation, LocationType> ConnectionPoints = [];

        public Dictionary<Placement, AnimationFrame> PlacementFrames = [];

        public List<CylSphere> CylSpheres = [];

        public List<Sphere> Spheres = [];

        public float Height;

        public float Radius;

        public float StepUpHeight;

        public float StepDownHeight;

        public Sphere SortingSphere;

        public Sphere SelectionSphere;

        public Dictionary<int, LightInfo> Lights = [];

        public uint DefaultAnimation;

        public uint DefaultScript;

        public uint DefaultMotionTable;

        public uint DefaultSoundTable;

        public uint DefaultScriptTable;

        /// <inheritdoc />
        public override bool Unpack(DatFileReader reader) {
            base.Unpack(reader);
            Flags = (SetupFlags)reader.ReadUInt32();
            var NumParts = reader.ReadUInt32();
            for (var i=0; i < NumParts; i++) {
                Parts.Add(reader.ReadUInt32());
            }
            if (Flags.HasFlag(SetupFlags.HasParent)) {
                for (var i=0; i < NumParts; i++) {
                    ParentIndex.Add(reader.ReadUInt32());
                }
            }
            if (Flags.HasFlag(SetupFlags.HasDefaultScale)) {
                for (var i=0; i < NumParts; i++) {
                    DefaultScale.Add(reader.ReadVector3());
                }
            }
            var _numHoldingLocations = reader.ReadInt32();
            for (var i=0; i < _numHoldingLocations; i++) {
                var _key = (ParentLocation)reader.ReadInt32();
                var _val = reader.ReadItem<LocationType>();
                HoldingLocations.Add(_key, _val);
            }
            var _numConnectionPoints = reader.ReadInt32();
            for (var i=0; i < _numConnectionPoints; i++) {
                var _key = (ParentLocation)reader.ReadInt32();
                var _val = reader.ReadItem<LocationType>();
                ConnectionPoints.Add(_key, _val);
            }
            var _numPlacements = reader.ReadInt32();
            for (var i=0; i < _numPlacements; i++) {
                var _key = (Placement)reader.ReadUInt32();
                var _val = reader.ReadItem<AnimationFrame>(NumParts);
                PlacementFrames.Add(_key, _val);
            }
            var _numCylSpheres = reader.ReadUInt32();
            for (var i=0; i < _numCylSpheres; i++) {
                CylSpheres.Add(reader.ReadItem<CylSphere>());
            }
            var _numSpheres = reader.ReadUInt32();
            for (var i=0; i < _numSpheres; i++) {
                Spheres.Add(reader.ReadItem<Sphere>());
            }
            Height = reader.ReadSingle();
            Radius = reader.ReadSingle();
            StepUpHeight = reader.ReadSingle();
            StepDownHeight = reader.ReadSingle();
            SortingSphere = reader.ReadItem<Sphere>();
            SelectionSphere = reader.ReadItem<Sphere>();
            var _numLights = reader.ReadInt32();
            for (var i=0; i < _numLights; i++) {
                var _key = reader.ReadInt32();
                var _val = reader.ReadItem<LightInfo>();
                Lights.Add(_key, _val);
            }
            DefaultAnimation = reader.ReadUInt32();
            DefaultScript = reader.ReadUInt32();
            DefaultMotionTable = reader.ReadUInt32();
            DefaultSoundTable = reader.ReadUInt32();
            DefaultScriptTable = reader.ReadUInt32();
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatFileWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32((uint)Flags);
            writer.WriteUInt32((uint)Parts.Count());
            foreach (var item in Parts) {
                writer.WriteUInt32(item);
            }
            if (Flags.HasFlag(SetupFlags.HasParent)) {
                foreach (var item in ParentIndex) {
                    writer.WriteUInt32(item);
                }
            }
            if (Flags.HasFlag(SetupFlags.HasDefaultScale)) {
                foreach (var item in DefaultScale) {
                    writer.WriteVector3(item);
                }
            }
            writer.WriteInt32((int)HoldingLocations.Count());
            foreach (var kv in HoldingLocations) {
                writer.WriteInt32((int)kv.Key);
                writer.WriteItem<LocationType>(kv.Value);
            }
            writer.WriteInt32((int)ConnectionPoints.Count());
            foreach (var kv in ConnectionPoints) {
                writer.WriteInt32((int)kv.Key);
                writer.WriteItem<LocationType>(kv.Value);
            }
            writer.WriteInt32((int)PlacementFrames.Count());
            foreach (var kv in PlacementFrames) {
                writer.WriteUInt32((uint)kv.Key);
                writer.WriteItem<AnimationFrame>(kv.Value);
            }
            writer.WriteUInt32((uint)CylSpheres.Count());
            foreach (var item in CylSpheres) {
                writer.WriteItem<CylSphere>(item);
            }
            writer.WriteUInt32((uint)Spheres.Count());
            foreach (var item in Spheres) {
                writer.WriteItem<Sphere>(item);
            }
            writer.WriteSingle(Height);
            writer.WriteSingle(Radius);
            writer.WriteSingle(StepUpHeight);
            writer.WriteSingle(StepDownHeight);
            writer.WriteItem<Sphere>(SortingSphere);
            writer.WriteItem<Sphere>(SelectionSphere);
            writer.WriteInt32((int)Lights.Count());
            foreach (var kv in Lights) {
                writer.WriteInt32(kv.Key);
                writer.WriteItem<LightInfo>(kv.Value);
            }
            writer.WriteUInt32(DefaultAnimation);
            writer.WriteUInt32(DefaultScript);
            writer.WriteUInt32(DefaultMotionTable);
            writer.WriteUInt32(DefaultSoundTable);
            writer.WriteUInt32(DefaultScriptTable);
            return true;
        }

    }

}
