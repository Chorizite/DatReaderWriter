using DatReaderWriter.Tests.Lib;
using DatReaderWriter;
using DatReaderWriter.Options;
using DatReaderWriter.Enums;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Types;
using System.Numerics;
using Newtonsoft.Json;

namespace DatReaderWriter.Tests.DBObjs {
    [TestClass]
    public class UILayoutTests {
        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEOR() {
            using var dat = new DatCollection(EORCommonData.DatDirectory);

            var res = dat.TryReadFile<LayoutDesc>(0x21000000u, out var layout);
            Assert.IsTrue(res);
            Assert.IsNotNull(layout);
            Assert.AreEqual(0x21000000u, layout.Id);

            Assert.AreEqual(800u, layout.Width);
            Assert.AreEqual(600u, layout.Height);

            Assert.AreEqual(2, layout.Elements.Count);
            Assert.IsTrue(layout.Elements.ContainsKey(0x1000041a));

            Assert.AreEqual(0u, layout.Elements[0x1000041a].ReadOrder, "ReadOrder");
            Assert.AreEqual(0x1000041Au, layout.Elements[0x1000041a].ElementId, "ElementId");
            Assert.AreEqual(3u, layout.Elements[0x1000041a].Type, "Type");
            Assert.AreEqual(0u, layout.Elements[0x1000041a].BaseElement, "BaseElement");
            Assert.AreEqual(0u, layout.Elements[0x1000041a].BaseLayoutId, "BaseLayoutId");
            Assert.AreEqual(StateId.Undef, layout.Elements[0x1000041a].DefaultState, "DefaultState");
            Assert.AreEqual((IncorporationFlags)63, layout.Elements[0x1000041a].StateDesc.IncorporationFlags, "Flags");

            Assert.AreEqual(7, layout.Elements[0x1000041a].Children.Count);

            dat.Dispose();
        }

        [TestMethod]
        [TestCategory("EOR")]
        public void CanReadEORAndWriteIdentical() {
            using var datCollection = new DatCollection(EORCommonData.DatDirectory);
            TestHelpers.CanReadAndWriteIdentical<LayoutDesc>("", 0x21000000u, datCollection.Local);
            TestHelpers.CanReadAndWriteIdentical<LayoutDesc>("", 0x21000001u, datCollection.Local);
            TestHelpers.CanReadAndWriteIdentical<LayoutDesc>("", 0x21000028u, datCollection.Local);
            TestHelpers.CanReadAndWriteIdentical<LayoutDesc>("", 0x21000075u, datCollection.Local);
        }
    }
}
