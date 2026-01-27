
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DatReaderWriter.SourceGenerator.Models {
    public class ACDat : ACBaseModel {
        public string Name { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }

        public ACDat(XElement element) : base(null, element) {

        }

        public static ACDat FromXElement(XElement element) {
            return new ACDat(element) {
                Name = element.Attribute("name").Value,
                Text = element.Attribute("text").Value,
                Type = element.Attribute("type").Value,
            };
        }
    }
}
