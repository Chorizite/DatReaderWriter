using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System;
using System.Xml.XPath;
using System.Linq;
using DatReaderWriter.SourceGen.Models;

namespace DatReaderWriter.SourceGen {
    public class XMLDefParser {
        private string XmlDefPath;
        public XElement Xml;

        public static Dictionary<string, string> PrimitiveTypeLookup = new Dictionary<string, string>() {
            { "byte", "System.Byte" },
            { "sbyte", "System.SByte" },
            { "short", "System.Int16" },
            { "ushort", "System.UInt16" },
            { "int", "System.Int32" },
            { "uint", "System.UInt32" },
        };

        public readonly Dictionary<string, ACEnum> ACEnums = [];
        public readonly Dictionary<string, ACDataType> ACDataTypes = [];
        public readonly Dictionary<string, ACDataType> ACTemplatedTypes = [];
        public readonly Dictionary<string, ACDBObj> ACDBObjs = [];

        public XMLDefParser(string xmlPath) {
            XmlDefPath = xmlPath;
            using (var stream = new FileStream(xmlPath, FileMode.Open)) {
                Xml = XElement.Load(stream);
            }

            ParseEnums();
            ParseTypes();
            ParseDBObjs();
        }

        private void ParseDBObjs() {
            var nodes = Xml.XPathSelectElements("/dats/dat/type");
            foreach (var node in nodes) {
                var acDataType = ACDBObj.FromXElement(ACDat.FromXElement(node.Parent), node);
                ACDBObjs.Add(acDataType.Name, acDataType);
            }
        }

        private void ParseTypes() {
            var nodes = Xml.XPathSelectElements("/types/type");
            foreach (var node in nodes) {
                var acDataType = ACDataType.FromXElement(null, node);
                ACDataTypes.Add(acDataType.Name, acDataType);
                foreach (var child in acDataType.SubTypes) {
                    ACDataTypes.Add(child.Name, child);
                }
            }
        }

        private void ParseEnums() {
            var nodes = Xml.XPathSelectElements("/enums/enum");
            foreach (var node in nodes) {
                var acEnum = new ACEnum(null, node);
                ACEnums.Add(acEnum.Name, acEnum);
            }
        }
    }
}