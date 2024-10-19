using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.IO;
using ACClientLib.DatReaderWriter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Tests.Types {
    [TestClass]
    public class AnimationFrameTests {
        [TestMethod]
        public void CanSerializeAndDeserialize() {
            var writeObj = new AnimationFrame(1) {
                Frames = [
                    new Frame() {
                        Orientation = new Quaternion(1,2,3,4),
                        Origin = new Vector3(1,2,3),
                    }    
                ],
                Hooks = [
                    new AnimationDoneHook() { Direction = AnimationHookDir.Both },
                    new DefaultScriptHook() { Direction = AnimationHookDir.Backward }
                ]
            };

            var buffer = new byte[5_000_000];
            var writer = new DatFileWriter(buffer);
            writeObj.Pack(writer);

            var readObj = new AnimationFrame(1);
            var reader = new DatFileReader(buffer);
            readObj.Unpack(reader);

            Assert.AreEqual(writeObj.Frames.Count, readObj.Frames.Count);
            Assert.AreEqual(writeObj.Hooks.Count, readObj.Hooks.Count);

            Assert.AreEqual(writeObj.Frames[0].Orientation, readObj.Frames[0].Orientation);
            Assert.AreEqual(writeObj.Frames[0].Origin, readObj.Frames[0].Origin);
            Assert.AreEqual(writeObj.Hooks[0].Direction, readObj.Hooks[0].Direction);
            Assert.AreEqual(writeObj.Hooks[1].Direction, readObj.Hooks[1].Direction);
        }
    }
}
