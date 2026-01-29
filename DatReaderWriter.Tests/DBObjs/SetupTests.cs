using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using System.Numerics;
using DatReaderWriter.Types;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class SetupTests {
        [TestMethod]
        public void CanInsertAndReadSetups() {
            var datFilePath = Path.GetTempFileName();
            using var dat = new DatDatabase(options => {
                options.FilePath = datFilePath;
                options.AccessType = DatAccessType.ReadWrite;
            });

            dat.BlockAllocator.InitNew(DatFileType.Portal, 0);

            var xpTable = new Setup() {
                Id = 0xE000018,
                Flags = SetupFlags.AllowFreeHeading | SetupFlags.HasParent,
                DefaultAnimation = 0x12345678,
                ParentIndex = [ 0x48283123 ],
                Parts = [ 0x01000657 ],
                Radius = 0.5f,
                PlacementFrames = new Dictionary<Placement, AnimationFrame>() {
                    { Placement.LeftUnarmed, new AnimationFrame(1) {
                        Frames = [
                            new Frame() {
                                Origin = new Vector3(1, 2, 3),
                                Orientation = new Quaternion(4, 5, 6, 7),
                            }
                        ]
                    } }
                },
                SelectionSphere = new Sphere() { Origin = new Vector3(8, 9, 10), Radius = 11 },
                SortingSphere = new Sphere() { Origin = new Vector3(12, 13, 14), Radius = 15 },
            };

            var res = dat.TryWriteFile(xpTable);
            Assert.IsTrue(res);

            var res2 = dat.TryGet<Setup>(0xE000018, out var setup);
            Assert.IsTrue(res2);
            Assert.IsNotNull(setup);
            Assert.AreEqual(0xE000018u, setup.Id);

            Assert.AreEqual(SetupFlags.AllowFreeHeading | SetupFlags.HasParent, setup.Flags);

            Assert.AreEqual(0x12345678u, setup.DefaultAnimation);
            Assert.AreEqual(1, setup.Parts.Count);
            Assert.AreEqual(0x01000657u, setup.Parts[0]);

            Assert.AreEqual(1, setup.ParentIndex.Count);
            Assert.AreEqual(0x48283123u, setup.ParentIndex[0]);
            Assert.AreEqual(0.5f, setup.Radius);

            Assert.AreEqual(1, setup.PlacementFrames.Count);
            Assert.AreEqual(1, setup.PlacementFrames[Placement.LeftUnarmed].Frames.Count);
            Assert.AreEqual(new Vector3(1, 2, 3), setup.PlacementFrames[Placement.LeftUnarmed].Frames[0].Origin);
            Assert.AreEqual(new Quaternion(4, 5, 6, 7), setup.PlacementFrames[Placement.LeftUnarmed].Frames[0].Orientation);

            Assert.AreEqual(new Vector3(8, 9, 10), setup.SelectionSphere.Origin);
            Assert.AreEqual(11, setup.SelectionSphere.Radius);
            Assert.AreEqual(new Vector3(12, 13, 14), setup.SortingSphere.Origin);
            Assert.AreEqual(15, setup.SortingSphere.Radius);

            dat.Dispose();
            File.Delete(datFilePath);
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORSetup() {
            using var dat = new DatCollection(EORCommonData.DatDirectory);

            var res = dat.TryGet<Setup>(0x02000108, out var setup1);
            Assert.IsTrue(res);
            Assert.IsNotNull(setup1);

            Assert.AreEqual(SetupFlags.HasPhysicsBSP, setup1.Flags);

            Assert.AreEqual(1, setup1.Parts.Count);
            Assert.AreEqual(0x010008ABu, setup1.Parts[0]);

            var part = setup1.Parts.First().Get(dat);
            Assert.IsNotNull(part);
            Assert.AreEqual(setup1.Parts.First().DataId, part.Id);

            Assert.AreEqual(1, setup1.PlacementFrames.Count);
            Assert.AreEqual(0, setup1.PlacementFrames[Placement.Default].Hooks.Count);
            Assert.AreEqual(1, setup1.PlacementFrames[Placement.Default].Frames.Count);
            Assert.AreEqual(Vector3.Zero, setup1.PlacementFrames[Placement.Default].Frames[0].Origin);
            Assert.AreEqual(new Quaternion(0, 0, 0, 1), setup1.PlacementFrames[Placement.Default].Frames[0].Orientation);

            Assert.AreEqual(0, setup1.CylSpheres.Count);
            Assert.AreEqual(1, setup1.Spheres.Count);
            Assert.AreEqual(new Vector3(0, -0.1475f, 0.439f), setup1.Spheres[0].Origin);
            Assert.AreEqual(0.439f, setup1.Spheres[0].Radius);

            Assert.AreEqual(0.878f, setup1.Height);
            Assert.AreEqual(0.5269661f, setup1.Radius);
            Assert.AreEqual(0.3612f, setup1.StepUpHeight);
            Assert.AreEqual(0.3612f, setup1.StepDownHeight);

            Assert.AreEqual(new Vector3(0.000971317f, -0.0658602f, 0.439f), setup1.SortingSphere.Origin);
            Assert.AreEqual(0.520641f, setup1.SortingSphere.Radius);

            Assert.AreEqual(new Vector3(0f, 0f, 0.439f), setup1.SelectionSphere.Origin);
            Assert.AreEqual(0.5269661f, setup1.SelectionSphere.Radius);
            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORSetupWithParents() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.OnDemand;
            });

            var res = dat.TryGet<Setup>(0x02000172, out var setup2);
            Assert.IsTrue(res);
            Assert.IsNotNull(setup2);

            Assert.AreEqual(SetupFlags.HasParent | SetupFlags.AllowFreeHeading, setup2.Flags);

            Assert.AreEqual(1, setup2.Parts.Count);
            Assert.AreEqual(0x01000657u, setup2.Parts[0]);

            Assert.AreEqual(1, setup2.ParentIndex.Count);
            Assert.AreEqual(0xFFFFFFFFu, setup2.ParentIndex[0]);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORSetupWithLights() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.OnDemand;
            });

            var res = dat.TryGet<Setup>(0x02000229, out var setup3);
            Assert.IsTrue(res);
            Assert.IsNotNull(setup3);

            Assert.AreEqual(SetupFlags.HasParent | SetupFlags.AllowFreeHeading, setup3.Flags);

            Assert.AreEqual(1, setup3.Parts.Count);
            Assert.AreEqual(0x010009BFu, setup3.Parts[0]);

            Assert.AreEqual(1, setup3.Lights.Count);
            Assert.AreEqual(new Vector3(-0.0325001f, 0.0525f, 0.4975f), setup3.Lights[0].ViewSpaceLocation.Origin);
            Assert.AreEqual(new Quaternion(-0.277816f, 0.115075f, -0.364972f, 0.88112f), setup3.Lights[0].ViewSpaceLocation.Orientation);
            Assert.AreEqual(100f, setup3.Lights[0].Intensity);
            Assert.AreEqual(5f, setup3.Lights[0].Falloff);
            Assert.AreEqual(-431602080f, setup3.Lights[0].ConeAngle);
            Assert.AreEqual(255, setup3.Lights[0].Color.Alpha);
            Assert.AreEqual(250, setup3.Lights[0].Color.Red);
            Assert.AreEqual(200, setup3.Lights[0].Color.Green);
            Assert.AreEqual(0, setup3.Lights[0].Color.Blue);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORSetupWithDefaultScales() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.OnDemand;
            });

            var res = dat.TryGet<Setup>(0x02000240, out var setup4);
            Assert.IsTrue(res);
            Assert.IsNotNull(setup4);

            Assert.AreEqual(SetupFlags.HasParent | SetupFlags.AllowFreeHeading | SetupFlags.HasDefaultScale, setup4.Flags);

            Assert.AreEqual(5, setup4.Parts.Count);
            Assert.AreEqual(0x01003769u, setup4.Parts[0]);
            Assert.AreEqual(0x0100376Du, setup4.Parts[4]);

            Assert.AreEqual(5, setup4.ParentIndex.Count);
            Assert.AreEqual(5, setup4.DefaultScale.Count);
            Assert.AreEqual(0.532f, setup4.DefaultScale[0].X);
            Assert.AreEqual(0.532f, setup4.DefaultScale[0].Y);
            Assert.AreEqual(0.532f, setup4.DefaultScale[0].Z);
            Assert.AreEqual(0.795999f, setup4.DefaultScale[4].X);
            Assert.AreEqual(0.795999f, setup4.DefaultScale[4].Y);
            Assert.AreEqual(0.795999f, setup4.DefaultScale[4].Z);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORSetupsWithHoldingLocations() {
            using var dat = new DatDatabase(options => {
                options.FilePath = Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat");
                options.IndexCachingStrategy = IndexCachingStrategy.OnDemand;
            });

            var res = dat.TryGet<Setup>(0x0200185A, out var setup5);
            Assert.IsTrue(res);
            Assert.IsNotNull(setup5);

            Assert.AreEqual(SetupFlags.HasParent | SetupFlags.AllowFreeHeading, setup5.Flags);

            Assert.AreEqual(9, setup5.HoldingLocations.Count);

            Assert.AreEqual(9, setup5.HoldingLocations[ParentLocation.None].PartId);
            Assert.AreEqual(new Vector3(0, 0.1f, 0.1825f), setup5.HoldingLocations[ParentLocation.None].Frame.Origin);
            Assert.AreEqual(new Quaternion(0, 0, 0, 1), setup5.HoldingLocations[ParentLocation.None].Frame.Orientation);

            Assert.AreEqual(12, setup5.HoldingLocations[ParentLocation.LeftWeapon].PartId);
            Assert.AreEqual(new Vector3(-0.045f , 0, -0.076f), setup5.HoldingLocations[ParentLocation.LeftWeapon].Frame.Origin);
            Assert.AreEqual(new Quaternion(0.707107f, 0, 0, 0.707107f), setup5.HoldingLocations[ParentLocation.LeftWeapon].Frame.Orientation);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            TestHelpers.CanReadAndWriteIdentical<Setup>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x0200185A);
            TestHelpers.CanReadAndWriteIdentical<Setup>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x02000240);
            TestHelpers.CanReadAndWriteIdentical<Setup>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x02000229);
            TestHelpers.CanReadAndWriteIdentical<Setup>(Path.Combine(EORCommonData.DatDirectory, $"client_portal.dat"), 0x02000172);
        }
    }
}
