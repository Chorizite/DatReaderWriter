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

                writer.WriteLine("#nullable enable");
                writer.WriteLine("using System.Numerics;");
                writer.WriteLine("using System.IO;");
                writer.WriteLine("using System.Linq;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using System.Threading;");
                writer.WriteLine("using System.Threading.Tasks;");
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
                            if ((dataType.Parent as Models.ACDat).Name != reader.ToLower()) continue;

                            if (dataType.FirstId != 0 && dataType.FirstId == dataType.LastId) {
                                WriteSummary(writer,
                                    "The " + dataType.Name + " DBObj in the database. This will always use the cache.");
                                writer.WriteLine(
                                    $"public {dataType.Name}? {dataType.Name} => GetCached<{dataType.Name}>(0x{dataType.FirstId:X8}u);");
                                writer.WriteLine("");
                            }

                            WriteSummary(writer,
                                $"Get a <see cref=\"{dataType.Name}\"/> entirely from the {reader.ToLower()}.dat");
                            writer.WriteLine(
                                $"public {dataType.Name}? Get{dataType.Name}(uint id) => Get<{dataType.Name}>(id);");

                            WriteSummary(writer,
                                $"Get a <see cref=\"{dataType.Name}\"/> entirely from the {reader.ToLower()}.dat asynchronously");
                            writer.WriteLine("#if (NET8_0_OR_GREATER)");
                            writer.WriteLine(
                                $"public ValueTask<{dataType.Name}?> Get{dataType.Name}Async(uint id, CancellationToken ct = default) => GetAsync<{dataType.Name}>(id, ct);");
                            writer.WriteLine("#else");
                            writer.WriteLine(
                                $"public Task<{dataType.Name}?> Get{dataType.Name}Async(uint id, CancellationToken ct = default) => GetAsync<{dataType.Name}>(id, ct);");
                            writer.WriteLine("#endif");
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
