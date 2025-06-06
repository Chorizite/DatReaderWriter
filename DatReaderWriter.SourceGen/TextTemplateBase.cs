﻿using DatReaderWriter.SourceGen.Models;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DatReaderWriter.SourceGen {

    /// <summary>
    /// Base TextTemplate class with built-in helpers for managing messages.xml data
    /// </summary>
    public abstract class TextTemplateBase {
        public class IndentHelper : IDisposable {
            public TextTemplateBase Template { get; }

            public IndentHelper(TextTemplateBase template, string indentStr = "    ") {
                Template = template;
                Template.Indent(indentStr);
            }

            public void Dispose() {
                Template.Outdent();
            }
        }

        const string _defaultIndentStr = "    ";
        string _currentIndent = "";
        bool _endsWithNewline;
        CompilerErrorCollection _errors;
        StringBuilder _generationEnvironment;
        List<int> _indentLengths;

        /// <summary>
        /// Messages.xml Reader class
        /// </summary>
        public XMLDefParser XMLDefParser;

        /// <summary>
        /// Create a new TextTemplateBase
        /// </summary>
        public TextTemplateBase() {
            ToStringHelper = new ToStringInstanceHelper();
        }

        public void SetupMessageParser(string path) {
            XMLDefParser = new XMLDefParser(path);
        }

        private object CreateEnum(ACEnum allMessageTypes, string name, string text, Func<XElement?, bool> cb) {
            var messageTypes = allMessageTypes.Values.Where(v => {
                var messageDef = XMLDefParser.Xml.XPathSelectElement($"/messages/message[@type='{v.Value}']");
                return cb?.Invoke(messageDef) ?? false;
            }).ToList();
            var messageTypeEnum = new XElement(name);
            messageTypeEnum.SetAttributeValue("text", text);
            messageTypeEnum.SetAttributeValue("parent", "uint");

            foreach (var type in allMessageTypes.Values) {
                var messageDef = XMLDefParser.Xml.XPathSelectElement($"/messages/message[@type='{type.Value}']");
                if (cb?.Invoke(messageDef) ?? false) {
                    messageTypeEnum.Add(type.Element);
                }
            }

            return messageTypeEnum;
        }

        /// <summary>
        /// Write out a class property definition
        /// </summary>
        /// <param name="member"></param>
        public abstract void WriteClassProperty(ACDataMember member);

        /// <summary>
        /// Write out a class property definition for a submember
        /// </summary>
        /// <param name="member"></param>
        public abstract void WriteClassProperty(ACSubMember member);

        /// <summary>
        /// Write out a class property definition for a vector
        /// </summary>
        /// <param name="vector"></param>
        public abstract void WriteClassVectorProperty(ACVector vector);

        /// <summary>
        /// Write out en enum reader
        /// </summary>
        /// <param name="member"></param>
        public abstract void WriteEnumReader(ACDataMember member);

        /// <summary>
        /// Write out a DataType reader
        /// </summary>
        /// <param name="member"></param>
        public abstract void WriteDataReader(ACDataMember member);

        /// <summary>
        /// write out a Primitive reader
        /// </summary>
        /// <param name="member"></param>
        public abstract void WritePrimitiveReader(ACDataMember member);

        /// <summary>
        /// Write out an if statement condition
        /// </summary>
        /// <param name="condition"></param>
        public abstract void WriteIfStatementStart(string condition);

        /// <summary>
        /// Write out an if statement ending (curly brace or whatever)
        /// </summary>
        /// <param name="condition"></param>
        public abstract void WriteIfStatementEnding(string condition);

        /// <summary>
        /// Write out an else statement start
        /// </summary>
        /// <param name="condition"></param>
        public abstract void WriteElseStatementStart();

        /// <summary>
        /// Write out an else statement ending (curly brace or whatever)
        /// </summary>
        /// <param name="condition"></param>
        public abstract void WriteElseStatementEnding();

        /// <summary>
        /// Write out the start of a mask map check (usually an if statement)
        /// </summary>
        /// <param name="mask"></param>
        public abstract void WriteMaskMapCheckStart(ACMaskMap maskMap, ACMask mask);

        /// <summary>
        /// Write out csharp code for the ending of a mask map check (usually an endif statement)
        /// </summary>
        /// <param name="mask"></param>
        public abstract void WriteMaskMapCheckEnding(ACMaskMap maskMap, ACMask mask);

        /// <summary>
        /// Write the code needed to align the buffer
        /// </summary>
        /// <param name="align"></param>
        public abstract void WriteAlignmentCheck(ACAlign align);

        /// <summary>
        /// Write out the start of a switch statement
        /// </summary>
        /// <param name="mask"></param>
        public abstract void WriteSwitchStart(ACSwitch acswitch);

        /// <summary>
        /// Write out csharp code for the ending of a switch statement
        /// </summary>
        /// <param name="mask"></param>
        public abstract void WriteSwitchEnding(ACSwitch acswitch);

        /// <summary>
        /// Write out the start of a switch case statement
        /// </summary>
        /// <param name="mask"></param>
        public abstract void WriteSwitchCaseStart(ACSwitchCase scase);

        /// <summary>
        /// Write out code for the ending of a switch case statement
        /// </summary>
        /// <param name="mask"></param>
        public abstract void WriteSwitchCaseEnding(ACSwitchCase scase);

        /// <summary>
        /// Write out the code for the start of a for loop
        /// </summary>
        /// <param name="mask"></param>
        public abstract string WriteForLoopStart(ACVector vector, int depth);

        /// <summary>
        /// Write out the code for the end of a for loop
        /// </summary>
        /// <param name="mask"></param>
        public abstract void WriteForLoopEnding(ACVector vector, int depth);

        /// <summary>
        /// Write out the code to push a single item to a vector
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="child"></param>
        public abstract void WriteVectorPusher(ACVector vector, ACBaseModel child);

        /// <summary>
        /// Write out the code to push a complex item to a vector
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="children"></param>
        public abstract void WriteVectorPusher(ACVector vector, List<ACBaseModel> children, string loopChar);

        /// <summary>
        /// Write out the code to define a local child vector
        /// </summary>
        /// <param name="vector"></param>
        public abstract void WriteLocalVectorDefinition(ACVector vector);

        /// <summary>
        /// Write out the code to define a local child variable
        /// </summary>
        /// <param name="child"></param>
        public abstract void WriteLocalChildDefinition(ACDataMember child);

        /// <summary>
        /// Write out binary reader for specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract string GetBinaryReaderForType(string type, string? size);

        /// <summary>
        /// Write out binary writer for specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract string GetBinaryWriterForType(string type);

        /// <summary>
        /// Writes a summary comment above classes/attribute/enum value definitions
        /// </summary>
        /// <param name="text"></param>
        public abstract void WriteSummary(string text);

        /// <summary>
        /// Writes a summary comment above classes/attribute/enum value definitions
        /// </summary>
        /// <param name="text"></param>
        public abstract void WriteUsingAliases(Dictionary<string, string> typeAliases);

        #region TemplateHelpers

        /// <summary>
        /// Simplifies an ac data type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual string SimplifyType(string type) {
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

        /// <summary>
        /// Write out all generated class attributes
        /// </summary>
        /// <param name="baseModel"></param>
        public void GenerateClassProperties(ACBaseModel baseModel, ref List<string> usedNames) {
            if (baseModel is ACDataMember) {
                var member = baseModel as ACDataMember;
                // we only want to include properties we haven't included yet
                if (!usedNames.Contains(member.Name)) {
                    usedNames.Add(member.Name);
                    WriteClassProperty(member);
                }

                foreach (var submember in member.SubMembers) {
                    WriteClassProperty(submember);
                }
            }
            else if (baseModel is ACVector) {
                var vector = baseModel as ACVector;
                WriteClassVectorProperty(vector);

                return; // dont generate child fields of vectors
            }

            // recursively go down the chain and add any found attributes
            foreach (var child in baseModel.Children) {
                GenerateClassProperties(child, ref usedNames);
            }
        }

        private void WriteVectorPusher(ACVector vector, string loopChar) {
            if (vector.Children.Count == 1) {
                WriteVectorPusher(vector, vector.Children.First());
            }
            else {
                WriteVectorPusher(vector, vector.Children, loopChar);
            }
        }

        public void WriteParentContructor(ACDataType type) {
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
                WriteLine($"private {SimplifyType(parentType?.Name)};");
            }
            else {
                var parentType2 = XMLDefParser.ACDBObjs.Values
                    .FirstOrDefault(t => t.Children.Any(c => c is ACVector v && v.GenericValue == type.Name))
                    .Children
                    .FirstOrDefault(c => c is ACDataMember m && m.Name == child.Length.Substring(7))
                    as ACDataMember;
                parentTypeType = SimplifyType(parentType2?.MemberType);
                parentTypeName = SimplifyType(parentType2?.Name);
                WriteLine($"private {parentType2.MemberType} {parentType2.Name};");
            }
            WriteLine("");

            WriteLine($"public {type.Name}({parentTypeType} {parentTypeName}) {{");
            Indent();
            WriteLine($"this.{parentTypeName} = {parentTypeName};");
            Outdent();
            WriteLine("}");
            WriteLine("");
        }

        /// <summary>
        /// Write out the code to align the buffer
        /// </summary>
        /// <param name="align"></param>
        public void WriteAlignmentCheck2(ACAlign align) {
            WriteLine($"writer.Align(4);");
        }

        /// <summary>
        /// Generates the reader code for an ACBaseModel (and its children)
        /// </summary>
        /// <param name="dataType"></param>
        public void GenerateWriterContents(ACBaseModel model, int depth, ACBaseModel parent = null, string pre = "") {
            if (model is ACDataMember) {
                GenerateMemberWriter(model as ACDataMember, parent as ACDataMember, pre);
            }
            else if (model is ACIf) {
                var acif = model as ACIf;
                WriteIfStatementStart(acif.WriteCondition ?? acif.Condition);
                foreach (var member in acif.Children) {
                    GenerateWriterContents(member, depth);
                }
                WriteIfStatementEnding(acif.WriteCondition ?? acif.Condition);
            }
            else if (model is ACMaskMap) {
                var maskMap = model as ACMaskMap;
                foreach (var mask in maskMap.Masks) {
                    WriteMaskMapCheckStart(maskMap, mask);
                    foreach (var maskChild in mask.Children) {
                        GenerateWriterContents(maskChild, depth);
                    }
                    WriteMaskMapCheckEnding(maskMap, mask);
                    if (mask.ElseChildren.Count > 0) {
                        WriteLine("else {");
                        Indent();
                        foreach (var elseChild in mask.ElseChildren) {
                            GenerateWriterContents(elseChild, depth);
                        }
                        Outdent();
                        WriteLine("}");
                    }
                }
            }
            else if (model is ACAlign) {
                WriteAlignmentCheck2(model as ACAlign);
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
                                WriteLocalChildDefinition(dm);
                                usedNames.Add(dm.Name);
                            }
                            else if (child is ACVector) {
                                WriteLocalVectorDefinition(child as ACVector);
                            }
                        }
                    }
                }
                WriteSwitchStart(acswitch);
                foreach (var scase in acswitch.Cases) {
                    WriteSwitchCaseStart(scase);
                    foreach (var child in scase.Children) {
                        GenerateWriterContents(child, depth);
                    }
                    WriteSwitchCaseEnding(scase);
                }
                WriteSwitchEnding(acswitch);
            }
            else if (model is ACVector) {
                var vector = model as ACVector;

                if (vector.Parent is ACVector) {
                    //WriteLocalVectorDefinition(vector);
                }

                if (string.IsNullOrEmpty(vector.GenericKey) && string.IsNullOrEmpty(vector.GenericValue)) {
                    var loopChar = WriteForLoopStart2(vector, pre, depth);
                    WriteLine($"writer.{GetBinaryWriterForType(vector.Type)}({vector.Name}[{loopChar}]);");
                    WriteForLoopEnding(vector, depth);
                }
                else if (string.IsNullOrEmpty(vector.GenericKey)) {
                    WriteLine($"foreach (var item in {vector.Name}) {{");
                    Indent();

                    if (XMLDefParser.ACEnums.ContainsKey(vector.GenericValue)) {
                        WriteLine($"writer.{GetBinaryWriterForType(XMLDefParser.ACEnums[vector.GenericValue].ParentType)}(({XMLDefParser.ACEnums[vector.GenericValue].ParentType})item);");
                    }
                    else {
                        WriteLine($"writer.{GetBinaryWriterForType(vector.GenericValue)}(item);");
                    }
                    Outdent();
                    WriteLine("}");
                }
                else {
                    WriteLine($"foreach (var kv in {vector.Name}) {{");
                    Indent();
                    if (XMLDefParser.ACEnums.ContainsKey(vector.GenericKey)) {
                        WriteLine($"writer.{GetBinaryWriterForType(XMLDefParser.ACEnums[vector.GenericKey].ParentType)}(({XMLDefParser.ACEnums[vector.GenericKey].ParentType})kv.Key);");
                    }
                    else {
                        WriteLine($"writer.{GetBinaryWriterForType(vector.GenericKey)}(kv.Key);");
                    }
                    if (XMLDefParser.ACEnums.ContainsKey(vector.GenericValue)) {
                        WriteLine($"writer.{GetBinaryWriterForType(XMLDefParser.ACEnums[vector.GenericValue].ParentType)}(({XMLDefParser.ACEnums[vector.GenericValue].ParentType})kv.Value);");
                    }
                    else {
                        WriteLine($"writer.{GetBinaryWriterForType(vector.GenericValue)}(kv.Value);");
                    }
                    Outdent();
                    WriteLine("}");
                }
            }
            else {
                WriteLine("Unknown Type: " + model.GetType());
            }
        }

        public string WriteForLoopStart2(ACVector vector, string pre, int depth) {
            var indexVar = new string[] { "i", "x", "y", "z", "q", "p", "t", "r", "f", "g", "h", "k", "u", "v" }[depth];
            if (!string.IsNullOrEmpty(vector.Skip)) {
                WriteLine($"for (var {indexVar}=0; {indexVar} < {pre}{vector.Length} - {vector.Skip}; {indexVar}++) {{");
            }
            else {
                WriteLine($"for (var {indexVar}=0; {indexVar} < {vector.Name}.Count(); {indexVar}++) {{");
            }
            Indent();
            return indexVar;
        }

        /// <summary>
        /// Generates the reader code for an ACBaseModel (and its children)
        /// </summary>
        /// <param name="dataType"></param>
        public void GenerateReaderContents(ACBaseModel model, int depth) {
            if (model is ACDataMember) {
                GenerateMemberReader(model as ACDataMember);
            }
            else if (model is ACIf) {
                var acif = model as ACIf;
                WriteIfStatementStart(acif.Condition);
                foreach (var member in acif.Children) {
                    GenerateReaderContents(member, depth);
                }
                WriteIfStatementEnding(acif.Condition);
            }
            else if (model is ACMaskMap) {
                var maskMap = model as ACMaskMap;
                foreach (var mask in maskMap.Masks) {
                    WriteMaskMapCheckStart(maskMap, mask);
                    foreach (var maskChild in mask.Children) {
                        GenerateReaderContents(maskChild, depth);
                    }
                    WriteMaskMapCheckEnding(maskMap, mask);
                    if (mask.ElseChildren.Count > 0) {
                        WriteLine("else {");
                        Indent();
                        foreach (var elseChild in mask.ElseChildren) {
                            GenerateReaderContents(elseChild, depth);
                        }
                        Outdent();
                        WriteLine("}");
                    }
                }
            }
            else if (model is ACAlign) {
                WriteAlignmentCheck(model as ACAlign);
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
                                WriteLocalChildDefinition(dm);
                                usedNames.Add(dm.Name);
                            }
                            else if (child is ACVector) {
                                WriteLocalVectorDefinition(child as ACVector);
                            }
                        }
                    }
                }
                WriteSwitchStart(acswitch);
                foreach (var scase in acswitch.Cases) {
                    WriteSwitchCaseStart(scase);
                    foreach (var child in scase.Children) {
                        GenerateReaderContents(child, depth);
                    }
                    WriteSwitchCaseEnding(scase);
                }
                WriteSwitchEnding(acswitch);
            }
            else if (model is ACVector) {
                var vector = model as ACVector;

                if (vector.Parent is ACVector) {
                    WriteLocalVectorDefinition(vector);
                }

                if (string.IsNullOrEmpty(vector.GenericKey) && string.IsNullOrEmpty(vector.GenericValue)) {
                    if (!string.IsNullOrEmpty(vector.LengthMod)) {
                        WriteLine($"{vector.Name} = new {SimplifyType(vector.Type)}[{vector.Length} + {vector.LengthMod}];");
                    }
                    else {
                        WriteLine($"{vector.Name} = new {SimplifyType(vector.Type)}[{vector.Length}];");
                    }
                }
                var loopChar = WriteForLoopStart(vector, depth);
                foreach (var vmember in vector.Children) {
                    GenerateReaderContents(vmember, depth + 1);
                }
                WriteVectorPusher(vector, loopChar);
                WriteForLoopEnding(vector, depth);
            }
            else {
                WriteLine("Unknown Type: " + model.GetType());
            }
        }

        /// <summary>
        /// Generates code to assign a class property to results from a binary reader
        /// </summary>
        /// <param name="member"></param>
        public void GenerateMemberReader(ACDataMember member) {
            bool isVector = member.HasParentOfType(typeof(ACVector));


            var type = XMLDefParser.ACDataTypes.Values
                   .FirstOrDefault(t => t.Name == member.MemberType);
            if (type != null && type.IsAbstract) {
                var field = type.Children
                    .FirstOrDefault(c => c is ACDataMember m && m.Name == type.TypeSwitch)
                    as ACDataMember;

                if (XMLDefParser.ACEnums.TryGetValue(field.MemberType, out var en)) {
                    var reader = GetBinaryReaderForType(XMLDefParser.ACEnums[field.MemberType].ParentType, null);
                    WriteLine($"var _peekedValue = (" + field.MemberType + ")" + reader + ";");
                }
                else {
                    WriteLine($"var _peekedValue = {GetBinaryReaderForType(field.MemberType, null)};");
                }
                WriteLine($"reader.Skip(-sizeof({field.MemberType}));");
                WriteLine($"{member.Name} = {member.MemberType}.Unpack(reader, _peekedValue);");
                return;
            }

            if (XMLDefParser.ACEnums.ContainsKey(member.MemberType)) {
                WriteEnumReader(member);
            }
            else if (XMLDefParser.ACDataTypes.ContainsKey(member.MemberType)) {
                WriteDataReader(member);
            }
            else if (XMLDefParser.ACTemplatedTypes.ContainsKey(member.MemberType)) {
                WriteTemplatedReader(member);
            }
            else {
                WritePrimitiveReader(member);
            }
        }

        private void WriteTemplatedReader(ACDataMember member) {
            if (!string.IsNullOrEmpty(member.GenericType)) {
                WriteLine($"{member.Name} = reader.Read{member.MemberType}<{SimplifyType(member.GenericType)}>();");
            }
            else {
                WriteLine($"{member.Name} = reader.Read{member.MemberType}<{SimplifyType(member.GenericKey)}, {SimplifyType(member.GenericValue)}>();");
            }
        }

        private void WriteTemplatedWriter(ACDataMember member) {
            WriteLine($"writer.Write{member.MemberType}({member.Name});");
        }

        public void WriteAbstractTypeGetter(ACDataType type) {
            var dType = (type.Parent as DatReaderWriter.SourceGen.Models.ACDataType);
            if (dType is null || string.IsNullOrEmpty(dType.TypeSwitch)) return;
            var name = dType.TypeSwitch.Substring(1, 1).ToUpper() + dType.TypeSwitch.Substring(2);
            var switched = dType.AllChildren.FirstOrDefault(s => s is ACDataMember m && m.Name == dType.TypeSwitch) as ACDataMember;
            WriteLine("/// <inheritdoc />");
            WriteLine($"public override {switched?.MemberType} {name} => {type?.Value};\n");
        }

        /// <summary>
        /// Writes out csharp to read an ACEnum
        /// </summary>
        /// <param name="member"></param>
        public void WriteEnumWriter(ACDataMember member, string pre = "") {

            if (member.Name.StartsWith("_") && member.Parent is ACDataType dType && dType.TypeSwitch == member.Name) {
                WriteLine($"writer.{GetBinaryWriterForType(XMLDefParser.ACEnums[member.MemberType].ParentType)}(({XMLDefParser.ACEnums[member.MemberType].ParentType}){member.Name.Substring(1,1).ToUpper() + member.Name.Substring(2)});");
                return;
            }
            /*
            if (member.Parent is ACDataType dType && dType.TypeSwitch == member.Name) {
                for (var i = 0; i < dType.SubTypes.Count; i++) {
                    var ifStr = i == 0 ? "if" : "else if";
                    WriteLine($"{ifStr} (this is {dType.SubTypes[i].Name}) {{");
                    Indent();
                    WriteLine($"writer.{GetBinaryWriterForType(XMLDefParser.ACEnums[member.MemberType].ParentType)}(({XMLDefParser.ACEnums[member.MemberType].ParentType}){dType.SubTypes[i].Value});");
                    Outdent();
                    WriteLine("}");
                }
                WriteLine("else {");
                Indent();
                WriteLine("""throw new Exception($"Unsupported Hook type: {GetType()}");""");
                Outdent();
                WriteLine("}");
                return;
            }
            */
            WriteLine($"writer.{GetBinaryWriterForType(XMLDefParser.ACEnums[member.MemberType].ParentType)}(({XMLDefParser.ACEnums[member.MemberType].ParentType}){member.Name});");
        }

        /// <summary>
        /// Generates code to assign a class property to results from a binary reader
        /// </summary>
        /// <param name="member"></param>
        public void GenerateMemberWriter(ACDataMember member, ACDataMember parent, string pre = "") {
            if (member.MemberType == "Vector3") {
                WriteLine($"writer.WriteVector3({pre}{member.Name});");
                return;
            }
            if (member.MemberType == "Quaternion") {
                WriteLine($"writer.WriteQuaternion({pre}{member.Name});");
                return;
            }
            if (XMLDefParser.ACTemplatedTypes.ContainsKey(member.MemberType)) {
                WriteTemplatedWriter(member);
                return;
            }

            if (member.IsLength) {
                if (!string.IsNullOrEmpty(member.LengthFor?.LengthMod)) {
                    WriteLine($"writer.{GetBinaryWriterForType(member.MemberType)}(({SimplifyType(member.MemberType)}){member.LengthFor.Name}.Count() - {member.LengthFor.LengthMod});");
                }
                else {
                    WriteLine($"writer.{GetBinaryWriterForType(member.MemberType)}(({SimplifyType(member.MemberType)}){(string.IsNullOrEmpty(member.LengthOf) ? member.LengthFor.Name : member.LengthOf)}.Count());");
                }
                return;
            }

            if (member.SubMembers.Count > 0) {
                WriteLine($"{SimplifyType(member.MemberType)} {member.Name} = default;");
                foreach (var sub in member.SubMembers) {
                    if (!string.IsNullOrEmpty(sub.Mask) && !string.IsNullOrEmpty(sub.Shift)) {
                        WriteLine($"{member.Name} |= ({member.MemberType})((({member.MemberType}){sub.Name} << {sub.Shift}) & {sub.Mask});");
                    }
                    else if (!string.IsNullOrEmpty(sub.Mask)) {
                        WriteLine($"{member.Name} |= ({member.MemberType})(({member.MemberType}){sub.Name} & {sub.Mask});");
                    }
                    else if (!string.IsNullOrEmpty(sub.Shift)) {
                        WriteLine($"{member.Name} |= ({member.MemberType})(({member.MemberType}){sub.Name} << {sub.Shift});");
                    }
                    else {
                        WriteLine($"{member.Name} |= ({member.MemberType}){member.Value};");
                    }
                }
                WriteLine($"writer.{GetBinaryWriterForType(member.MemberType)}({member.Name});");
                return;
            }

            if (!string.IsNullOrEmpty(member.Value)) {
                WriteLine($"writer.{GetBinaryWriterForType(member.MemberType)}({member.Value}{(string.IsNullOrEmpty(member.Size) ? "" : $", {member.Size}")});");
                return;
            }

            if (!string.IsNullOrEmpty(member.KnownType)) {
                WriteLine($"writer.{GetBinaryWriterForType(member.MemberType)}({member.Name}, {member.KnownType});");
                return;
            }

            if (!string.IsNullOrEmpty(parent?.GenericType) && parent.MemberType == "List") {
                if (XMLDefParser.ACEnums.ContainsKey(member.MemberType)) {
                    WriteEnumWriter(member, pre);
                }
                else if (XMLDefParser.ACDataTypes.ContainsKey(member.MemberType)) {
                    WriteLine($"{pre.TrimEnd('.')}.Write(writer);");
                }
                else {
                    WriteLine($"writer.Write({pre.TrimEnd('.')});");
                }
            }
            else {
                if (XMLDefParser.ACEnums.ContainsKey(member.MemberType)) {
                    WriteEnumWriter(member, pre);
                }
                else if (XMLDefParser.ACDataTypes.ContainsKey(member.MemberType)) {
                    if (string.IsNullOrEmpty(pre)) {
                        WriteLine($"writer.WriteItem<{member.MemberType}>({member.Name});");
                    }
                    else if (member.Parent?.GetType() == typeof(ACVector) && member.Parent?.Children?.Count == 1) {
                        WriteLine($"{pre}Write(writer);");
                    }
                    else if (member.MemberType == member.Name) {
                        WriteLine($"{pre}{member.Name}.Write(writer);");
                    }
                    else {
                        WriteLine($"{pre}Write(writer);");
                    }
                }
                else if (string.IsNullOrEmpty(pre)) {
                    WriteLine($"writer.{GetBinaryWriterForType(member.MemberType)}({member.Name}{(string.IsNullOrEmpty(member.Size) ? "" : $", {member.Size}")});");
                }
                else if (member.Parent?.GetType() == typeof(ACVector) && member.Parent?.Children?.Count > 1) {
                    WriteLine($"writer.Write({pre}{member.Name});");
                }
                else {
                    WriteLine($"writer.Write({pre.TrimEnd('.')});");
                }
            }
        }

        public string GetTypeDeclaration(ACDataMember member) {
            var simplifiedType = SimplifyType(member.MemberType);
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
        #endregion

        /// <summary>
        /// Increases the internal indent amount by one
        /// </summary>
        public void Indent(string indentStr = _defaultIndentStr) {
            PushIndent(indentStr);
        }

        /// <summary>
        /// Decreases the internal indent amount by one
        /// </summary>
        public void Outdent() {
            PopIndent();
        }

        /// <summary>
        /// Prints a warning not to modify this file, but to modify the template instead
        /// </summary>
        public void PrintLocalModificationWarning() {
            WriteLine("//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//");
            WriteLine("//                                                            //");
            WriteLine("//                          WARNING                           //");
            WriteLine("//                                                            //");
            WriteLine("//           DO NOT MAKE LOCAL CHANGES TO THIS FILE           //");
            WriteLine("//               EDIT THE .tt TEMPLATE INSTEAD                //");
            WriteLine("//                                                            //");
            WriteLine("//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//");
            WriteLine("\n");
        }

        #region Base Implementation
        /// <summary>
        /// Required to make this class a base class for T4 templates
        /// </summary>
        public abstract string TransformText();

        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected StringBuilder GenerationEnvironment {
            get { return _generationEnvironment ?? (_generationEnvironment = new StringBuilder()); }
            set { _generationEnvironment = value; }
        }

        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        List<int> IndentLengths {
            get { return _indentLengths ?? (_indentLengths = new List<int>()); }
        }

        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent {
            get { return _currentIndent; }
        }

        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual IDictionary<string, object> Session { get; set; }

        public void Initialize() {

        }

        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend) {
            if (string.IsNullOrEmpty(textToAppend))
                return;
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (GenerationEnvironment.Length == 0 || _endsWithNewline) {
                GenerationEnvironment.Append(_currentIndent);
                _endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(Environment.NewLine, StringComparison.CurrentCulture))
                _endsWithNewline = true;
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if (_currentIndent.Length == 0) {
                GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(Environment.NewLine, Environment.NewLine + _currentIndent);
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (_endsWithNewline)
                GenerationEnvironment.Append(textToAppend, 0, textToAppend.Length - _currentIndent.Length);
            else
                GenerationEnvironment.Append(textToAppend);
        }

        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend) {
            Write(textToAppend);
            Write("\r\n");
            _endsWithNewline = true;
        }

        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args) {
            Write(string.Format(CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args) {
            WriteLine(string.Format(CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent) {
            if (indent == null)
                throw new ArgumentNullException("indent");
            _currentIndent = _currentIndent + indent;
            IndentLengths.Add(indent.Length);
        }

        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent() {
            string returnValue = "";
            if (IndentLengths.Count > 0) {
                int indentLength = IndentLengths[IndentLengths.Count - 1];
                IndentLengths.RemoveAt(IndentLengths.Count - 1);
                if (indentLength > 0) {
                    returnValue = _currentIndent.Substring(_currentIndent.Length - indentLength);
                    _currentIndent = _currentIndent.Remove(_currentIndent.Length - indentLength);
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent() {
            IndentLengths.Clear();
            _currentIndent = "";
        }

        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper { get; private set; }

        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper {
            IFormatProvider _formatProvider = CultureInfo.InvariantCulture;

            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public IFormatProvider FormatProvider {
                get { return _formatProvider; }
                set {
                    if (value != null)
                        _formatProvider = value;
                }
            }

            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert) {
                if (objectToConvert == null)
                    throw new ArgumentNullException("objectToConvert");
                Type t = objectToConvert.GetType();
                MethodInfo method = t.GetMethod("ToString", new[]
                                                                {
                                                                    typeof (IFormatProvider)
                                                                });
                if (method == null)
                    return objectToConvert.ToString();
                else {
                    return (string)(method.Invoke(objectToConvert, new object[]
                                                                        {
                                                                            _formatProvider
                                                                        }));
                }
            }
        }

        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public CompilerErrorCollection Errors {
            get { return _errors ?? (_errors = new CompilerErrorCollection()); }
        }

        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message) {
            Errors.Add(new CompilerError { ErrorText = message });
        }
        #endregion
    }
}
