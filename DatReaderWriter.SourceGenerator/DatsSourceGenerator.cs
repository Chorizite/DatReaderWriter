using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DatReaderWriter.SourceGenerator {
    [Generator]
    public class DatsSourceGenerator : IIncrementalGenerator {
        private void ReportGenError(SourceProductionContext spc, string generatorName, Exception e) {
            spc.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "DRW002",
                    "Error generating source",
                    $"Error in {generatorName}: {{0}}",
                    "SourceGenerator",
                    DiagnosticSeverity.Error,
                    true),
                Location.None,
                e.ToString()));
        }

        public void Initialize(IncrementalGeneratorInitializationContext context) {
            var datsXmlFiles = context.AdditionalTextsProvider
                .Where(file => Path.GetFileName(file.Path).Equals("dats.xml", StringComparison.OrdinalIgnoreCase));

            var datsXmlContents = datsXmlFiles.Select((text, cancellationToken) => {
                var content = text.GetText(cancellationToken)?.ToString();
                return content;
            });

            // We only care if dats.xml changes.
            context.RegisterSourceOutput(datsXmlContents, (spc, content) => {
                spc.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "DRW000",
                        "Generator Running",
                        "DatsSourceGenerator is running. Content length: {0}",
                        "SourceGenerator",
                        DiagnosticSeverity.Info,
                        true),
                    Location.None,
                    content?.Length ?? -1));

                if (string.IsNullOrEmpty(content)) return;

                XMLDefParser parser = null;
                try {
                    var doc = XDocument.Parse(content, LoadOptions.SetLineInfo);
                    parser = new XMLDefParser(doc);
                }
                catch (Exception e) {
                     spc.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor(
                            "DRW001",
                            "Error parsing dats.xml",
                            "Error parsing dats.xml: {0}",
                            "SourceGenerator",
                            DiagnosticSeverity.Error,
                            true),
                        Location.None,
                        e.ToString()));
                    return;
                }

                try {
                    EnumsGenerator.Generate(spc, parser);
                }
                catch (Exception e) { ReportGenError(spc, "EnumsGenerator", e); }

                try {
                    new TypesGenerator(parser).Generate(spc, parser);
                }
                catch (Exception e) { ReportGenError(spc, "TypesGenerator", e); }

                try {
                    new DBObjsGenerator(parser).Generate(spc, parser);
                }
                catch (Exception e) { ReportGenError(spc, "DBObjsGenerator", e); }

                try {
                    new DatabaseReadersGenerator(parser).Generate(spc, parser);
                }
                catch (Exception e) { ReportGenError(spc, "DatabaseReadersGenerator", e); }

                try {
                    new HashTableGenerator(parser).Generate(spc, parser);
                }
                catch (Exception e) { ReportGenError(spc, "HashTableGenerator", e); }
            });
        }
    }
}
