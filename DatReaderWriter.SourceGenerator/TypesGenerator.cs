using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using DatReaderWriter.SourceGenerator.Models;

namespace DatReaderWriter.SourceGenerator {
    public class TypesGenerator : BaseGenerator {
        public TypesGenerator(XMLDefParser parser) : base(parser) { }

        public override void Generate(SourceProductionContext spc, XMLDefParser parser) {
            // Define the ignore list for types
            var ignoreList = new HashSet<string> {
                "Vector3",
                "StateDesc",
                "Quaternion",
                "Plane",
                "DBObj",
                "SpellBase",
                "BaseProperty",
                "ArrayBaseProperty",
                "StructBaseProperty",
                "TerrainInfo",
                "T4Template" // Added T4Template to the ignore list as per instruction
            };

            foreach (var kv in parser.ACDataTypes) {
                if (ignoreList.Contains(kv.Key)) {
                    continue;
                }

                // We only care about types that are NOT hash tables
                if (!string.IsNullOrEmpty(kv.Value.GenericKey)) {
                    continue;
                }

                var writer = new SourceWriter();
                var dataType = kv.Value;

                if (dataType.IsTemplated)
                    continue;

                writer.WriteLine("using System;");
                writer.WriteLine("using System.Numerics;");
                writer.WriteLine("using System.IO;");
                writer.WriteLine("using System.Linq;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using DatReaderWriter.Enums;");
                writer.WriteLine("using DatReaderWriter.Lib;");
                writer.WriteLine("using DatReaderWriter.Lib.Attributes;");
                writer.WriteLine("using DatReaderWriter.Lib.IO;");
                writer.WriteLine("");
                writer.WriteLine("namespace DatReaderWriter.Types {");

                using (writer.IndentScope()) {
                    WriteSummary(writer, dataType.Text);

                    if (dataType.IsAbstractImplementation) {
                        writer.WriteLine("public" + (dataType.IsAbstract ? " abstract" : "") + " partial class " +
                                         dataType.Name + " : " + dataType.ParentType + " {");
                    }
                    else {
                        var baseType = !string.IsNullOrEmpty(dataType.ParentType) ? dataType.ParentType : "IDatObjType";
                        writer.WriteLine("public" + (dataType.IsAbstract ? " abstract" : "") + " partial class " +
                                         dataType.Name + " : " + baseType + " {");
                    }

                    using (writer.IndentScope()) {
                        var usedPropertyNames = new List<string>();

                        if (dataType.IsAbstractImplementation) {
                            WriteAbstractTypeGetter(writer, dataType);
                        }

                        foreach (var baseModel in dataType.AllChildren) {
                            GenerateClassProperties(writer, baseModel, ref usedPropertyNames);
                        }

                        WriteParentContructor(writer, dataType);

                        // define method that can parse from binary
                        writer.WriteLine("/// <inheritdoc />");

                        if (dataType.IsAbstractImplementation) {
                            writer.WriteLine("public override bool Unpack(DatBinReader reader) {");
                        }
                        else if (dataType.IsAbstract) {
                            writer.WriteLine("public virtual bool Unpack(DatBinReader reader) {");
                        }
                        else {
                            writer.WriteLine("public bool Unpack(DatBinReader reader) {");
                        }

                        using (writer.IndentScope()) {
                            if (dataType.IsAbstractImplementation) {
                                writer.WriteLine("base.Unpack(reader);");
                            }

                            foreach (var child in dataType.Children) {
                                GenerateReaderContents(writer, child, 0);
                            }

                            writer.WriteLine("return true;");
                        }

                        writer.WriteLine("}");
                        writer.WriteLine("");

                        // define method that can parse to binary
                        writer.WriteLine("/// <inheritdoc />");
                        if (dataType.IsAbstractImplementation) {
                            writer.WriteLine("public override bool Pack(DatBinWriter writer) {");
                        }
                        else if (dataType.IsAbstract) {
                            writer.WriteLine("public virtual bool Pack(DatBinWriter writer) {");
                        }
                        else {
                            writer.WriteLine("public bool Pack(DatBinWriter writer) {");
                        }

                        using (writer.IndentScope()) {
                            if (dataType.IsAbstractImplementation) {
                                writer.WriteLine("base.Pack(writer);");
                            }

                            foreach (var child in dataType.Children) {
                                GenerateWriterContents(writer, child, 0);
                            }

                            writer.WriteLine("return true;");
                        }

                        writer.WriteLine("}");
                        writer.WriteLine("");

                        if (!string.IsNullOrEmpty(dataType.TypeSwitch)) {
                            WriteSummary(writer, "Create a typed instance of this abstract class");
                            // Calculate GetParameterType
                            string paramType = "";
                            var switchMember =
                                dataType.AllChildren.FirstOrDefault(c =>
                                    c is ACDataMember m && m.Name == dataType.TypeSwitch) as ACDataMember;
                            if (switchMember != null) {
                                if (XMLDefParser.ACEnums.ContainsKey(switchMember.MemberType))
                                    paramType = switchMember.MemberType;
                                else
                                    paramType = TypeGeneratorHelper.SimplifyType(switchMember.MemberType);
                            }

                            writer.WriteLine(
                                $"public static {dataType.Name}? Unpack(DatBinReader reader, {paramType} type) {{");
                            using (writer.IndentScope()) {
                                writer.WriteLine($"{dataType.Name}? instance = null;");
                                writer.WriteLine("switch(type) {");
                                using (writer.IndentScope()) {
                                    foreach (var subType in dataType.SubTypes) {
                                        writer.WriteLine($"case {subType.Value}:");
                                        using (writer.IndentScope()) {
                                            writer.WriteLine($"instance = new {subType.Name}();");
                                            writer.WriteLine("break;");
                                        }
                                    }
                                }

                                writer.WriteLine("}");
                                writer.WriteLine("instance?.Unpack(reader);");
                                writer.WriteLine("return instance;");
                            }

                            writer.WriteLine("}");
                        }
                    }

                    writer.WriteLine("}");
                }

                writer.WriteLine("}");

                spc.AddSource($"Types/{dataType.Name}.generated.cs", writer.ToString());
            }
        }
    }
}
