using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using DatReaderWriter.SourceGenerator.Models;

namespace DatReaderWriter.SourceGenerator {
    public class DatabaseReadersGenerator : BaseGenerator {
        public DatabaseReadersGenerator(XMLDefParser parser) : base(parser) { }

        public override void Generate(SourceProductionContext spc, XMLDefParser parser) {
             var readers = new List<string>() { "Portal", "Cell", "Local" };
             foreach (var reader in readers) {
                var writer = new SourceWriter();

                writer.WriteLine("using System.Numerics;");
                writer.WriteLine("using System.IO;");
                writer.WriteLine("using System.Linq;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using DatReaderWriter.Enums;");
                writer.WriteLine("using DatReaderWriter.Types;");
                writer.WriteLine("using DatReaderWriter.Lib;");
                writer.WriteLine("using DatReaderWriter.Lib.Attributes;");
                writer.WriteLine("using DatReaderWriter.Lib.IO;");
                writer.WriteLine("using DatReaderWriter.DBObjs;");
                writer.WriteLine("");
                writer.WriteLine("namespace DatReaderWriter {");

                using (writer.IndentScope()) {
                    writer.WriteLine($"public partial class {reader}Database {{");
                    using (writer.IndentScope()) {
                        foreach (var kv in parser.ACDBObjs) {
                            var dataType = kv.Value;
                             if (dataType.Children.Count == 0 || (dataType.Parent as Models.ACDat).Name != reader.ToLower()) continue;
                            
                             if (dataType.FirstId != 0 && dataType.FirstId == dataType.LastId) {
                                WriteSummary(writer, "The " + dataType.Name + " DBObj in the database. This will always use the cache.");
                                writer.WriteLine($"public {dataType.Name}? {dataType.Name} => GetCached<{dataType.Name}>(0x{dataType.FirstId:X8}u);");
                                writer.WriteLine("");
                            }
                        }
                    }
                    writer.WriteLine("}"); // close Database class
                }
                writer.WriteLine("}"); // close namespace

                spc.AddSource($"DatabaseReaders/{reader}Database.generated.cs", writer.ToString());
             }
        }
    }
}
