using System.Text;
using System.Collections.Generic;
using DatReaderWriter.SourceGenerator.Models;
using Microsoft.CodeAnalysis;

namespace DatReaderWriter.SourceGenerator {
    public class EnumsGenerator {
        public static void Generate(SourceProductionContext spc, XMLDefParser parser) {
            // Define the ignore list for enums
            var ignoreList = new HashSet<string> {
                "BSPType"
            };

            foreach (var kv in parser.ACEnums) {
                if (ignoreList.Contains(kv.Key)) continue;

                var writer = new SourceWriter();
                var enumDef = kv.Value;

                writer.WriteLine("using System;");
                writer.WriteLine("namespace DatReaderWriter.Enums {");
                
                using (writer.IndentScope()) {
                    if (!string.IsNullOrWhiteSpace(enumDef.Text)) {
                        writer.WriteLine("/// <summary>");
                        writer.WriteLine("/// " + System.Net.WebUtility.HtmlEncode(enumDef.Text));
                        writer.WriteLine("/// </summary>");
                    }

                    if (enumDef.IsMask)
                        writer.WriteLine("[Flags]");

                    writer.WriteLine($"public enum {kv.Key} : {enumDef.ParentType} {{");
                    
                    using (writer.IndentScope()) {
                        foreach (var enumValue in enumDef.Values) {
                            if (!string.IsNullOrWhiteSpace(enumValue.Text)) {
                                writer.WriteLine("/// <summary>");
                                writer.WriteLine("/// " + System.Net.WebUtility.HtmlEncode(enumValue.Text));
                                writer.WriteLine("/// </summary>");
                            }
                            writer.WriteLine($"{enumValue.Name} = {enumValue.Value},");
                            writer.WriteLine("");
                        }
                    }
                    writer.WriteLine("};");
                }
                writer.WriteLine("}");

                spc.AddSource($"Enums/{kv.Key}.generated.cs", writer.ToString());
            }

            // Generate DBObjType enum
            var dbObjWriter = new SourceWriter();
            dbObjWriter.WriteLine("using System;");
            dbObjWriter.WriteLine("namespace DatReaderWriter.Enums {");
            using (dbObjWriter.IndentScope()) {
                dbObjWriter.WriteLine("/// <summary>");
                dbObjWriter.WriteLine("/// DBObjTypes");
                dbObjWriter.WriteLine("/// </summary>");
                dbObjWriter.WriteLine("public enum DBObjType : int {");
                
                using (dbObjWriter.IndentScope()) {
                    dbObjWriter.WriteLine("/// <summary>");
                    dbObjWriter.WriteLine("/// Unknown type");
                    dbObjWriter.WriteLine("/// </summary>");
                    dbObjWriter.WriteLine("Unknown,");
                    dbObjWriter.WriteLine("");

                    dbObjWriter.WriteLine("/// <summary>");
                    dbObjWriter.WriteLine("/// DBObj Iteration");
                    dbObjWriter.WriteLine("/// </summary>");
                    dbObjWriter.WriteLine("Iteration,");
                    dbObjWriter.WriteLine("");

                    foreach (var dataType in parser.ACDBObjs.Values) {
                        dbObjWriter.WriteLine("/// <summary>");
                        dbObjWriter.WriteLine($"/// DBObj {dataType.Name} - {dataType.Text}");
                        dbObjWriter.WriteLine("/// </summary>");
                        dbObjWriter.WriteLine($"{dataType.Name},");
                        dbObjWriter.WriteLine("");
                    }
                }
                dbObjWriter.WriteLine("};");
            }
            dbObjWriter.WriteLine("}");
            spc.AddSource("Enums/DBObjType.generated.cs", dbObjWriter.ToString());
        }
    }
}
