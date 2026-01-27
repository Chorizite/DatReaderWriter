using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DatReaderWriter.SourceGenerator
{
    [Generator]
    public class DatsSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var datsXmlFiles = context.AdditionalTextsProvider
                .Where(file => Path.GetFileName(file.Path).Equals("dats.xml", StringComparison.OrdinalIgnoreCase));

            var datsXmlContents = datsXmlFiles.Select((text, cancellationToken) =>
            {
                var content = text.GetText(cancellationToken)?.ToString();
                return content;
            });

            // We only care if dats.xml changes.
            context.RegisterSourceOutput(datsXmlContents, (spc, content) =>
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "DRW000",
                        "Generator Running",
                        "DatsSourceGenerator is running. Content length: {0}",
                        "SourceGenerator",
                        DiagnosticSeverity.Warning,
                        true),
                    Location.None,
                    content?.Length ?? -1));

                if (string.IsNullOrEmpty(content)) return;

                try
                {
                    var doc = XDocument.Parse(content);
                    var parser = new XMLDefParser(doc);

                    EnumsGenerator.Generate(spc, parser);
                    new TypesGenerator(parser).Generate(spc, parser);
                    new DBObjsGenerator(parser).Generate(spc, parser);
                    new DatabaseReadersGenerator(parser).Generate(spc, parser);
                }
                catch (Exception e)
                {
                    spc.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor(
                            "DRW001",
                            "Error generating source",
                            "Error parsing dats.xml: {0}",
                            "SourceGenerator",
                            DiagnosticSeverity.Error,
                            true),
                        Location.None,
                        e.ToString()));
                }
            });
        }
    }
}
