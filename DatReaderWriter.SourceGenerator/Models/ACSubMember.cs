using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DatReaderWriter.SourceGenerator.Models {
    public class ACSubMember : ACBaseModel {
        public string Text { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Shift { get; set; }
        public string Mask { get; set; }

        public ACSubMember(ACBaseModel parent, XElement element) : base(parent, element) {

        }

        internal static ACSubMember FromXElement(ACBaseModel parent, XElement element) {
            var text = (string)element.Attribute("text");
            var name = (string)element.Attribute("name");
            var type = (string)element.Attribute("type");
            var value = (string)element.Attribute("value");
            var shift = (string)element.Attribute("shift");
            var mask = (string)element.Attribute("mask");

            return new ACSubMember(parent, element) {
                Name = (name),
                Text = text,
                Type = type,
                Value = value,
                Shift = shift,
                Mask = mask
            };
        }
    }
}
