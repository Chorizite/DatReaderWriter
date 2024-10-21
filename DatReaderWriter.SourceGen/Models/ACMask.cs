using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DatReaderWriter.SourceGen.Models {
    public class ACMask : ACBaseModel {
        public string Value { get; set; }
        public List<ACBaseModel> ElseChildren { get; set; } = [];

        public ACMask(ACBaseModel parent, XElement element) : base(parent, element) {

        }

        internal static ACMask FromXElement(ACBaseModel parent, XElement element) {
            var value = (string)element.Attribute("value");

            var acmask = new ACMask(parent, element) {
                Value = value
            };

            var elseNode = element.XPathSelectElement("./else");
            if (elseNode != null) {
                acmask.ElseChildren.AddRange(acmask.ParseChildren(elseNode));
            }

            return acmask;
        }
    }
}