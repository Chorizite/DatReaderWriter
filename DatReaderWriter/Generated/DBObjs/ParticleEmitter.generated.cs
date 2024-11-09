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
using DatReaderWriter.Types;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;

namespace DatReaderWriter.DBObjs {
    /// <summary>
    /// DB_TYPE_PARTICLE_EMITTER in the client.
    /// </summary>
    [DBObjType(typeof(ParticleEmitter), DatFileType.Portal, DBObjType.ParticleEmitter, DBObjHeaderFlags.HasId, 0x32000000, 0x3200FFFF, 0x00000000)]
    public partial class ParticleEmitter : DBObj {
        /// <inheritdoc />
        public override DBObjHeaderFlags HeaderFlags => DBObjHeaderFlags.HasId;

        /// <inheritdoc />
        public override DBObjType DBObjType => DBObjType.ParticleEmitter;

        public uint Unknown;

        public EmitterType EmitterType;

        public ParticleType ParticleType;

        public uint GfxObjId;

        public uint HwGfxObjId;

        public double Birthrate;

        public int MaxParticles;

        public int InitialParticles;

        public int TotalParticles;

        public double TotalSeconds;

        public double Lifespan;

        public double LifespanRand;

        public Vector3 OffsetDir;

        public float MinOffset;

        public float MaxOffset;

        public Vector3 A;

        public float MinA;

        public float MaxA;

        public Vector3 B;

        public float MinB;

        public float MaxB;

        public Vector3 C;

        public float MinC;

        public float MaxC;

        public float StartScale;

        public float FinalScale;

        public float ScaleRand;

        public float StartTrans;

        public float FinalTrans;

        public float TransRand;

        public bool IsParentLocal;

        /// <inheritdoc />
        public override bool Unpack(DatBinReader reader) {
            base.Unpack(reader);
            Unknown = reader.ReadUInt32();
            EmitterType = (EmitterType)reader.ReadInt32();
            ParticleType = (ParticleType)reader.ReadInt32();
            GfxObjId = reader.ReadUInt32();
            HwGfxObjId = reader.ReadUInt32();
            Birthrate = reader.ReadDouble();
            MaxParticles = reader.ReadInt32();
            InitialParticles = reader.ReadInt32();
            TotalParticles = reader.ReadInt32();
            TotalSeconds = reader.ReadDouble();
            Lifespan = reader.ReadDouble();
            LifespanRand = reader.ReadDouble();
            OffsetDir = reader.ReadVector3();
            MinOffset = reader.ReadSingle();
            MaxOffset = reader.ReadSingle();
            A = reader.ReadVector3();
            MinA = reader.ReadSingle();
            MaxA = reader.ReadSingle();
            B = reader.ReadVector3();
            MinB = reader.ReadSingle();
            MaxB = reader.ReadSingle();
            C = reader.ReadVector3();
            MinC = reader.ReadSingle();
            MaxC = reader.ReadSingle();
            StartScale = reader.ReadSingle();
            FinalScale = reader.ReadSingle();
            ScaleRand = reader.ReadSingle();
            StartTrans = reader.ReadSingle();
            FinalTrans = reader.ReadSingle();
            TransRand = reader.ReadSingle();
            IsParentLocal = reader.ReadBool();
            return true;
        }

        /// <inheritdoc />
        public override bool Pack(DatBinWriter writer) {
            base.Pack(writer);
            writer.WriteUInt32(Unknown);
            writer.WriteInt32((int)EmitterType);
            writer.WriteInt32((int)ParticleType);
            writer.WriteUInt32(GfxObjId);
            writer.WriteUInt32(HwGfxObjId);
            writer.WriteDouble(Birthrate);
            writer.WriteInt32(MaxParticles);
            writer.WriteInt32(InitialParticles);
            writer.WriteInt32(TotalParticles);
            writer.WriteDouble(TotalSeconds);
            writer.WriteDouble(Lifespan);
            writer.WriteDouble(LifespanRand);
            writer.WriteVector3(OffsetDir);
            writer.WriteSingle(MinOffset);
            writer.WriteSingle(MaxOffset);
            writer.WriteVector3(A);
            writer.WriteSingle(MinA);
            writer.WriteSingle(MaxA);
            writer.WriteVector3(B);
            writer.WriteSingle(MinB);
            writer.WriteSingle(MaxB);
            writer.WriteVector3(C);
            writer.WriteSingle(MinC);
            writer.WriteSingle(MaxC);
            writer.WriteSingle(StartScale);
            writer.WriteSingle(FinalScale);
            writer.WriteSingle(ScaleRand);
            writer.WriteSingle(StartTrans);
            writer.WriteSingle(FinalTrans);
            writer.WriteSingle(TransRand);
            writer.WriteBool(IsParentLocal);
            return true;
        }

    }

}
