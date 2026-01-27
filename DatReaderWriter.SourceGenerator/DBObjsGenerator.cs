using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using DatReaderWriter.SourceGenerator.Models;

namespace DatReaderWriter.SourceGenerator {
    public class DBObjsGenerator : BaseGenerator {
        public DBObjsGenerator(XMLDefParser parser) : base(parser) { }

        public override void Generate(SourceProductionContext spc, XMLDefParser parser) {
            foreach (var kv in parser.ACDBObjs) {
                if (kv.Value.Children.Count == 0) continue;
                if (kv.Key == "DBProperties" || kv.Key == "MasterProperty" || kv.Key == "LandBlock" || kv.Key == "ActionMap" || kv.Key == "Iteration") {
                    continue;
                }

                var writer = new SourceWriter();
                var dbObj = kv.Value;

                writer.WriteLine("using System;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using System.IO;");
                writer.WriteLine("using System.Linq;");
                writer.WriteLine("using System.Numerics;");
                writer.WriteLine("using DatReaderWriter.Enums;");
                writer.WriteLine("using DatReaderWriter.Lib;");
                writer.WriteLine("using DatReaderWriter.Lib.Attributes;");
                writer.WriteLine("using DatReaderWriter.Lib.IO;");
                writer.WriteLine("using DatReaderWriter.Types;");
                writer.WriteLine("");

                writer.WriteLine("namespace DatReaderWriter.DBObjs {");
                    using (writer.IndentScope()) {
                    WriteSummary(writer, dbObj.Text);
                    var baseType = string.IsNullOrEmpty(dbObj.BaseType) ? "IDatObjType" : dbObj.BaseType;
                    writer.WriteLine($"public partial class {dbObj.Name} : {baseType} {{");
                    
                    using (writer.IndentScope()) {
                        var usedPropertyNames = new List<string>();

                        // HeaderFlags
                        var flags = string.IsNullOrEmpty(dbObj.DBObjHeaderFlags) ? "DBObjHeaderFlags.None" : dbObj.DBObjHeaderFlags;
                        writer.WriteLine("/// <inheritdoc />");
                        writer.WriteLine($"public override DBObjHeaderFlags HeaderFlags => {flags};");
                        writer.WriteLine("");

                        // DBObjType
                        writer.WriteLine("/// <inheritdoc />");
                        writer.WriteLine($"public override DBObjType DBObjType => DBObjType.{dbObj.Name};");
                        writer.WriteLine("");

                        foreach (var baseModel in dbObj.AllChildren) {
                            GenerateClassProperties(writer, baseModel, ref usedPropertyNames);
                        }

                        // Unpack method
                        writer.WriteLine("/// <inheritdoc />");
                        var methodPrefix = baseType == "IDatObjType" ? "public" : "public override";
                        writer.WriteLine($"{methodPrefix} bool Unpack(DatBinReader reader) {{");
                        using (writer.IndentScope()) {
                            if (baseType != "IDatObjType") {
                                writer.WriteLine("base.Unpack(reader);");
                            }
                            foreach (var child in dbObj.Children) {
                                GenerateReaderContents(writer, child, 0);
                            }
                            writer.WriteLine("return true;");
                        }
                        writer.WriteLine("}");
                        writer.WriteLine("");

                         // Pack method
                        writer.WriteLine("/// <inheritdoc />");
                        writer.WriteLine($"{methodPrefix} bool Pack(DatBinWriter writer) {{");
                        using (writer.IndentScope()) {
                            if (baseType != "IDatObjType") {
                                writer.WriteLine("base.Pack(writer);");
                            }
                            foreach (var child in dbObj.Children) {
                                GenerateWriterContents(writer, child, 0);
                            }
                            writer.WriteLine("return true;");
                        }
                        writer.WriteLine("}");
                    }
                    writer.WriteLine("}");
                }
                writer.WriteLine("}");

                spc.AddSource($"DBObjs/{dbObj.Name}.generated.cs", writer.ToString());
            }
        }
    }
}
