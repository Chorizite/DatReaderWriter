using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DatReaderWriter.SourceGen.Models {
    public class ACIf : ACBaseModel {
        public string Text { get; set; }
        public string Condition { get; set; }
        public string WriteCondition { get; set; }

        public List<ACBaseModel> Members { get; set; } = new List<ACBaseModel>();

        public ACIf(ACBaseModel parent, XElement element) : base(parent, element) {

        }

        internal static ACIf FromXElement(ACBaseModel parent, XElement element) {
            var text = (string)element.Attribute("text");
            var condition = ((string)element.Attribute("condition"));
            var writeCondition = ((string)element.Attribute("writeCondition"));

            var acif = new ACIf(parent, element) {
                Text = text,
                Condition = condition,
                WriteCondition = writeCondition
            };

            acif.Members = acif.ParseChildren(element.XPathSelectElement("./true"));

            acif.Children.AddRange(acif.Members);

            return acif;
        }
    }
}
