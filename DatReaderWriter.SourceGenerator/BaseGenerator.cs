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
                writer.WriteLine("public abstract " + TypeGeneratorHelper.GetTypeDeclaration(member, XMLDefParser) +
                                 " " +
                                 member.Name.Substring(1, 1).ToUpper() + member.Name.Substring(2) + " { get; }");
                writer.WriteLine("");
                return;
            }

            if (member.Name.StartsWith("_")) return;

            WriteSummary(writer, member.Text);
            if (XMLDefParser.ACTemplatedTypes.ContainsKey(member.MemberType)) {
                writer.WriteLine("public " + TypeGeneratorHelper.GetTypeDeclaration(member, XMLDefParser) + " " +
                                 member.Name + " = new();");
            }
            else {
                writer.WriteLine("public " + TypeGeneratorHelper.GetTypeDeclaration(member, XMLDefParser) + " " +
                                 member.Name + ";");
            }

            writer.WriteLine("");
        }

        public void WriteClassProperty(SourceWriter writer, ACSubMember member) {
            if (member.Name.StartsWith("_")) return;

            var parent = member.Parent as ACDataMember;
            var simplifiedType = TypeGeneratorHelper.SimplifyType(member.Type);
            WriteSummary(writer, member.Text);
            writer.WriteLine("public " + simplifiedType + " " + member.Name + ";");
            writer.WriteLine("");
        }

        public void WriteClassVectorProperty(SourceWriter writer, ACVector vector) {
            bool areGenericArgs = vector.Children.Any(c => c is ACDataMember m && string.IsNullOrEmpty(m.Name)) ||
                                  vector.Children.Any(c => c is ACVector v && string.IsNullOrEmpty(v.Name));

            if (vector.Children.Count > 1 && !areGenericArgs)
                WriteVectorItemClassDefinition(writer, vector);

            WriteSummary(writer, vector.Text);
            if (vector.IsGenericNonContainer) {
                writer.WriteLine(
                    $"public {TypeGeneratorHelper.GetTypeDeclaration(vector, XMLDefParser)} {vector.Name} = new();");
            }
            else {
                writer.WriteLine(
                    $"public {TypeGeneratorHelper.GetTypeDeclaration(vector, XMLDefParser)} {vector.Name} = [];");
            }

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
            switch (model) {
                case ACDataMember member:
                    GenerateMemberReader(writer, member);
                    break;
                case ACIf acIf:
                    GenerateReaderIf(writer, acIf, depth);
                    break;
                case ACMaskMap maskMap:
                    GenerateReaderMaskMap(writer, maskMap, depth);
                    break;
                case ACAlign:
                    writer.WriteLine($"reader.Align(4);");
                    break;
                case ACSwitch acSwitch:
                    GenerateReaderSwitch(writer, acSwitch, depth);
                    break;
                case ACVector vector:
                    GenerateReaderVector(writer, vector, depth);
                    break;
            }
        }

        private void GenerateReaderIf(SourceWriter writer, ACIf acif, int depth) {
            writer.WriteLine($"if ({acif.Condition}) {{");
            using (writer.IndentScope()) {
                foreach (var member in acif.Children) {
                    GenerateReaderContents(writer, member, depth);
                }
            }

            writer.WriteLine("}");
        }

        private void GenerateReaderMaskMap(SourceWriter writer, ACMaskMap maskMap, int depth) {
            foreach (var mask in maskMap.Masks) {
                WriteMaskMapCheckStart(writer, maskMap, mask);
                using (writer.IndentScope()) {
                    foreach (var maskChild in mask.Children) {
                        GenerateReaderContents(writer, maskChild, depth);
                    }
                }

                writer.WriteLine("}"); // End if

                if (mask.ElseChildren.Count > 0) {
                    writer.WriteLine("else {");
                    using (writer.IndentScope()) {
                        foreach (var elseChild in mask.ElseChildren) {
                            GenerateReaderContents(writer, elseChild, depth);
                        }
                    }

                    writer.WriteLine("}");
                }
            }
        }

        private void GenerateReaderSwitch(SourceWriter writer, ACSwitch acswitch, int depth) {
            // switches need to define locally used variables first
            var usedNames = new List<string>();
            if (acswitch.Parent.GetType() != typeof(ACDataType) && acswitch.Parent.GetType() != typeof(ACDBObj)) {
                foreach (var scase in acswitch.Cases) {
                    if (acswitch.Parent is ACSwitchCase)
                        break;
                    foreach (ACBaseModel child in
                             scase.AllChildren.Where(c => c is ACDataMember || c is ACVector)) {
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

                    using (writer.IndentScope()) {
                        foreach (var child in scase.Children) {
                            GenerateReaderContents(writer, child, depth);
                        }

                        writer.WriteLine("break;");
                    }
                }
            }

            writer.WriteLine("}");
        }

        private void GenerateReaderVector(SourceWriter writer, ACVector vector, int depth) {
            if (vector.Parent is ACVector) {
                WriteLocalVectorDefinition(writer, vector);
            }

            bool areGenericArgs = vector.Children.Any(c => c is ACDataMember m && string.IsNullOrEmpty(m.Name)) ||
                                  vector.Children.Any(c => c is ACVector v && string.IsNullOrEmpty(v.Name));

            if (vector.IsGenericNonContainer || areGenericArgs && string.IsNullOrEmpty(vector.Length) && string.IsNullOrEmpty(vector.LengthMod) && string.IsNullOrEmpty(vector.Skip)) {
                 writer.WriteLine($"{vector.Name} = reader.ReadItem<{TypeGeneratorHelper.GetTypeDeclaration(vector, XMLDefParser)}>();");
                 return;
            }

            if (string.IsNullOrEmpty(vector.GenericKey) && string.IsNullOrEmpty(vector.GenericValue) && vector.Children.Count == 0) {
                if (!string.IsNullOrEmpty(vector.LengthMod)) {
                    writer.WriteLine(
                        $"{vector.Name} = new {TypeGeneratorHelper.SimplifyType(vector.Type)}[{vector.Length} + {vector.LengthMod}];");
                }
                else {
                    writer.WriteLine(
                        $"{vector.Name} = new {TypeGeneratorHelper.SimplifyType(vector.Type)}[{vector.Length}];");
                }
            }

            var indexVar =
                new string[] { "i", "x", "y", "z", "q", "p", "t", "r", "f", "g", "h", "k", "u", "v" }[depth];
            if (!string.IsNullOrEmpty(vector.Skip)) {
                writer.WriteLine(
                    $"for (var {indexVar}=0; {indexVar} < {vector.Length} - {vector.Skip}; {indexVar}++) {{");
            }
            else if (!string.IsNullOrEmpty(vector.LengthMod)) {
                writer.WriteLine(
                    $"for (var {indexVar}=0; {indexVar} < {vector.Length} + {vector.LengthMod}; {indexVar}++) {{");
            }
            else {
                writer.WriteLine(
                    $"for (var {indexVar}=0; {indexVar} < {vector.Length.Replace("parent.", "")}; {indexVar}++) {{");
            }

            using (writer.IndentScope()) {
                if (areGenericArgs) {
                    if (vector.Children.Count == 1) {
                        var valModel = vector.Children[0];
                        string valRead = "";
                        if (valModel is ACDataMember valDm) {
                            if (XMLDefParser.ACEnums.ContainsKey(valDm.MemberType)) {
                                 valRead = $"({valDm.MemberType}){TypeGeneratorHelper.GetBinaryReaderForType(XMLDefParser.ACEnums[valDm.MemberType].ParentType)}";
                            }
                            else {
                                valRead = TypeGeneratorHelper.GetBinaryReaderForType(valDm.MemberType, valDm.Size);
                            }
                        }
                        else if (valModel is ACVector v) {
                            valRead = $"reader.ReadItem<{TypeGeneratorHelper.GetTypeDeclaration(v, XMLDefParser)}>()";
                        }
                        writer.WriteLine($"{vector.Name}.Add({valRead});");
                    }
                    else if (vector.Children.Count == 2) {
                        var keyModel = vector.Children[0];
                        var valModel = vector.Children[1];
                        
                        string keyRead = "";
                        if (keyModel is ACDataMember keyDm) {
                            if (XMLDefParser.ACEnums.ContainsKey(keyDm.MemberType)) {
                                keyRead = $"({keyDm.MemberType}){TypeGeneratorHelper.GetBinaryReaderForType(XMLDefParser.ACEnums[keyDm.MemberType].ParentType)}";
                            }
                            else {
                                keyRead = TypeGeneratorHelper.GetBinaryReaderForType(keyDm.MemberType, keyDm.Size);
                            }
                        }
                        else if (keyModel is ACVector v) {
                            keyRead = $"reader.ReadItem<{TypeGeneratorHelper.GetTypeDeclaration(v, XMLDefParser)}>()";
                        }

                        string valRead = "";
                        if (valModel is ACDataMember valDm) {
                            if (XMLDefParser.ACEnums.ContainsKey(valDm.MemberType)) {
                                valRead = $"({valDm.MemberType}){TypeGeneratorHelper.GetBinaryReaderForType(XMLDefParser.ACEnums[valDm.MemberType].ParentType)}";
                            }
                            else {
                                valRead = TypeGeneratorHelper.GetBinaryReaderForType(valDm.MemberType, valDm.Size);
                            }
                        }
                        else if (valModel is ACVector v) {
                            valRead = $"reader.ReadItem<{TypeGeneratorHelper.GetTypeDeclaration(v, XMLDefParser)}>()";
                        }
                        
                        writer.WriteLine($"var _key = {keyRead};");
                        writer.WriteLine($"var _val = {valRead};");
                        writer.WriteLine($"{vector.Name}.Add(_key, _val);");
                    }
                }
                else {
                    foreach (var vmember in vector.Children) {
                        GenerateReaderContents(writer, vmember, depth + 1);
                    }
                    WriteVectorPusher(writer, vector, indexVar);
                }
            }

            writer.WriteLine("}");
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
            var type = TypeGeneratorHelper.SimplifyType(child.MemberType);
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
                default:
                    writer.WriteLine($"{type} {child.Name} = new();");
                    break;
            }
        }

        public void WriteLocalVectorDefinition(SourceWriter writer, ACVector vector) {
            writer.WriteLine($"var {vector.Name} = new {TypeGeneratorHelper.GetTypeDeclaration(vector, XMLDefParser)}();");
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
                    var reader =
                        TypeGeneratorHelper.GetBinaryReaderForType(XMLDefParser.ACEnums[field.MemberType].ParentType,
                            null);
                    writer.WriteLine($"var _peekedValue = (" + field.MemberType + ")" + reader + ";");
                }
                else {
                    writer.WriteLine(
                        $"var _peekedValue = {TypeGeneratorHelper.GetBinaryReaderForType(field.MemberType, null)};");
                }

                writer.WriteLine($"reader.Skip(-sizeof({field.MemberType}));");
                writer.WriteLine($"{member.Name} = {member.MemberType}.Unpack(reader, _peekedValue);");
                return;
            }

            if (XMLDefParser.ACEnums.ContainsKey(member.MemberType)) {
                var reader =
                    TypeGeneratorHelper.GetBinaryReaderForType(XMLDefParser.ACEnums[member.MemberType].ParentType);
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

                writer.WriteLine(
                    $"{member.Name} = reader.ReadItem<{TypeGeneratorHelper.GetTypeDeclaration(member, XMLDefParser)}>();");
            }
            else if (XMLDefParser.ACTemplatedTypes.ContainsKey(member.MemberType)) {
                if (!string.IsNullOrEmpty(member.GenericType)) {
                    writer.WriteLine(
                        $"{member.Name} = reader.Read{member.MemberType}<{TypeGeneratorHelper.SimplifyType(member.GenericType)}>();");
                }
                else {
                    writer.WriteLine(
                        $"{member.Name} = reader.Read{member.MemberType}<{TypeGeneratorHelper.SimplifyType(member.GenericKey)}, {TypeGeneratorHelper.SimplifyType(member.GenericValue)}>();");
                }
            }
            else {
                if (member.Parent is ACVector || member.IsLength || member.Name.StartsWith("_")) {
                    writer.WriteLine("var " + member.Name + " = " +
                                     TypeGeneratorHelper.GetBinaryReaderForType(member.MemberType, member.Size) + ";");
                    if (member.SubMembers.Count > 0) {
                        foreach (var sub in member.SubMembers) {
                            if (!string.IsNullOrEmpty(sub.Mask) && !string.IsNullOrEmpty(sub.Shift)) {
                                writer.WriteLine(
                                    $"{sub.Name} = ({TypeGeneratorHelper.SimplifyType(sub.Type)})(({member.Name} & {sub.Mask}) >> {sub.Shift});");
                            }
                            else if (!string.IsNullOrEmpty(sub.Mask)) {
                                writer.WriteLine(
                                    $"{sub.Name} = ({TypeGeneratorHelper.SimplifyType(sub.Type)})({member.Name} & {sub.Mask});");
                            }
                            else if (!string.IsNullOrEmpty(sub.Shift)) {
                                writer.WriteLine(
                                    $"{sub.Name} = ({TypeGeneratorHelper.SimplifyType(sub.Type)})({member.Name} >> {sub.Shift});");
                            }
                            else {
                                writer.WriteLine(
                                    $"{sub.Name} = ({TypeGeneratorHelper.SimplifyType(sub.Type)})({member.Name});");
                            }
                        }
                    }
                }
                else {
                    if (!string.IsNullOrEmpty(member.KnownType)) {
                        writer.WriteLine(member.Name + " = " +
                                         TypeGeneratorHelper.GetBinaryReaderForType(member.MemberType,
                                             member.KnownType) + ";");
                    }
                    else {
                        writer.WriteLine(member.Name + " = " +
                                         TypeGeneratorHelper.GetBinaryReaderForType(member.MemberType, member.Size) +
                                         ";");
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
                writer.WriteLine(
                    $"{vector.Name}[{loopChar}] = {TypeGeneratorHelper.GetBinaryReaderForType(vector.Type)};");
            }
            else if (string.IsNullOrEmpty(vector.GenericKey)) {
                var type = XMLDefParser.ACDataTypes.Values.FirstOrDefault(t => t.Name == vector.GenericValue);

                if (type != null && type.IsAbstract) {
                    var field =
                        type.Children.FirstOrDefault(c => c is ACDataMember m && m.Name == type.TypeSwitch) as
                            ACDataMember;

                    if (XMLDefParser.ACEnums.TryGetValue(field.MemberType, out var en)) {
                        var reader =
                            TypeGeneratorHelper.GetBinaryReaderForType(
                                XMLDefParser.ACEnums[field.MemberType].ParentType);
                        writer.WriteLine($"var _peekedValue = (" + field.MemberType + ")" + reader + ";");
                    }
                    else {
                        writer.WriteLine(
                            $"var _peekedValue = {TypeGeneratorHelper.GetBinaryReaderForType(field.MemberType)};");
                    }

                    writer.WriteLine($"reader.Skip(-sizeof({field.MemberType}));");
                    writer.WriteLine($"{vector.Name}.Add({vector.GenericValue}.Unpack(reader, _peekedValue));");
                }
                else {
                    XMLDefParser.ACDataTypes.TryGetValue(vector.GenericValue, out var vType);
                    if (vType is not null &&
                        vType.AllChildren.Any(c => c is ACVector m && m.Length.Contains("parent."))) {
                        var child =
                            vType.AllChildren.First(c => c is ACVector m && m.Length.Contains("parent.")) as ACVector;
                        writer.WriteLine(
                            $"var _val = {TypeGeneratorHelper.GetBinaryReaderForType(vector.GenericValue).TrimEnd(')')}{child.Length.Substring(7)});");
                        writer.WriteLine($"{vector.Name}.Add(_val);");
                    }
                    else {
                        if (XMLDefParser.ACEnums.TryGetValue(vector.GenericValue, out var en)) {
                            writer.WriteLine(
                                $"{vector.Name}.Add(({vector.GenericValue}){TypeGeneratorHelper.GetBinaryReaderForType(en.ParentType)});");
                        }
                        else {
                            writer.WriteLine(
                                $"{vector.Name}.Add({TypeGeneratorHelper.GetBinaryReaderForType(vector.GenericValue)});");
                        }
                    }
                }
            }
            else {
                if (XMLDefParser.ACEnums.ContainsKey(vector.GenericKey)) {
                    var reader =
                        TypeGeneratorHelper.GetBinaryReaderForType(XMLDefParser.ACEnums[vector.GenericKey].ParentType);
                    writer.WriteLine("var _key = (" + vector.GenericKey + ")" + reader + ";");
                }
                else {
                    writer.WriteLine($"var _key = {TypeGeneratorHelper.GetBinaryReaderForType(vector.GenericKey)};");
                }

                var type = XMLDefParser.ACDataTypes.Values.FirstOrDefault(t => t.Name == vector.GenericValue);
                if (type != null && type.IsAbstract) {
                    var field =
                        type.Children.FirstOrDefault(c => c is ACDataMember m && m.Name == type.TypeSwitch) as
                            ACDataMember;
                    if (!string.IsNullOrEmpty(field.Size)) {
                        writer.WriteLine($"reader.ReadBytes({field.Size});");
                    }

                    if (XMLDefParser.ACEnums.TryGetValue(field.MemberType, out var en)) {
                        var reader =
                            TypeGeneratorHelper.GetBinaryReaderForType(
                                XMLDefParser.ACEnums[field.MemberType].ParentType);
                        writer.WriteLine($"var _peekedValue = (" + field.MemberType + ")" + reader + ";");
                    }
                    else {
                        writer.WriteLine(
                            $"var _peekedValue = {TypeGeneratorHelper.GetBinaryReaderForType(field.MemberType)};");
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
                    if (vType is not null &&
                        vType.AllChildren.Any(c => c is ACVector m && m.Length.Contains("parent."))) {
                        var child =
                            vType.AllChildren.First(c => c is ACVector m && m.Length.Contains("parent.")) as ACVector;
                        writer.WriteLine(
                            $"var _val = {TypeGeneratorHelper.GetBinaryReaderForType(vector.GenericValue).TrimEnd(')')}{child.Length.Substring(7)});");
                    }
                    else {
                        if (XMLDefParser.ACEnums.ContainsKey(vector.GenericValue)) {
                            var reader =
                                TypeGeneratorHelper.GetBinaryReaderForType(XMLDefParser.ACEnums[vector.GenericValue]
                                    .ParentType);
                            writer.WriteLine("var _val = (" + vector.GenericValue + ")" + reader + ";");
                        }
                        else {
                            writer.WriteLine(
                                $"var _val = {TypeGeneratorHelper.GetBinaryReaderForType(vector.GenericValue)};");
                        }
                    }

                    writer.WriteLine($"{vector.Name}.Add(_key, _val);");
                }
            }
        }

        public void WriteEnumWriter(SourceWriter writer, ACDataMember member, string pre = "") {
            if (member.Name.StartsWith("_") && member.Parent is ACDataType dType && dType.TypeSwitch == member.Name) {
                writer.WriteLine(
                    $"writer.{TypeGeneratorHelper.GetBinaryWriterForType(XMLDefParser.ACEnums[member.MemberType].ParentType)}(({XMLDefParser.ACEnums[member.MemberType].ParentType}){member.Name.Substring(1, 1).ToUpper() + member.Name.Substring(2)});");
                return;
            }

            writer.WriteLine(
                $"writer.{TypeGeneratorHelper.GetBinaryWriterForType(XMLDefParser.ACEnums[member.MemberType].ParentType)}(({XMLDefParser.ACEnums[member.MemberType].ParentType}){member.Name});");
        }

        public void WriteTemplatedWriter(SourceWriter writer, ACDataMember member) {
            writer.WriteLine($"writer.Write{member.MemberType}({member.Name});");
        }

        public void GenerateMemberWriter(SourceWriter writer, ACDataMember member, ACDataMember parent,
            string pre = "") {
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
                    writer.WriteLine(
                        $"writer.{TypeGeneratorHelper.GetBinaryWriterForType(member.MemberType)}(({TypeGeneratorHelper.SimplifyType(member.MemberType)}){member.LengthFor.Name}.Count() - {member.LengthFor.LengthMod});");
                }
                else {
                    writer.WriteLine(
                        $"writer.{TypeGeneratorHelper.GetBinaryWriterForType(member.MemberType)}(({TypeGeneratorHelper.SimplifyType(member.MemberType)}){(string.IsNullOrEmpty(member.LengthOf) ? member.LengthFor.Name : member.LengthOf)}.Count());");
                }

                return;
            }

            if (member.SubMembers.Count > 0) {
                writer.WriteLine($"{TypeGeneratorHelper.SimplifyType(member.MemberType)} {member.Name} = default;");
                foreach (var sub in member.SubMembers) {
                    if (!string.IsNullOrEmpty(sub.Mask) && !string.IsNullOrEmpty(sub.Shift)) {
                        writer.WriteLine(
                            $"{member.Name} |= ({member.MemberType})((({member.MemberType}){sub.Name} << {sub.Shift}) & {sub.Mask});");
                    }
                    else if (!string.IsNullOrEmpty(sub.Mask)) {
                        writer.WriteLine(
                            $"{member.Name} |= ({member.MemberType})(({member.MemberType}){sub.Name} & {sub.Mask});");
                    }
                    else if (!string.IsNullOrEmpty(sub.Shift)) {
                        writer.WriteLine(
                            $"{member.Name} |= ({member.MemberType})(({member.MemberType}){sub.Name} << {sub.Shift});");
                    }
                    else {
                        writer.WriteLine($"{member.Name} |= ({member.MemberType}){member.Value};");
                    }
                }

                writer.WriteLine(
                    $"writer.{TypeGeneratorHelper.GetBinaryWriterForType(member.MemberType)}({member.Name});");
                return;
            }

            if (!string.IsNullOrEmpty(member.Value)) {
                writer.WriteLine(
                    $"writer.{TypeGeneratorHelper.GetBinaryWriterForType(member.MemberType)}({member.Value}{(string.IsNullOrEmpty(member.Size) ? "" : $", {member.Size}")});");
                return;
            }

            if (!string.IsNullOrEmpty(member.KnownType)) {
                writer.WriteLine(
                    $"writer.{TypeGeneratorHelper.GetBinaryWriterForType(member.MemberType)}({member.Name}, {member.KnownType});");
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
                        writer.WriteLine(
                            $"writer.WriteItem<{TypeGeneratorHelper.GetTypeDeclaration(member, XMLDefParser)}>({member.Name});");
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
                    writer.WriteLine(
                        $"writer.{TypeGeneratorHelper.GetBinaryWriterForType(member.MemberType)}({member.Name}{(string.IsNullOrEmpty(member.Size) ? "" : $", {member.Size}")});");
                }
                else if (member.Parent?.GetType() == typeof(ACVector) && member.Parent?.Children?.Count > 1) {
                    writer.WriteLine($"writer.Write({pre}{member.Name});");
                }
                else {
                    writer.WriteLine($"writer.Write({pre.TrimEnd('.')});");
                }
            }
        }

        public void GenerateWriterContents(SourceWriter writer, ACBaseModel model, int depth, ACBaseModel parent = null,
            string pre = "") {
            switch (model) {
                case ACDataMember member:
                    GenerateMemberWriter(writer, member, parent as ACDataMember, pre);
                    break;
                case ACIf acIf:
                    GenerateWriterIf(writer, acIf, depth);
                    break;
                case ACMaskMap maskMap:
                    GenerateWriterMaskMap(writer, maskMap, depth);
                    break;
                case ACAlign:
                    writer.WriteLine($"writer.Align(4);");
                    break;
                case ACSwitch acSwitch:
                    GenerateWriterSwitch(writer, acSwitch, depth);
                    break;
                case ACVector vector:
                    GenerateWriterVector(writer, vector, depth, pre);
                    break;
            }
        }

        private void GenerateWriterIf(SourceWriter writer, ACIf acif, int depth) {
            writer.WriteLine($"if ({acif.WriteCondition ?? acif.Condition}) {{");
            using (writer.IndentScope()) {
                foreach (var member in acif.Children) {
                    GenerateWriterContents(writer, member, depth);
                }
            }

            writer.WriteLine("}");
        }

        private void GenerateWriterMaskMap(SourceWriter writer, ACMaskMap maskMap, int depth) {
            foreach (var mask in maskMap.Masks) {
                WriteMaskMapCheckStart(writer, maskMap, mask);
                using (writer.IndentScope()) {
                    foreach (var maskChild in mask.Children) {
                        GenerateWriterContents(writer, maskChild, depth);
                    }
                }

                writer.WriteLine("}"); // End if

                if (mask.ElseChildren.Count > 0) {
                    writer.WriteLine("else {");
                    using (writer.IndentScope()) {
                        foreach (var elseChild in mask.ElseChildren) {
                            GenerateWriterContents(writer, elseChild, depth);
                        }
                    }

                    writer.WriteLine("}");
                }
            }
        }

        private void GenerateWriterSwitch(SourceWriter writer, ACSwitch acswitch, int depth) {
            // switches need to define locally used variables first
            var usedNames = new List<string>();
            if (acswitch.Parent.GetType() != typeof(ACDataType) && acswitch.Parent.GetType() != typeof(ACDBObj)) {
                foreach (var scase in acswitch.Cases) {
                    if (acswitch.Parent is ACSwitchCase)
                        break;
                    foreach (ACBaseModel child in
                             scase.AllChildren.Where(c => c is ACDataMember || c is ACVector)) {
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

                    using (writer.IndentScope()) {
                        foreach (var child in scase.Children) {
                            GenerateWriterContents(writer, child, depth);
                        }

                        writer.WriteLine("break;");
                    }
                }
            }

            writer.WriteLine("}");
        }

        private void GenerateWriterVector(SourceWriter writer, ACVector vector, int depth, string pre) {
            bool areGenericArgs = vector.Children.Any(c => c is ACDataMember m && string.IsNullOrEmpty(m.Name)) ||
                                  vector.Children.Any(c => c is ACVector v && string.IsNullOrEmpty(v.Name));

            if (vector.IsGenericNonContainer || areGenericArgs && string.IsNullOrEmpty(vector.Length) && string.IsNullOrEmpty(vector.LengthMod) && string.IsNullOrEmpty(vector.Skip)) {
                 writer.WriteLine($"writer.WriteItem<{TypeGeneratorHelper.GetTypeDeclaration(vector, XMLDefParser)}>({vector.Name});");
                 return;
            }

            if (string.IsNullOrEmpty(vector.GenericKey) && string.IsNullOrEmpty(vector.GenericValue) && !areGenericArgs) {
                var indexVar =
                    new string[] { "i", "x", "y", "z", "q", "p", "t", "r", "f", "g", "h", "k", "u", "v" }[depth];
                if (!string.IsNullOrEmpty(vector.Skip)) {
                    writer.WriteLine(
                        $"for (var {indexVar}=0; {indexVar} < {pre}{vector.Length} - {vector.Skip}; {indexVar}++) {{");
                }
                else {
                    writer.WriteLine(
                        $"for (var {indexVar}=0; {indexVar} < {vector.Name}.Count(); {indexVar}++) {{");
                }

                using (writer.IndentScope()) {
                    writer.WriteLine(
                        $"writer.{TypeGeneratorHelper.GetBinaryWriterForType(vector.Type)}({vector.Name}[{indexVar}]);");
                }

                writer.WriteLine("}");
            }
            else if (string.IsNullOrEmpty(vector.GenericKey) && (!areGenericArgs || vector.Children.Count == 1)) {
                writer.WriteLine($"foreach (var item in {vector.Name}) {{");
                using (writer.IndentScope()) {
                    if (areGenericArgs) {
                         var valModel = vector.Children[0];
                         if (valModel is ACDataMember valDm) {
                            if (XMLDefParser.ACEnums.ContainsKey(valDm.MemberType)) {
                                writer.WriteLine($"writer.{TypeGeneratorHelper.GetBinaryWriterForType(XMLDefParser.ACEnums[valDm.MemberType].ParentType)}(({XMLDefParser.ACEnums[valDm.MemberType].ParentType})item);");
                            } else {
                                writer.WriteLine($"writer.{TypeGeneratorHelper.GetBinaryWriterForType(valDm.MemberType)}(item);");
                            }
                         } else if (valModel is ACVector v) {
                             writer.WriteLine($"writer.WriteItem<{TypeGeneratorHelper.GetTypeDeclaration(v, XMLDefParser)}>(item);");
                         }
                    }
                    else if (XMLDefParser.ACEnums.ContainsKey(vector.GenericValue)) {
                        writer.WriteLine(
                            $"writer.{TypeGeneratorHelper.GetBinaryWriterForType(XMLDefParser.ACEnums[vector.GenericValue].ParentType)}(({XMLDefParser.ACEnums[vector.GenericValue].ParentType})item);");
                    }
                    else {
                        writer.WriteLine(
                            $"writer.{TypeGeneratorHelper.GetBinaryWriterForType(vector.GenericValue)}(item);");
                    }
                }

                writer.WriteLine("}");
            }
            else {
                writer.WriteLine($"foreach (var kv in {vector.Name}) {{");
                using (writer.IndentScope()) {
                    if (areGenericArgs) {
                        var keyModel = vector.Children[0];
                        var valModel = vector.Children[1];
                        
                         if (keyModel is ACDataMember keyDm) {
                            if (XMLDefParser.ACEnums.ContainsKey(keyDm.MemberType)) {
                                writer.WriteLine($"writer.{TypeGeneratorHelper.GetBinaryWriterForType(XMLDefParser.ACEnums[keyDm.MemberType].ParentType)}(({XMLDefParser.ACEnums[keyDm.MemberType].ParentType})kv.Key);");
                            } else {
                                writer.WriteLine($"writer.{TypeGeneratorHelper.GetBinaryWriterForType(keyDm.MemberType)}(kv.Key);");
                            }
                         } else if (keyModel is ACVector v) {
                             writer.WriteLine($"writer.WriteItem<{TypeGeneratorHelper.GetTypeDeclaration(v, XMLDefParser)}>(kv.Key);");
                         }
                         
                         if (valModel is ACDataMember valDm) {
                            if (XMLDefParser.ACEnums.ContainsKey(valDm.MemberType)) {
                                writer.WriteLine($"writer.{TypeGeneratorHelper.GetBinaryWriterForType(XMLDefParser.ACEnums[valDm.MemberType].ParentType)}(({XMLDefParser.ACEnums[valDm.MemberType].ParentType})kv.Value);");
                            } else {
                                writer.WriteLine($"writer.{TypeGeneratorHelper.GetBinaryWriterForType(valDm.MemberType)}(kv.Value);");
                            }
                         } else if (valModel is ACVector v) {
                             writer.WriteLine($"writer.WriteItem<{TypeGeneratorHelper.GetTypeDeclaration(v, XMLDefParser)}>(kv.Value);");
                         }
                    }
                    else {
                        if (XMLDefParser.ACEnums.ContainsKey(vector.GenericKey)) {
                            writer.WriteLine(
                                $"writer.{TypeGeneratorHelper.GetBinaryWriterForType(XMLDefParser.ACEnums[vector.GenericKey].ParentType)}(({XMLDefParser.ACEnums[vector.GenericKey].ParentType})kv.Key);");
                        }
                        else {
                            writer.WriteLine(
                                $"writer.{TypeGeneratorHelper.GetBinaryWriterForType(vector.GenericKey)}(kv.Key);");
                        }

                        if (XMLDefParser.ACEnums.ContainsKey(vector.GenericValue)) {
                            writer.WriteLine(
                                $"writer.{TypeGeneratorHelper.GetBinaryWriterForType(XMLDefParser.ACEnums[vector.GenericValue].ParentType)}(({XMLDefParser.ACEnums[vector.GenericValue].ParentType})kv.Value);");
                        }
                        else {
                            writer.WriteLine(
                                $"writer.{TypeGeneratorHelper.GetBinaryWriterForType(vector.GenericValue)}(kv.Value);");
                        }
                    }
                }

                writer.WriteLine("}");
            }
        }

        public void WriteAbstractTypeGetter(SourceWriter writer, ACDataType type) {
            var dType = (type.Parent as DatReaderWriter.SourceGenerator.Models.ACDataType);
            if (dType is null || string.IsNullOrEmpty(dType.TypeSwitch)) return;
            var name = dType.TypeSwitch.Substring(1, 1).ToUpper() + dType.TypeSwitch.Substring(2);
            var switched =
                dType.AllChildren.FirstOrDefault(s =>
                    s is ACDataMember m && m.Name == dType.TypeSwitch) as ACDataMember;
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
                parentTypeType = TypeGeneratorHelper.SimplifyType(parentType?.Name);
                parentTypeName = TypeGeneratorHelper.SimplifyType(parentType?.Name);
                writer.WriteLine(
                    $"private {TypeGeneratorHelper.SimplifyType(parentType?.Name)} {TypeGeneratorHelper.SimplifyType(parentType?.Name).ToLower()};"); // Fixed field definition
            }
            else {
                var parentType2 = XMLDefParser.ACDBObjs.Values
                        .FirstOrDefault(t => t.Children.Any(c => c is ACVector v && v.GenericValue == type.Name))
                        .Children
                        .FirstOrDefault(c => c is ACDataMember m && m.Name == child.Length.Substring(7))
                    as ACDataMember;
                parentTypeType = TypeGeneratorHelper.SimplifyType(parentType2?.MemberType);
                parentTypeName = TypeGeneratorHelper.SimplifyType(parentType2?.Name);
                writer.WriteLine($"private {parentType2.MemberType} {parentType2.Name};");
            }

            writer.WriteLine("");

            writer.WriteLine($"public {type.Name}({parentTypeType} {parentTypeName}) {{");
            using (writer.IndentScope()) {
                if (parentType is not null) {
                    writer.WriteLine(
                        $"this.{TypeGeneratorHelper.SimplifyType(parentType?.Name).ToLower()} = {parentTypeName};");
                }
                else {
                    writer.WriteLine($"this.{parentTypeName} = {parentTypeName};");
                }
            }

            writer.WriteLine("}");
            writer.WriteLine("");
        }

        public void GenerateGenericRead(SourceWriter writer, string genericType, string varName) {
            writer.WriteLine($"{genericType} {varName};");
            writer.WriteLine(
                $"if (typeof({genericType}) == typeof(uint)) {{ {varName} = ({genericType})(object){TypeGeneratorHelper.GetBinaryReaderForType("uint")}; }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(int)) {{ {varName} = ({genericType})(object){TypeGeneratorHelper.GetBinaryReaderForType("int")}; }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(ulong)) {{ {varName} = ({genericType})(object){TypeGeneratorHelper.GetBinaryReaderForType("ulong")}; }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(long)) {{ {varName} = ({genericType})(object){TypeGeneratorHelper.GetBinaryReaderForType("long")}; }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(ushort)) {{ {varName} = ({genericType})(object){TypeGeneratorHelper.GetBinaryReaderForType("ushort")}; }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(short)) {{ {varName} = ({genericType})(object){TypeGeneratorHelper.GetBinaryReaderForType("short")}; }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(byte)) {{ {varName} = ({genericType})(object){TypeGeneratorHelper.GetBinaryReaderForType("byte")}; }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(sbyte)) {{ {varName} = ({genericType})(object){TypeGeneratorHelper.GetBinaryReaderForType("sbyte")}; }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(bool)) {{ {varName} = ({genericType})(object){TypeGeneratorHelper.GetBinaryReaderForType("bool")}; }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(float)) {{ {varName} = ({genericType})(object){TypeGeneratorHelper.GetBinaryReaderForType("float")}; }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(double)) {{ {varName} = ({genericType})(object){TypeGeneratorHelper.GetBinaryReaderForType("double")}; }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(string)) {{ {varName} = ({genericType})(object){TypeGeneratorHelper.GetBinaryReaderForType("string")}; }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(Guid)) {{ {varName} = ({genericType})(object){TypeGeneratorHelper.GetBinaryReaderForType("guid")}; }}");
            writer.WriteLine(
                $"else if (typeof(IUnpackable).IsAssignableFrom(typeof({genericType}))) {{ var item = (IUnpackable)Activator.CreateInstance(typeof({genericType})); item.Unpack(reader); {varName} = ({genericType})item; }}");
            writer.WriteLine(
                $"else {{ throw new NotSupportedException($\"Type {{typeof({genericType})}} is not supported by reader.\"); }}");
        }

        public void GenerateGenericWrite(SourceWriter writer, string genericType, string varName) {
            writer.WriteLine(
                $"if (typeof({genericType}) == typeof(uint)) {{ writer.{TypeGeneratorHelper.GetBinaryWriterForType("uint")}((uint)(object){varName}); }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(int)) {{ writer.{TypeGeneratorHelper.GetBinaryWriterForType("int")}((int)(object){varName}); }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(ulong)) {{ writer.{TypeGeneratorHelper.GetBinaryWriterForType("ulong")}((ulong)(object){varName}); }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(long)) {{ writer.{TypeGeneratorHelper.GetBinaryWriterForType("long")}((long)(object){varName}); }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(ushort)) {{ writer.{TypeGeneratorHelper.GetBinaryWriterForType("ushort")}((ushort)(object){varName}); }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(short)) {{ writer.{TypeGeneratorHelper.GetBinaryWriterForType("short")}((short)(object){varName}); }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(byte)) {{ writer.{TypeGeneratorHelper.GetBinaryWriterForType("byte")}((byte)(object){varName}); }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(sbyte)) {{ writer.{TypeGeneratorHelper.GetBinaryWriterForType("sbyte")}((sbyte)(object){varName}); }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(bool)) {{ writer.{TypeGeneratorHelper.GetBinaryWriterForType("bool")}((bool)(object){varName}); }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(float)) {{ writer.{TypeGeneratorHelper.GetBinaryWriterForType("float")}((float)(object){varName}); }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(double)) {{ writer.{TypeGeneratorHelper.GetBinaryWriterForType("double")}((double)(object){varName}); }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(string)) {{ writer.{TypeGeneratorHelper.GetBinaryWriterForType("string")}((string)(object){varName}); }}");
            writer.WriteLine(
                $"else if (typeof({genericType}) == typeof(Guid)) {{ writer.{TypeGeneratorHelper.GetBinaryWriterForType("guid")}((Guid)(object){varName}); }}");
            writer.WriteLine($"else if ({varName} is IPackable packable) {{ packable.Pack(writer); }}");
            writer.WriteLine(
                $"else {{ throw new NotSupportedException($\"Type {{typeof({genericType})}} is not supported by {varName} writer.\"); }}");
        }
    }
}
