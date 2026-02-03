using System;
using System.Collections.Generic;
using System.Text;
using DatReaderWriter.SourceGenerator.Models;
using Microsoft.CodeAnalysis;

namespace DatReaderWriter.SourceGenerator {
    public class HashTableGenerator : BaseGenerator {
        public HashTableGenerator(XMLDefParser parser) : base(parser) { }

        public override void Generate(SourceProductionContext spc, XMLDefParser parser) {
            foreach (var kv in parser.ACDataTypes) {
                var dataType = kv.Value;

                // We only care about types that have a generic key (indicating a hash table)
                if (string.IsNullOrEmpty(dataType.GenericKey))
                    continue;

                var writer = new SourceWriter();
                writer.WriteLine("#nullable enable");
                writer.WriteWarningPragmas();

                writer.WriteLine("using System;");
                writer.WriteLine("using System.IO;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using DatReaderWriter.Lib;");
                writer.WriteLine("using DatReaderWriter.Lib.IO;");
                writer.WriteLine("");
                writer.WriteLine("namespace DatReaderWriter.Types {");

                using (writer.IndentScope()) {
                    var keyType = TypeGeneratorHelper.SimplifyType(dataType.GenericKey);
                    var valueType = TypeGeneratorHelper.SimplifyType(dataType.GenericValue);

                    if (XMLDefParser.ACEnums.ContainsKey(dataType.GenericKey))
                        keyType = dataType.GenericKey;

                    if (XMLDefParser.ACEnums.ContainsKey(dataType.GenericValue))
                        valueType = dataType.GenericValue;

                    writer.WriteLine(
                        $"public partial class {dataType.Name}<TKey, TValue> : Dictionary<TKey, TValue>, IUnpackable, IPackable {{");
                    using (writer.IndentScope()) {
                        // Unpack
                        writer.WriteLine("public bool Unpack(DatBinReader reader) {");
                        using (writer.IndentScope()) {
                            writer.WriteLine("var _bucketSizeIndex = reader.ReadByte();");
                            writer.WriteLine("var _numElements = reader.ReadCompressedUInt();");
                            writer.WriteLine("for (var i = 0; i < _numElements; i++) {");
                            using (writer.IndentScope()) {
                                // Read Key
                                if (XMLDefParser.ACEnums.ContainsKey(dataType.GenericKey)) {
                                    var readerMethod =
                                        TypeGeneratorHelper.GetBinaryReaderForType(XMLDefParser
                                            .ACEnums[dataType.GenericKey].ParentType);
                                    writer.WriteLine($"var key = ({dataType.GenericKey})({readerMethod});");
                                }
                                else {
                                    if (keyType == dataType.GenericKey) {
                                        GenerateGenericRead(writer, "TKey", "key");
                                    }
                                    else {
                                        writer.WriteLine(
                                            $"var key = ({dataType.GenericKey})(object)reader.ReadGeneric<TKey>();");
                                    }
                                }

                                // Read Value
                                if (XMLDefParser.ACEnums.ContainsKey(dataType.GenericValue)) {
                                    var readerMethod =
                                        TypeGeneratorHelper.GetBinaryReaderForType(XMLDefParser
                                            .ACEnums[dataType.GenericValue].ParentType);
                                    writer.WriteLine($"var val = ({dataType.GenericValue})({readerMethod});");
                                }
                                else {
                                    if (valueType == dataType.GenericValue) {
                                        GenerateGenericRead(writer, "TValue", "val");
                                    }
                                    else {
                                        writer.WriteLine(
                                            $"var val = ({dataType.GenericValue})(object)reader.ReadGeneric<TValue>();");
                                    }
                                }

                                writer.WriteLine("this.Add((TKey)(object)key, (TValue)(object)val);");
                            }

                            writer.WriteLine("}");
                            writer.WriteLine("return true;");
                        }

                        writer.WriteLine("}"); // End Unpack

                        writer.WriteLine("");

                        // Pack
                        writer.WriteLine("public bool Pack(DatBinWriter writer) {");
                        using (writer.IndentScope()) {
                            if (dataType.IsAutoGrow) {
                                writer.WriteLine(
                                    $"var _bucketSizeIndex = (byte)HashTableHelpers.GetBucketSizeIndex(this.Count, true);");
                            }
                            else {
                                writer.WriteLine(
                                    $"var _bucketSizeIndex = (byte)HashTableHelpers.GetBucketSizeIndex(this.Count, false);");
                            }

                            writer.WriteLine("writer.WriteByte(_bucketSizeIndex);");
                            writer.WriteLine("writer.WriteCompressedUInt((uint)this.Count);");

                            writer.WriteLine("foreach (var kvp in this) {");
                            using (writer.IndentScope()) {
                                // Write Key
                                if (XMLDefParser.ACEnums.ContainsKey(dataType.GenericKey)) {
                                    writer.WriteLine(
                                        $"writer.{TypeGeneratorHelper.GetBinaryWriterForType(XMLDefParser.ACEnums[dataType.GenericKey].ParentType)}(({XMLDefParser.ACEnums[dataType.GenericKey].ParentType})(object)kvp.Key);");
                                }
                                else {
                                    if (keyType == dataType.GenericKey) {
                                        GenerateGenericWrite(writer, "TKey", "kvp.Key");
                                    }
                                    else {
                                        writer.WriteLine($"writer.WriteGeneric<TKey>(kvp.Key);");
                                    }
                                }

                                // Write Value
                                if (XMLDefParser.ACEnums.ContainsKey(dataType.GenericValue)) {
                                    writer.WriteLine(
                                        $"writer.{TypeGeneratorHelper.GetBinaryWriterForType(XMLDefParser.ACEnums[dataType.GenericValue].ParentType)}(({XMLDefParser.ACEnums[dataType.GenericValue].ParentType})(object)kvp.Value);");
                                }
                                else {
                                    if (valueType == dataType.GenericValue) {
                                        GenerateGenericWrite(writer, "TValue", "kvp.Value");
                                    }
                                    else {
                                        writer.WriteLine($"writer.WriteGeneric<TValue>(kvp.Value);");
                                    }
                                }
                            }

                            writer.WriteLine("}");
                            writer.WriteLine("return true;");
                        }


                        writer.WriteLine("}"); // End Pack
                    }

                    writer.WriteLine("}"); // End Class
                }

                writer.WriteLine("}"); // End Namespace

                spc.AddSource($"Types/{dataType.Name}.generated.cs", writer.ToString());
            }
        }
    }
}
