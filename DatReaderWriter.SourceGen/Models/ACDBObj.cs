using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DatReaderWriter.SourceGen.Models {
    public class ACDBObj : ACBaseModel {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string Text { get; set; } = "";
        public string BaseType { get; set; } = "";
        public uint FirstId { get; set; }
        public uint LastId { get; set; }
        public bool HasDataCategory { get; set; } = false;

        public ACDBObj(ACBaseModel parent, XElement element) : base(parent, element) {

        }

        public static ACDBObj FromXElement(ACBaseModel parent, XElement element) {
            var name = (string)element.Attribute("name");
            var type = (string)element.Attribute("type");
            var text = (string)element.Attribute("text");
            var baseType = (string)element.Attribute("parent");
            var hasDataCategory = ((string)element.Attribute("categorized") ?? "").ToLower() == "true";
            var firstId = Convert.ToUInt32(element.Attribute("first").Value, 16);
            var lastId = Convert.ToUInt32(element.Attribute("last").Value, 16);
            var message = new ACDBObj(parent, element) {
                Name = name,
                Type = type,
                Text = text,
                BaseType = baseType,
                FirstId = firstId,
                LastId = lastId,
                HasDataCategory = hasDataCategory
            };

            return message;
        }
    }
}