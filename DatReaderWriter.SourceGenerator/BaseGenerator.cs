using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using DatReaderWriter.SourceGenerator.Models;
using Microsoft.CodeAnalysis;

namespace DatReaderWriter.SourceGenerator {
    public abstract class BaseGenerator {
        protected XMLDefParser XMLDefParser { get; }
        
        public BaseGenerator(XMLDefParser parser) {
            XMLDefParser = parser;
        }

        public abstract void Generate(SourceProductionContext spc, XMLDefParser parser);

        public void WriteSummary(SourceWriter writer, string text) {
            if (string.IsNullOrWhiteSpace(text))
                return;

            writer.WriteLine("/// <summary>");
            writer.WriteLine("/// " + WebUtility.HtmlEncode(text.Trim()));
            writer.WriteLine("/// </summary>");
        }

        public string SimplifyType(string type) {
            if (type == "ObjectId" || type == "PackedDWORD" || type == "LandcellId" || type == "DataId") {
                return "uint";
            }
            if (type == "WString" || type == "bytestring" || type == "obfuscatedstring" || type=="rawstring" || type == "compressedstring" || type == "ushortstring") {
                return "string";
            }
            if (type == "CompressedUInt") {
                return "uint";
            }
            if (type == "DataIdOfKnownType") {
                return "uint";
            }
             if (type == "guid") {
                return "Guid";
            }
            return type;
        }

        public string GetTypeDeclaration(ACDataMember member) {
            var simplifiedType = SimplifyType(member.MemberType);
            if (XMLDefParser.ACTemplatedTypes.ContainsKey(member.MemberType)) {
                simplifiedType = XMLDefParser.ACTemplatedTypes[member.MemberType].ParentType;
            }
            if (!string.IsNullOrWhiteSpace(member.GenericKey) && !string.IsNullOrWhiteSpace(member.GenericValue))
                return $"{simplifiedType}<{SimplifyType(member.GenericKey)}, {SimplifyType(member.GenericValue)}>";
            if (!string.IsNullOrWhiteSpace(member.GenericType))
                return $"{simplifiedType}<{SimplifyType(member.GenericType)}>";
            return simplifiedType;
        }

        public string GetTypeDeclaration(ACDataType member) {
            foreach (var child in member.Children) {
                if (child is ACVector) {
                    var memberTypes = child.Children.Where(c => c is ACDataMember).Select(
                        c => SimplifyType((c as ACDataMember).MemberType)
                    );
                    return member.Name + "<" + string.Join(", ", memberTypes) + ">";
                }
            }
            return member.Name;
        }

         public string GetTypeDeclaration(ACVector vector) {
            if (!string.IsNullOrEmpty(vector.GenericKey)) {
                return $"{SimplifyType(vector.Type)}<{SimplifyType(vector.GenericKey)}, {SimplifyType(vector.GenericValue)}>";
            }
            else if (!string.IsNullOrEmpty(vector.GenericValue)) {
                return $"{SimplifyType(vector.Type)}<{SimplifyType(vector.GenericValue)}>";
            }
            else {
                return $"{SimplifyType(vector.Type)}[]";
            }
        }
        
        public string GetBinaryReaderForType(string type, string? size = null) {
            switch (type) {
                case "WORD":
                case "SpellId":
                case "ushort":
                    return "reader.ReadUInt16()";
                case "short":
                    return "reader.ReadInt16()";
                case "DWORD":
                case "ObjectId":
                case "LandcellId":
                case "uint":
                    return "reader.ReadUInt32()";
                case "int":
                    return "reader.ReadInt32()";
                case "ulong":
                    return "reader.ReadUInt64()";
                case "long":
                    return "reader.ReadInt64()";
                case "float":
                    return "reader.ReadSingle()";
                case "double":
                    return "reader.ReadDouble()";
                case "bool":
                    return $"reader.ReadBool({size})";
                case "byte":
                    return "reader.ReadByte()";
                case "sbyte":
                    return "reader.ReadSByte()";
                case "string":
                    return "reader.ReadString16L()";
                case "bytestring":
                    return "reader.ReadString16LByte()";
                case "compressedstring":
                    return "reader.ReadStringCompressed()";
                case "WString":
                    return "reader.ReadString32L()";
                case "PackedWORD":
                    return "reader.ReadPackedWORD()";
                case "Vector3":
                    return "reader.ReadVector3()";
                case "Quaternion":
                    return "reader.ReadQuaternion()";
                case "DataID":
                case "DataId":
                case "PackedDWORD":
                    return "reader.ReadPackedDWORD()";
                case "CompressedUInt":
                    return "reader.ReadCompressedUInt()";
                case "DataIdOfKnownType":
                    return $"reader.ReadDataIdOfKnownType({size})";
                case "obfuscatedstring":
                    return $"reader.ReadObfuscatedString()";
                case "rawstring":
                    return $"reader.ReadString()";
                case "ushortstring":
                    return $"reader.ReadUShortString()";
                case "guid":
                    return $"reader.ReadGuid()";
                default:
                    return $"reader.ReadItem<{type}>()";
            }
        }

         public string GetBinaryWriterForType(string type) {
            switch (type) {
                case "WORD":
                case "SpellId":
                case "ushort":
                    return "WriteUInt16";
                case "short":
                    return "WriteInt16";
                case "DWORD":
                case "ObjectId":
                case "LandcellId":
                case "uint":
                    return "WriteUInt32";
                case "int":
                    return "WriteInt32";
                case "ulong":
                    return "WriteUInt64";
                case "long":
                    return "WriteInt64";
                case "float":
                    return "WriteSingle";
                case "double":
                    return "WriteDouble";
                case "bool":
                    return "WriteBool";
                case "byte":
                    return "WriteByte";
                case "sbyte":
                    return "WriteSByte";
                case "ushortstring":
                    return "WriteUShortString";
                case "rawstring":
                    return "WriteString";
                case "string":
                    return "WriteString16L";
                case "compressedstring":
                    return "WriteStringCompressed";
                case "bytestring":
                    return "WriteString16LByte";
                case "WString":
                    return "WriteString32L";
                case "Vector3":
                    return "WriteVector3";
                case "Quaternion":
                    return "WriteQuaternion";
                case "CompressedUInt":
                    return "WriteCompressedUInt";
                case "DataIdOfKnownType":
                    return $"WriteDataIdOfKnownType";
                case "obfuscatedstring":
                    return $"WriteObfuscatedString";
                case "guid":
                    return $"WriteGuid";
                default:
                    return $"WriteItem<{type}>";
            }
        }

        public void GenerateClassProperties(SourceWriter writer, ACBaseModel baseModel, ref List<string> usedNames) {
            if (baseModel is ACDataMember) {
                var member = baseModel as ACDataMember;
                // we only want to include properties we haven't included yet
                if (!usedNames.Contains(member.Name)) {
                    usedNames.Add(member.Name);
                    WriteClassProperty(writer, member);
                }

                foreach (var submember in member.SubMembers) {
                    WriteClassProperty(writer, submember);
                }
            }
            else if (baseModel is ACVector) {
                var vector = baseModel as ACVector;
                WriteClassVectorProperty(writer, vector);

                return; // dont generate child fields of vectors
            }

            // recursively go down the chain and add any found attributes
            foreach (var child in baseModel.Children) {
                GenerateClassProperties(writer, child, ref usedNames);
            }
        }

        public void WriteClassProperty(SourceWriter writer, ACDataMember member) {
            if (member.Name.StartsWith("_") && member.Parent is ACDataType dType && dType.TypeSwitch == member.Name) {
                WriteSummary(writer, member.Text);
                writer.WriteLine("public abstract " + GetTypeDeclaration(member) + " " + member.Name.Substring(1, 1).ToUpper() + member.Name.Substring(2) + " { get; }");
                writer.WriteLine("");
                return;
            }
            if (member.Name.StartsWith("_")) return;
            
            WriteSummary(writer, member.Text);
            if (XMLDefParser.ACTemplatedTypes.ContainsKey(member.MemberType)) {
                writer.WriteLine("public " + GetTypeDeclaration(member) + " " + member.Name + " = new();");
            }
            else {
                writer.WriteLine("public " + GetTypeDeclaration(member) + " " + member.Name + ";");
            }
            writer.WriteLine("");
        }

        public void WriteClassProperty(SourceWriter writer, ACSubMember member) {
            if (member.Name.StartsWith("_")) return;

            var parent = member.Parent as ACDataMember;
            var simplifiedType = SimplifyType(member.Type);
            WriteSummary(writer, member.Text);
            writer.WriteLine("public " + simplifiedType + " " + member.Name + ";");
            writer.WriteLine("");
        }

        public void WriteClassVectorProperty(SourceWriter writer, ACVector vector) {
            if (vector.Children.Count > 1)
                 WriteVectorItemClassDefinition(writer, vector);

            WriteSummary(writer, vector.Text);
            writer.WriteLine($"public {GetTypeDeclaration(vector)} {vector.Name} = [];");
            writer.WriteLine("");
        }

        private void WriteVectorItemClassDefinition(SourceWriter writer, ACVector vector) {
            WriteSummary(writer, $"Vector collection data for the {vector.Name} vector");
            writer.WriteLine($"public class {GetVectorClassName(vector)} {{");
            using (writer.IndentScope()) {
                var usedNames = new List<string>();
                foreach (var child in vector.Children) {
                    GenerateClassProperties(writer, child, ref usedNames);
                }
            }
            writer.WriteLine("}");
            writer.WriteLine("");
        }

        private string GetVectorClassName(ACVector vector, bool fullyQualified = false) {
             var parent = "";
            if (fullyQualified && vector.HasParentOfType(typeof(ACVector))) {
                var p = vector.GetParentOfType<ACVector>();
                while (p != null) {
                    parent = GetVectorClassName(p) + "." + parent;
                    p = p.GetParentOfType<ACVector>();
                }
            }
            return vector.Type;
        }

        public void GenerateReaderContents(SourceWriter writer, ACBaseModel model, int depth) {
            if (model is ACDataMember) {
                GenerateMemberReader(writer, model as ACDataMember);
            }
            else if (model is ACIf) {
                var acif = model as ACIf;
                writer.WriteLine($"if ({acif.Condition}) {{");
                using(writer.IndentScope()) {
                    foreach (var member in acif.Children) {
                        GenerateReaderContents(writer, member, depth);
                    }
                }
                writer.WriteLine("}");
            }
             else if (model is ACMaskMap) {
                 // Simplified mask map logic for now, expanding slightly but keeping it somewhat raw
                var maskMap = model as ACMaskMap;
                foreach (var mask in maskMap.Masks) {
                    WriteMaskMapCheckStart(writer, maskMap, mask);
                    using(writer.IndentScope()) {
                        foreach (var maskChild in mask.Children) {
                            GenerateReaderContents(writer, maskChild, depth);
                        }
                    }
                    writer.WriteLine("}"); // End if
                    
                    if (mask.ElseChildren.Count > 0) {
                        writer.WriteLine("else {");
                        using(writer.IndentScope()) {
                            foreach (var elseChild in mask.ElseChildren) {
                                GenerateReaderContents(writer, elseChild, depth);
                            }
                        }
                        writer.WriteLine("}");
                    }
                }
            }
            else if (model is ACAlign) {
                 writer.WriteLine($"reader.Align(4);");
            }
             else if (model is ACSwitch) {
                var acswitch = model as ACSwitch;
                 // switches need to define locally used variables first
                var usedNames = new List<string>();
                 if (model.Parent.GetType() != typeof(ACDataType) && model.Parent.GetType() != typeof(ACDBObj)) {
                    foreach (var scase in acswitch.Cases) {
                        if (acswitch.Parent is ACSwitchCase)
                            break;
                         foreach (ACBaseModel child in scase.AllChildren.Where(c => c is ACDataMember || c is ACVector)) {
                            if (child is ACDataMember) {
                                var dm = child as ACDataMember;
                                if (usedNames.Contains(dm.Name))
                                    continue;
                                WriteLocalChildDefinition(writer, dm);
                                usedNames.Add(dm.Name);
                            }
                            else if (child is ACVector) {
                                WriteLocalVectorDefinition(writer, child as ACVector);
                            }
                        }
                    }
                }

                writer.WriteLine($"switch({acswitch.Name}) {{");
                using (writer.IndentScope()) {
                     foreach (var scase in acswitch.Cases) {
                         var cases = scase.Value.Split(new string[] { " | " }, StringSplitOptions.None);
                        foreach (var s in cases) {
                            writer.WriteLine("case " + s + ":");
                        }
                        using(writer.IndentScope()) {
                             foreach (var child in scase.Children) {
                                GenerateReaderContents(writer, child, depth);
                            }
                            writer.WriteLine("break;");
                        }
                     }
                }
                writer.WriteLine("}");
            }
            else if (model is ACVector) {
                 var vector = model as ACVector;
                 if (vector.Parent is ACVector) {
                    WriteLocalVectorDefinition(writer, vector);
                }

                if (string.IsNullOrEmpty(vector.GenericKey) && string.IsNullOrEmpty(vector.GenericValue)) {
                    if (!string.IsNullOrEmpty(vector.LengthMod)) {
                        writer.WriteLine($"{vector.Name} = new {SimplifyType(vector.Type)}[{vector.Length} + {vector.LengthMod}];");
                    }
                    else {
                        writer.WriteLine($"{vector.Name} = new {SimplifyType(vector.Type)}[{vector.Length}];");
                    }
                }

                var indexVar = new string[] { "i", "x", "y", "z", "q", "p", "t", "r", "f", "g", "h", "k", "u", "v" }[depth];
                 if (!string.IsNullOrEmpty(vector.Skip)) {
                    writer.WriteLine($"for (var {indexVar}=0; {indexVar} < {vector.Length} - {vector.Skip}; {indexVar}++) {{");
                }
                else if (!string.IsNullOrEmpty(vector.LengthMod)) {
                    writer.WriteLine($"for (var {indexVar}=0; {indexVar} < {vector.Length} + {vector.LengthMod}; {indexVar}++) {{");
                }
                else {
                    writer.WriteLine($"for (var {indexVar}=0; {indexVar} < {vector.Length.Replace("parent.", "")}; {indexVar}++) {{");
                }
                
                using(writer.IndentScope()) {
                     foreach (var vmember in vector.Children) {
                        GenerateReaderContents(writer, vmember, depth + 1);
                    }
                    WriteVectorPusher(writer, vector, indexVar);
                }
                writer.WriteLine("}");
            }
        }

        public void WriteMaskMapCheckStart(SourceWriter writer, ACMaskMap maskMap, ACMask mask) {
             if (!string.IsNullOrEmpty(maskMap.XOR)) {
                writer.WriteLine($"if ((((uint){maskMap.Name} ^ {maskMap.XOR}) & {mask.Value}) != 0) {{");
            }
            else if (mask.Value.StartsWith("0x")) {
                writer.WriteLine($"if (((uint){maskMap.Name} & (uint){mask.Value}) != 0) {{");
            }
            else {
                 var parts = mask.Value
                    .Split('|').Select(v => v.Trim())
                    .Select(a => {
                        if (a.StartsWith("!")) {
                            return $"!{maskMap.Name}.HasFlag({a.Substring(1)})";
                        }
                        else {
                            return $"{maskMap.Name}.HasFlag({a})";
                        }
                    });
                writer.WriteLine($"if ({string.Join(" || ", parts)}) {{");
            }
        }

        public void WriteLocalChildDefinition(SourceWriter writer, ACDataMember child) {
            var type = SimplifyType(child.MemberType);
             switch (type) {
                case "SpellID":
                case "ushort":
                case "short":
                case "ObjectID":
                case "uint":
                case "int":
                case "ulong":
                case "long":
                case "float":
                case "double":
                case "sbyte":
                case "byte":
                    writer.WriteLine($"{type} {child.Name} = 0;");
                    break;
                case "bool":
                    writer.WriteLine($"{type} {child.Name} = false;");
                    break;
                case "string":
                case "bytestring":
                case "rawstring":
                case "compressedstring":
                case "ushortstring":
                case "WString":
                    writer.WriteLine($"{type} {child.Name} = \"\";");
                    break;
                default:
                    writer.WriteLine($"{type} {child.Name} = new();");
                    break;
            }
        }

        public void WriteLocalVectorDefinition(SourceWriter writer, ACVector vector) {
            writer.WriteLine($"var {vector.Name} = new {GetTypeDeclaration(vector)}();");
        }

        public void GenerateMemberReader(SourceWriter writer, ACDataMember member) {
             bool isVector = member.HasParentOfType(typeof(ACVector));
              var type = XMLDefParser.ACDataTypes.Values
                   .FirstOrDefault(t => t.Name == member.MemberType);
            
            if (type != null && type.IsAbstract) {
                 var field = type.Children
                    .FirstOrDefault(c => c is ACDataMember m && m.Name == type.TypeSwitch)
                    as ACDataMember;

                if (XMLDefParser.ACEnums.TryGetValue(field.MemberType, out var en)) {
                    var reader = GetBinaryReaderForType(XMLDefParser.ACEnums[field.MemberType].ParentType, null);
                    writer.WriteLine($"var _peekedValue = (" + field.MemberType + ")" + reader + ";");
                }
                else {
                    writer.WriteLine($"var _peekedValue = {GetBinaryReaderForType(field.MemberType, null)};");
                }
                writer.WriteLine($"reader.Skip(-sizeof({field.MemberType}));");
                writer.WriteLine($"{member.Name} = {member.MemberType}.Unpack(reader, _peekedValue);");
                return;
            }

            if (XMLDefParser.ACEnums.ContainsKey(member.MemberType)) {
                var reader = GetBinaryReaderForType(XMLDefParser.ACEnums[member.MemberType].ParentType);
                if (member.HasParentOfType(typeof(ACVector)) || member.Name.StartsWith("_")) {
                     writer.WriteLine("var " + member.Name + " = (" + member.MemberType + ")" + reader + ";");
                }
                else {
                    writer.WriteLine(member.Name + " = (" + member.MemberType + ")" + reader + ";");
                }
            }
            else if (XMLDefParser.ACDataTypes.ContainsKey(member.MemberType)) {
                 if (member.MemberType == "Vector3") {
                    writer.WriteLine($"{member.Name} = reader.ReadVector3();");
                    return;
                }
                if (member.MemberType == "Quaternion") {
                    writer.WriteLine($"{member.Name} = reader.ReadQuaternion();");
                    return;
                }
                writer.WriteLine($"{member.Name} = reader.ReadItem<{member.MemberType}>();");
            }
            else if (XMLDefParser.ACTemplatedTypes.ContainsKey(member.MemberType)) {
                if (!string.IsNullOrEmpty(member.GenericType)) {
                    writer.WriteLine($"{member.Name} = reader.Read{member.MemberType}<{SimplifyType(member.GenericType)}>();");
                }
                else {
                    writer.WriteLine($"{member.Name} = reader.Read{member.MemberType}<{SimplifyType(member.GenericKey)}, {SimplifyType(member.GenericValue)}>();");
                }
            }
            else {
                 if (member.Parent is ACVector || member.IsLength || member.Name.StartsWith("_")) {
                    writer.WriteLine("var " + member.Name + " = " + GetBinaryReaderForType(member.MemberType, member.Size) + ";");
                    if (member.SubMembers.Count > 0) {
                        foreach (var sub in member.SubMembers) {
                            if (!string.IsNullOrEmpty(sub.Mask) && !string.IsNullOrEmpty(sub.Shift)) {
                                writer.WriteLine($"{sub.Name} = ({SimplifyType(sub.Type)})(({member.Name} & {sub.Mask}) >> {sub.Shift});");
                            }
                            else if (!string.IsNullOrEmpty(sub.Mask)) {
                                writer.WriteLine($"{sub.Name} = ({SimplifyType(sub.Type)})({member.Name} & {sub.Mask});");
                            }
                            else if (!string.IsNullOrEmpty(sub.Shift)) {
                                writer.WriteLine($"{sub.Name} = ({SimplifyType(sub.Type)})({member.Name} >> {sub.Shift});");
                            }
                            else {
                                writer.WriteLine($"{sub.Name} = ({SimplifyType(sub.Type)})({member.Name});");
                            }
                        }
                    }
                }
                else {
                    if (!string.IsNullOrEmpty(member.KnownType)) {
                        writer.WriteLine(member.Name + " = " + GetBinaryReaderForType(member.MemberType, member.KnownType) + ";");
                    }
                    else {
                        writer.WriteLine(member.Name + " = " + GetBinaryReaderForType(member.MemberType, member.Size) + ";");
                    }
                }
            }
        }

        public void WriteVectorPusher(SourceWriter writer, ACVector vector, ACBaseModel child) {
            var name = child is ACVector ? (child as ACVector).Name : (child as ACDataMember).Name;
            writer.WriteLine($"{vector.Name}.Add({name});");
        }

        public void WriteVectorPusher(SourceWriter writer, ACVector vector, string loopChar) {
             if (vector.Children.Count == 1) {
                WriteVectorPusher(writer, vector, vector.Children.First());
                return;
            }
            // Logic for complex vector pushing (dictionaries and lists)
             if (string.IsNullOrEmpty(vector.GenericKey) && string.IsNullOrEmpty(vector.GenericValue)) {
                writer.WriteLine($"{vector.Name}[{loopChar}] = {GetBinaryReaderForType(vector.Type)};");
            }
            else if (string.IsNullOrEmpty(vector.GenericKey)) {
                var type = XMLDefParser.ACDataTypes.Values.FirstOrDefault(t => t.Name == vector.GenericValue);

                if (type != null && type.IsAbstract) {
                     var field = type.Children.FirstOrDefault(c => c is ACDataMember m && m.Name == type.TypeSwitch) as ACDataMember;

                    if (XMLDefParser.ACEnums.TryGetValue(field.MemberType, out var en)) {
                        var reader = GetBinaryReaderForType(XMLDefParser.ACEnums[field.MemberType].ParentType);
                        writer.WriteLine($"var _peekedValue = (" + field.MemberType + ")" + reader + ";");
                    }
                     else {
                         writer.WriteLine($"var _peekedValue = {GetBinaryReaderForType(field.MemberType)};");
                    }
                    writer.WriteLine($"reader.Skip(-sizeof({field.MemberType}));");
                    writer.WriteLine($"{vector.Name}.Add({vector.GenericValue}.Unpack(reader, _peekedValue));");
                }
                else {
                     XMLDefParser.ACDataTypes.TryGetValue(vector.GenericValue, out var vType);
                     if (vType is not null && vType.AllChildren.Any(c => c is ACVector m && m.Length.Contains("parent."))) {
                         var child = vType.AllChildren.First(c => c is ACVector m && m.Length.Contains("parent.")) as ACVector;
                         writer.WriteLine($"var _val = {GetBinaryReaderForType(vector.GenericValue).TrimEnd(')')}{child.Length.Substring(7)});");
                         writer.WriteLine($"{vector.Name}.Add(_val);");
                    }
                     else {
                        if (XMLDefParser.ACEnums.TryGetValue(vector.GenericValue, out var en)) {
                            writer.WriteLine($"{vector.Name}.Add(({vector.GenericValue}){GetBinaryReaderForType(en.ParentType)});");
                        }
                        else {
                            writer.WriteLine($"{vector.Name}.Add({GetBinaryReaderForType(vector.GenericValue)});");
                        }
                    }
                }
            }
             else {
                 if (XMLDefParser.ACEnums.ContainsKey(vector.GenericKey)) {
                    var reader = GetBinaryReaderForType(XMLDefParser.ACEnums[vector.GenericKey].ParentType);
                    writer.WriteLine("var _key = (" + vector.GenericKey + ")" + reader + ";");
                }
                else {
                    writer.WriteLine($"var _key = {GetBinaryReaderForType(vector.GenericKey)};");
                }

                 var type = XMLDefParser.ACDataTypes.Values.FirstOrDefault(t => t.Name == vector.GenericValue);
                 if (type != null && type.IsAbstract) {
                      var field = type.Children.FirstOrDefault(c => c is ACDataMember m && m.Name == type.TypeSwitch) as ACDataMember;
                      if (!string.IsNullOrEmpty(field.Size)) {
                        writer.WriteLine($"reader.ReadBytes({field.Size});");
                    }
                     if (XMLDefParser.ACEnums.TryGetValue(field.MemberType, out var en)) {
                        var reader = GetBinaryReaderForType(XMLDefParser.ACEnums[field.MemberType].ParentType);
                        writer.WriteLine($"var _peekedValue = (" + field.MemberType + ")" + reader + ";");
                    }
                    else {
                        writer.WriteLine($"var _peekedValue = {GetBinaryReaderForType(field.MemberType)};");
                    }
                     if (!string.IsNullOrEmpty(field.Size)) {
                        writer.WriteLine($"reader.Skip(-sizeof({field.MemberType}) + {field.Size});");
                    }
                    else {
                        writer.WriteLine($"reader.Skip(-sizeof({field.MemberType}));");
                    }
                    writer.WriteLine($"var _val = {vector.GenericValue}.Unpack(reader, _peekedValue);");
                    writer.WriteLine($"{vector.Name}.Add(_key, _val);");
                 }
                 else {
                     XMLDefParser.ACDataTypes.TryGetValue(vector.GenericValue, out var vType);
                     if (vType is not null && vType.AllChildren.Any(c => c is ACVector m && m.Length.Contains("parent."))) {
                          var child = vType.AllChildren.First(c => c is ACVector m && m.Length.Contains("parent.")) as ACVector;
                        writer.WriteLine($"var _val = {GetBinaryReaderForType(vector.GenericValue).TrimEnd(')')}{child.Length.Substring(7)});");
                     }
                     else {
                          if (XMLDefParser.ACEnums.ContainsKey(vector.GenericValue)) {
                            var reader = GetBinaryReaderForType(XMLDefParser.ACEnums[vector.GenericValue].ParentType);
                            writer.WriteLine("var _val = (" + vector.GenericValue + ")" + reader + ";");
                        }
                        else {
                            writer.WriteLine($"var _val = {GetBinaryReaderForType(vector.GenericValue)};");
                        }
                     }
                    writer.WriteLine($"{vector.Name}.Add(_key, _val);");
                 }
             }
        }

        public void WriteEnumWriter(SourceWriter writer, ACDataMember member, string pre = "") {
            if (member.Name.StartsWith("_") && member.Parent is ACDataType dType && dType.TypeSwitch == member.Name) {
                writer.WriteLine($"writer.{GetBinaryWriterForType(XMLDefParser.ACEnums[member.MemberType].ParentType)}(({XMLDefParser.ACEnums[member.MemberType].ParentType}){member.Name.Substring(1,1).ToUpper() + member.Name.Substring(2)});");
                return;
            }
            writer.WriteLine($"writer.{GetBinaryWriterForType(XMLDefParser.ACEnums[member.MemberType].ParentType)}(({XMLDefParser.ACEnums[member.MemberType].ParentType}){member.Name});");
        }

        public void WriteTemplatedWriter(SourceWriter writer, ACDataMember member) {
            writer.WriteLine($"writer.Write{member.MemberType}({member.Name});");
        }

        public void GenerateMemberWriter(SourceWriter writer, ACDataMember member, ACDataMember parent, string pre = "") {
            if (member.MemberType == "Vector3") {
                writer.WriteLine($"writer.WriteVector3({pre}{member.Name});");
                return;
            }
            if (member.MemberType == "Quaternion") {
                writer.WriteLine($"writer.WriteQuaternion({pre}{member.Name});");
                return;
            }
            if (XMLDefParser.ACTemplatedTypes.ContainsKey(member.MemberType)) {
                WriteTemplatedWriter(writer, member);
                return;
            }

            if (member.IsLength) {
                if (!string.IsNullOrEmpty(member.LengthFor?.LengthMod)) {
                    writer.WriteLine($"writer.{GetBinaryWriterForType(member.MemberType)}(({SimplifyType(member.MemberType)}){member.LengthFor.Name}.Count() - {member.LengthFor.LengthMod});");
                }
                else {
                    writer.WriteLine($"writer.{GetBinaryWriterForType(member.MemberType)}(({SimplifyType(member.MemberType)}){(string.IsNullOrEmpty(member.LengthOf) ? member.LengthFor.Name : member.LengthOf)}.Count());");
                }
                return;
            }

            if (member.SubMembers.Count > 0) {
                writer.WriteLine($"{SimplifyType(member.MemberType)} {member.Name} = default;");
                foreach (var sub in member.SubMembers) {
                    if (!string.IsNullOrEmpty(sub.Mask) && !string.IsNullOrEmpty(sub.Shift)) {
                        writer.WriteLine($"{member.Name} |= ({member.MemberType})((({member.MemberType}){sub.Name} << {sub.Shift}) & {sub.Mask});");
                    }
                    else if (!string.IsNullOrEmpty(sub.Mask)) {
                        writer.WriteLine($"{member.Name} |= ({member.MemberType})(({member.MemberType}){sub.Name} & {sub.Mask});");
                    }
                    else if (!string.IsNullOrEmpty(sub.Shift)) {
                        writer.WriteLine($"{member.Name} |= ({member.MemberType})(({member.MemberType}){sub.Name} << {sub.Shift});");
                    }
                    else {
                        writer.WriteLine($"{member.Name} |= ({member.MemberType}){member.Value};");
                    }
                }
                writer.WriteLine($"writer.{GetBinaryWriterForType(member.MemberType)}({member.Name});");
                return;
            }

            if (!string.IsNullOrEmpty(member.Value)) {
                writer.WriteLine($"writer.{GetBinaryWriterForType(member.MemberType)}({member.Value}{(string.IsNullOrEmpty(member.Size) ? "" : $", {member.Size}")});");
                return;
            }

            if (!string.IsNullOrEmpty(member.KnownType)) {
                writer.WriteLine($"writer.{GetBinaryWriterForType(member.MemberType)}({member.Name}, {member.KnownType});");
                return;
            }

            if (!string.IsNullOrEmpty(parent?.GenericType) && parent.MemberType == "List") {
                if (XMLDefParser.ACEnums.ContainsKey(member.MemberType)) {
                    WriteEnumWriter(writer, member, pre);
                }
                else if (XMLDefParser.ACDataTypes.ContainsKey(member.MemberType)) {
                    writer.WriteLine($"{pre.TrimEnd('.')}.Write(writer);");
                }
                else {
                    writer.WriteLine($"writer.Write({pre.TrimEnd('.')});");
                }
            }
            else {
                if (XMLDefParser.ACEnums.ContainsKey(member.MemberType)) {
                    WriteEnumWriter(writer, member, pre);
                }
                else if (XMLDefParser.ACDataTypes.ContainsKey(member.MemberType)) {
                    if (string.IsNullOrEmpty(pre)) {
                        writer.WriteLine($"writer.WriteItem<{member.MemberType}>({member.Name});");
                    }
                    else if (member.Parent?.GetType() == typeof(ACVector) && member.Parent?.Children?.Count == 1) {
                         writer.WriteLine($"{pre}Write(writer);");
                    }
                    else if (member.MemberType == member.Name) {
                        writer.WriteLine($"{pre}{member.Name}.Write(writer);");
                    }
                    else {
                        writer.WriteLine($"{pre}Write(writer);");
                    }
                }
                else if (string.IsNullOrEmpty(pre)) {
                    writer.WriteLine($"writer.{GetBinaryWriterForType(member.MemberType)}({member.Name}{(string.IsNullOrEmpty(member.Size) ? "" : $", {member.Size}")});");
                }
                else if (member.Parent?.GetType() == typeof(ACVector) && member.Parent?.Children?.Count > 1) {
                     writer.WriteLine($"writer.Write({pre}{member.Name});");
                }
                else {
                    writer.WriteLine($"writer.Write({pre.TrimEnd('.')});");
                }
            }
        }

        public void GenerateWriterContents(SourceWriter writer, ACBaseModel model, int depth, ACBaseModel parent = null, string pre = "") {
             if (model is ACDataMember) {
                GenerateMemberWriter(writer, model as ACDataMember, parent as ACDataMember, pre);
            }
            else if (model is ACIf) {
                var acif = model as ACIf;
                 writer.WriteLine($"if ({acif.WriteCondition ?? acif.Condition}) {{");
                 using(writer.IndentScope()) {
                     foreach (var member in acif.Children) {
                        GenerateWriterContents(writer, member, depth);
                    }
                }
                writer.WriteLine("}");
            }
             else if (model is ACMaskMap) {
                var maskMap = model as ACMaskMap;
                foreach (var mask in maskMap.Masks) {
                    WriteMaskMapCheckStart(writer, maskMap, mask);
                    using(writer.IndentScope()) {
                        foreach (var maskChild in mask.Children) {
                            GenerateWriterContents(writer, maskChild, depth);
                        }
                    }
                     writer.WriteLine("}"); // End if

                     if (mask.ElseChildren.Count > 0) {
                        writer.WriteLine("else {");
                        using(writer.IndentScope()) {
                            foreach (var elseChild in mask.ElseChildren) {
                                GenerateWriterContents(writer, elseChild, depth);
                            }
                        }
                        writer.WriteLine("}");
                    }
                }
            }
            else if (model is ACAlign) {
                 writer.WriteLine($"writer.Align(4);");
            }
            else if (model is ACSwitch) {
                var acswitch = model as ACSwitch;
                 // switches need to define locally used variables first
                var usedNames = new List<string>();
                if (model.Parent.GetType() != typeof(ACDataType) && model.Parent.GetType() != typeof(ACDBObj)) {
                    foreach (var scase in acswitch.Cases) {
                        if (acswitch.Parent is ACSwitchCase)
                            break;
                        foreach (ACBaseModel child in scase.AllChildren.Where(c => c is ACDataMember || c is ACVector)) {
                            if (child is ACDataMember) {
                                var dm = child as ACDataMember;
                                if (usedNames.Contains(dm.Name))
                                    continue;
                                WriteLocalChildDefinition(writer, dm);
                                usedNames.Add(dm.Name);
                            }
                            else if (child is ACVector) {
                                WriteLocalVectorDefinition(writer, child as ACVector);
                            }
                        }
                    }
                }
                writer.WriteLine($"switch({acswitch.Name}) {{");
                using (writer.IndentScope()) {
                    foreach (var scase in acswitch.Cases) {
                         var cases = scase.Value.Split(new string[] { " | " }, StringSplitOptions.None);
                        foreach (var s in cases) {
                            writer.WriteLine("case " + s + ":");
                        }
                        using(writer.IndentScope()) {
                             foreach (var child in scase.Children) {
                                GenerateWriterContents(writer, child, depth);
                            }
                             writer.WriteLine("break;");
                        }
                    }
                }
                writer.WriteLine("}");
            }
            else if (model is ACVector) {
                var vector = model as ACVector;

                if (string.IsNullOrEmpty(vector.GenericKey) && string.IsNullOrEmpty(vector.GenericValue)) {
                     var indexVar = new string[] { "i", "x", "y", "z", "q", "p", "t", "r", "f", "g", "h", "k", "u", "v" }[depth];
                    if (!string.IsNullOrEmpty(vector.Skip)) {
                        writer.WriteLine($"for (var {indexVar}=0; {indexVar} < {pre}{vector.Length} - {vector.Skip}; {indexVar}++) {{");
                    }
                    else {
                        writer.WriteLine($"for (var {indexVar}=0; {indexVar} < {vector.Name}.Count(); {indexVar}++) {{");
                    }
                    
                    using (writer.IndentScope()) {
                         writer.WriteLine($"writer.{GetBinaryWriterForType(vector.Type)}({vector.Name}[{indexVar}]);");
                    }
                    writer.WriteLine("}");
                }
                else if (string.IsNullOrEmpty(vector.GenericKey)) {
                    writer.WriteLine($"foreach (var item in {vector.Name}) {{");
                    using(writer.IndentScope()) {
                        if (XMLDefParser.ACEnums.ContainsKey(vector.GenericValue)) {
                            writer.WriteLine($"writer.{GetBinaryWriterForType(XMLDefParser.ACEnums[vector.GenericValue].ParentType)}(({XMLDefParser.ACEnums[vector.GenericValue].ParentType})item);");
                        }
                        else {
                            writer.WriteLine($"writer.{GetBinaryWriterForType(vector.GenericValue)}(item);");
                        }
                    }
                    writer.WriteLine("}");
                }
                 else {
                    writer.WriteLine($"foreach (var kv in {vector.Name}) {{");
                    using(writer.IndentScope()) {
                        if (XMLDefParser.ACEnums.ContainsKey(vector.GenericKey)) {
                            writer.WriteLine($"writer.{GetBinaryWriterForType(XMLDefParser.ACEnums[vector.GenericKey].ParentType)}(({XMLDefParser.ACEnums[vector.GenericKey].ParentType})kv.Key);");
                        }
                        else {
                            writer.WriteLine($"writer.{GetBinaryWriterForType(vector.GenericKey)}(kv.Key);");
                        }
                        if (XMLDefParser.ACEnums.ContainsKey(vector.GenericValue)) {
                            writer.WriteLine($"writer.{GetBinaryWriterForType(XMLDefParser.ACEnums[vector.GenericValue].ParentType)}(({XMLDefParser.ACEnums[vector.GenericValue].ParentType})kv.Value);");
                        }
                        else {
                            writer.WriteLine($"writer.{GetBinaryWriterForType(vector.GenericValue)}(kv.Value);");
                        }
                    }
                    writer.WriteLine("}");
                }
            }
        }

        public void WriteAbstractTypeGetter(SourceWriter writer, ACDataType type) {
            var dType = (type.Parent as DatReaderWriter.SourceGenerator.Models.ACDataType);
            if (dType is null || string.IsNullOrEmpty(dType.TypeSwitch)) return;
            var name = dType.TypeSwitch.Substring(1, 1).ToUpper() + dType.TypeSwitch.Substring(2);
            var switched = dType.AllChildren.FirstOrDefault(s => s is ACDataMember m && m.Name == dType.TypeSwitch) as ACDataMember;
            writer.WriteLine("/// <inheritdoc />");
            writer.WriteLine($"public override {switched?.MemberType} {name} => {type?.Value};");
            writer.WriteLine("");
        }

        public void WriteParentContructor(SourceWriter writer, ACDataType type) {
            var child = type.AllVectors.FirstOrDefault(c => c.Length.StartsWith("parent."));
            if (child is null) {
                return;
            }

            string parentTypeType = "";
            string parentTypeName = "";
             var parentType = XMLDefParser.ACDataTypes.Values
                .FirstOrDefault(t => t.AllVectors.Count(v => v.GenericValue == type.Name) > 0);

            if (parentType is not null) {
                parentTypeType = SimplifyType(parentType?.Name);
                parentTypeName = SimplifyType(parentType?.Name);
                writer.WriteLine($"private {SimplifyType(parentType?.Name)} {SimplifyType(parentType?.Name).ToLower()};"); // Fixed field definition
            }
            else {
                var parentType2 = XMLDefParser.ACDBObjs.Values
                    .FirstOrDefault(t => t.Children.Any(c => c is ACVector v && v.GenericValue == type.Name))
                    .Children
                    .FirstOrDefault(c => c is ACDataMember m && m.Name == child.Length.Substring(7))
                    as ACDataMember;
                parentTypeType = SimplifyType(parentType2?.MemberType);
                parentTypeName = SimplifyType(parentType2?.Name);
                writer.WriteLine($"private {parentType2.MemberType} {parentType2.Name};");
            }
            writer.WriteLine("");

            writer.WriteLine($"public {type.Name}({parentTypeType} {parentTypeName}) {{");
             using (writer.IndentScope()) {
                 if (parentType is not null) {
                     writer.WriteLine($"this.{SimplifyType(parentType?.Name).ToLower()} = {parentTypeName};");
                }
                else {
                    writer.WriteLine($"this.{parentTypeName} = {parentTypeName};");
                }
            }
            writer.WriteLine("}");
            writer.WriteLine("");
        }
    }
}
