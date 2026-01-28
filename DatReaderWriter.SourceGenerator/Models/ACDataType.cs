
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DatReaderWriter.SourceGenerator.Models {
    public class ACDataType : ACBaseModel {
        public string Name { get; set; }
        public string Primitive { get; set; }
        public string ParentType { get; private set; }
        public string BaseType { get; set; }
        public string Param { get; set; }
        public string Text { get; set; }
        public string TypeSwitch { get; set; }
        public string Value { get; set; }
        public string Size { get; set; }
        public bool IsTemplated { get; set; }
        public bool HasDataCategory { get; set; } = false;
        public bool IsAbstract { get; set; } = false;
        public List<ACDataType> SubTypes { get; set; } = new List<ACDataType>();
        public bool IsAbstractImplementation { get; set; } = false;
        public string GenericKey { get; set; }
        public string GenericValue { get; set; }
        public bool IsAutoGrow { get; set; }

        public ACDataType(ACBaseModel parent, XElement element) : base(parent, element) {
        }

        public string GetParameterType() {
            var el = Element.XPathSelectElements("./field")
                .FirstOrDefault(e => e.Attribute("name")?.Value == TypeSwitch);

            return el.Attribute("type").Value;
        }

        public static ACDataType FromXElement(ACBaseModel parent, XElement element) {
            var name = (string)element.Attribute("name");
            var primitive = ((string)element.Attribute("primitive"));
            var parentType = ((string)element.Attribute("parent"));
            var baseType = ((string)element.Attribute("baseType"));
            var param = ((string)element.Attribute("param"));
            var text = ((string)element.Attribute("text"));
            var value = ((string)element.Attribute("value"));
            var templated = ((string)element.Attribute("templated"));
            var size = ((string)element.Attribute("size"));
            var hasDataCategory = ((string)element.Attribute("category"))?.ToLower() == "true";
            var isAbstract = ((string)element.Attribute("abstract"))?.ToLower() == "true";

            var type = new ACDataType(parent, element) {
                Name = name ?? "",
                Primitive = primitive ?? "",
                ParentType = parentType ?? "",
                BaseType = baseType ?? "",
                Param = param ?? "",
                Text = text ?? "",
                Value = value ?? "",
                IsTemplated = !string.IsNullOrWhiteSpace(templated),
                Size = size ?? "",
                HasDataCategory = hasDataCategory,
                IsAbstract = isAbstract,
                TypeSwitch = element.XPathSelectElement("./typeswitch")?.Attribute("name")?.Value ?? "",
                GenericKey = (string)element.Attribute("genericKey") ?? "",
                GenericValue = (string)element.Attribute("genericValue") ?? "",
                IsAutoGrow = ((string)element.Attribute("isAutoGrow"))?.ToLower() == "true",
            };

            var subTypeNodes = element.XPathSelectElements("./typeswitch/type");
            foreach (var valueNode in subTypeNodes) {
                var subType = ACDataType.FromXElement(type, valueNode);
                subType.ParentType = name;
                subType.IsAbstractImplementation = true;
                type.SubTypes.Add(subType);
            }

            return type;
        }
    }
}
