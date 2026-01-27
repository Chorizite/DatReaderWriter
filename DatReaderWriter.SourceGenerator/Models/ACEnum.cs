using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DatReaderWriter.SourceGenerator.Models {
    public class ACEnum : ACBaseModel {
        public string Name { get; set; }
        public string ParentType { get; set; }
        public string Text { get; set; }
        public bool IsMask { get => Element.Attribute("mask")?.Value?.ToLower() == "true"; }

        public List<ACEnumValue> Values { get; set; }

		public ACEnum(string name, string text) : base(null, null) { 
            Name = name;
            Text = text;
        }

        public ACEnum(ACBaseModel parent, XElement element) : base(parent, element) {
			Name = element.Attribute("name")?.Value;
            Text = element.Attribute("text")?.Value;
            ParentType = element.Attribute("parent")?.Value;
            Values = element.XPathSelectElements("./*[self::value or self::mask]").Select(
					e => new ACEnumValue(this, e)
				).ToList();
		}
    }
}
