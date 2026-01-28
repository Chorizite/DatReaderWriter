using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DatReaderWriter.SourceGenerator.Models {
    public class ACVector : ACBaseModel {
        public string Name { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public string Length { get; set; }
        public string LengthMod { get; set; }
        public string Skip { get; set; }

        public string GenericKey { get; set; }
        public string GenericValue { get; set; }

        public string TypeDeclaration {
            get {
				var typeDeclaration = "";
				var typeBase = "";
				return typeDeclaration;
			}
        }

        public ACVector(ACBaseModel parent, XElement element) : base(parent, element) {

        }

        public static ACVector FromXElement(ACBaseModel parent, XElement element) {
            return new ACVector(parent, element) {
                Name = (string)element.Attribute("name") ?? "",
                Type = (string)element.Attribute("type") ?? "",
                Length = (string)element.Attribute("length") ?? "",
                LengthMod = (string)element.Attribute("lengthmod") ?? "",
                Text = (string)element.Attribute("text") ?? "",
                Skip = (string)element.Attribute("skip") ?? "",
                GenericKey = (string)element.Attribute("genericKey") ?? "",
                GenericValue = (string)element.Attribute("genericValue") ?? ""
            };
        }
    }
}