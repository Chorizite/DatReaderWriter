using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DatReaderWriter.SourceGen.Models {
    public class ACDataMember : ACBaseModel {
        public string Name { get; set; } = "";
        public string MemberType { get; set; } = "";
        public string Text { get; set; } = "";
        public string GenericKey { get; set; } = "";
        public string GenericValue { get; set; } = "";
        public string GenericType { get; set; } = "";
        public string Value { get; set; } = "";
        public string Size { get; set; } = "";

        public List<ACSubMember> SubMembers { get; set; } = new List<ACSubMember>();

        public bool IsLength => Parent.AllChildren.Any(c => c is ACVector v && v.Length.Split(' ').First() == Name);
        public ACVector? LengthFor => Parent.AllChildren.FirstOrDefault(c => c is ACVector v && v.Length.Split(' ').First() == Name) as ACVector;

        public ACDataMember(ACBaseModel parent, XElement element) : base(parent, element) {

        }

        public static ACDataMember FromXElement(ACBaseModel parent, XElement element) {
            var name = (string)element.Attribute("name");
            var memberType = (string)element.Attribute("type");
            var text = (string)element.Attribute("text");
            var genericKey = (string)element.Attribute("genericKey");
            var genericValue = (string)element.Attribute("genericValue");
            var genericType = (string)element.Attribute("genericType");
            var value = (string)element.Attribute("value");
            var size = (string)element.Attribute("size");

            var dataMember = new ACDataMember(parent, element) {
                Name = name,
                Text = text,
                MemberType = memberType,
                GenericKey = genericKey,
                GenericType = genericType,
                GenericValue = genericValue,
                Value = value,
                Size = size
            };

            var subMemberNodes = element.XPathSelectElements("./subfield");
            foreach (var valueNode in subMemberNodes) {
                dataMember.SubMembers.Add(ACSubMember.FromXElement(dataMember, valueNode));
            }

            return dataMember;
        }
    }
}