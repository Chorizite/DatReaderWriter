using DatReaderWriter.Enums;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests.Types {
    [TestClass]
    public class MotionDataTests {
        [TestMethod]
        public void CanSerializeAndDeserializeWithOmegaAndVelocity() {
            var writeObj = new MotionData() {
                Bitfield = 0x02,
                Flags = MotionDataFlags.HasOmega | MotionDataFlags.HasVelocity,
                Anims = [
                    new AnimData() {
                        AnimId = 0x12345678,
                        HighFrame = 0x1234,
                        LowFrame = 0x5678,
                        Framerate = 0.12f
                    }
                ],
                Omega = new Vector3(0.1f, 0.2f, 0.3f),
                Velocity = new Vector3(0.4f, 0.5f, 0.6f)
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new MotionData();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Bitfield, readObj.Bitfield);
            Assert.AreEqual(writeObj.Flags, readObj.Flags);
            Assert.AreEqual(writeObj.Anims[0].AnimId, readObj.Anims[0].AnimId);
            Assert.AreEqual(writeObj.Anims[0].HighFrame, readObj.Anims[0].HighFrame);
            Assert.AreEqual(writeObj.Anims[0].LowFrame, readObj.Anims[0].LowFrame);
            Assert.AreEqual(writeObj.Anims[0].Framerate, readObj.Anims[0].Framerate);
            Assert.AreEqual(writeObj.Omega.X, readObj.Omega.X);
            Assert.AreEqual(writeObj.Omega.Y, readObj.Omega.Y);
            Assert.AreEqual(writeObj.Omega.Z, readObj.Omega.Z);
            Assert.AreEqual(writeObj.Velocity.X, readObj.Velocity.X);
            Assert.AreEqual(writeObj.Velocity.Y, readObj.Velocity.Y);
            Assert.AreEqual(writeObj.Velocity.Z, readObj.Velocity.Z);
        }

        [TestMethod]
        public void CanSerializeAndDeserializeWithOmega() {
            var writeObj = new MotionData() {
                Bitfield = 0x02,
                Flags = MotionDataFlags.HasOmega,
                Anims = [
                    new AnimData() {
                        AnimId = 0x12345678,
                        HighFrame = 0x1234,
                        LowFrame = 0x5678,
                        Framerate = 0.12f
                    }
                ],
                Omega = new Vector3(0.1f, 0.2f, 0.3f),
                Velocity = new Vector3(0.4f, 0.5f, 0.6f)
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new MotionData();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Bitfield, readObj.Bitfield);
            Assert.AreEqual(writeObj.Flags, readObj.Flags);
            Assert.AreEqual(writeObj.Anims[0].AnimId, readObj.Anims[0].AnimId);
            Assert.AreEqual(writeObj.Anims[0].HighFrame, readObj.Anims[0].HighFrame);
            Assert.AreEqual(writeObj.Anims[0].LowFrame, readObj.Anims[0].LowFrame);
            Assert.AreEqual(writeObj.Anims[0].Framerate, readObj.Anims[0].Framerate);
            Assert.AreEqual(writeObj.Omega.X, readObj.Omega.X);
            Assert.AreEqual(writeObj.Omega.Y, readObj.Omega.Y);
            Assert.AreEqual(writeObj.Omega.Z, readObj.Omega.Z);
            Assert.AreEqual(Vector3.Zero, readObj.Velocity);
        }

        [TestMethod]
        public void CanSerializeAndDeserializeWithVelocity() {
            var writeObj = new MotionData() {
                Bitfield = 0x02,
                Flags = MotionDataFlags.HasVelocity,
                Anims = [
                    new AnimData() {
                        AnimId = 0x12345678,
                        HighFrame = 0x1234,
                        LowFrame = 0x5678,
                        Framerate = 0.12f
                    }
                ],
                Omega = new Vector3(0.1f, 0.2f, 0.3f),
                Velocity = new Vector3(0.4f, 0.5f, 0.6f)
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new MotionData();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Bitfield, readObj.Bitfield);
            Assert.AreEqual(writeObj.Flags, readObj.Flags);
            Assert.AreEqual(writeObj.Anims[0].AnimId, readObj.Anims[0].AnimId);
            Assert.AreEqual(writeObj.Anims[0].HighFrame, readObj.Anims[0].HighFrame);
            Assert.AreEqual(writeObj.Anims[0].LowFrame, readObj.Anims[0].LowFrame);
            Assert.AreEqual(writeObj.Anims[0].Framerate, readObj.Anims[0].Framerate);
            Assert.AreEqual(writeObj.Velocity.X, readObj.Velocity.X);
            Assert.AreEqual(writeObj.Velocity.Y, readObj.Velocity.Y);
            Assert.AreEqual(writeObj.Velocity.Z, readObj.Velocity.Z);
            Assert.AreEqual(Vector3.Zero, readObj.Omega);
        }

        [TestMethod]
        public void CanSerializeAndDeserializeWithNoOmegaAndNoVelocity() {
            var writeObj = new MotionData() {
                Bitfield = 0x02,
                Flags = MotionDataFlags.None,
                Anims = [
                    new AnimData() {
                        AnimId = 0x12345678,
                        HighFrame = 0x1234,
                        LowFrame = 0x5678,
                        Framerate = 0.12f
                    }
                ],
                Omega = new Vector3(0.1f, 0.2f, 0.3f),
                Velocity = new Vector3(0.4f, 0.5f, 0.6f)
            };

            var buffer = new byte[5_000_000];
            var writer = new DatBinWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new MotionData();
            var reader = new DatBinReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Bitfield, readObj.Bitfield);
            Assert.AreEqual(writeObj.Flags, readObj.Flags);
            Assert.AreEqual(writeObj.Anims[0].AnimId, readObj.Anims[0].AnimId);
            Assert.AreEqual(writeObj.Anims[0].HighFrame, readObj.Anims[0].HighFrame);
            Assert.AreEqual(writeObj.Anims[0].LowFrame, readObj.Anims[0].LowFrame);
            Assert.AreEqual(writeObj.Anims[0].Framerate, readObj.Anims[0].Framerate);
            Assert.AreEqual(Vector3.Zero, readObj.Omega);
            Assert.AreEqual(Vector3.Zero, readObj.Velocity);
        }
    }
}
