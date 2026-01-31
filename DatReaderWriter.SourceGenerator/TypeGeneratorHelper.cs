using System;
using System.Linq;
using DatReaderWriter.SourceGenerator.Models;

namespace DatReaderWriter.SourceGenerator {
    public static class TypeGeneratorHelper {
        public static string SimplifyType(string type) {
            return type switch {
                "ObjectId" or "PackedDWORD" or "LandcellId" or "DataId" => "uint",
                "CompressedUInt" => "uint",
                "DataIdOfKnownType" => "uint",
                "guid" => "Guid",
                _ => type
            };
        }

        public static string GetTypeDeclaration(ACDataMember member, XMLDefParser parser) {
            var simplifiedType = SimplifyType(member.MemberType);
            if (parser.ACTemplatedTypes.ContainsKey(member.MemberType)) {
                simplifiedType = parser.ACTemplatedTypes[member.MemberType].ParentType;
            }

            if (!string.IsNullOrWhiteSpace(member.GenericKey) && !string.IsNullOrWhiteSpace(member.GenericValue))
                return $"{simplifiedType}<{SimplifyType(member.GenericKey)}, {SimplifyType(member.GenericValue)}>";
            if (!string.IsNullOrWhiteSpace(member.GenericType))
                return $"{simplifiedType}<{SimplifyType(member.GenericType)}>";
            return simplifiedType;
        }

        public static string GetTypeDeclaration(ACDataType member) {
            foreach (var child in member.Children) {
                if (child is ACVector) {
                    var memberTypes = child.Children.Where(c => c is ACDataMember)
                        .Select(c => SimplifyType((c as ACDataMember).MemberType)
                        );
                    return member.Name + "<" + string.Join(", ", memberTypes) + ">";
                }
            }

            return member.Name;
        }

        public static string GetTypeDeclaration(ACVector vector, XMLDefParser parser) {
            if (!string.IsNullOrEmpty(vector.GenericKey)) {
                return
                    $"{SimplifyType(vector.Type)}<{SimplifyType(vector.GenericKey)}, {SimplifyType(vector.GenericValue)}>";
            }
            else if (!string.IsNullOrEmpty(vector.GenericValue)) {
                return $"{SimplifyType(vector.Type)}<{SimplifyType(vector.GenericValue)}>";
            }
            else if (vector.Children.Any(c => c is ACDataMember m && string.IsNullOrEmpty(m.Name)) ||
                     vector.Children.Any(c => c is ACVector v && string.IsNullOrEmpty(v.Name))) {
                var types = vector.Children.Select(c => {
                    if (c is ACDataMember m) return GetTypeDeclaration(m, parser);
                    if (c is ACVector v) return GetTypeDeclaration(v, parser);
                    return "object";
                });
                return $"{SimplifyType(vector.Type)}<{string.Join(", ", types)}>";
            }
            else {
                return $"{SimplifyType(vector.Type)}[]";
            }
        }

        public static string GetBinaryReaderForType(string type, string? size = null) {
            return type switch {
                "WORD" or "SpellId" or "ushort" => "reader.ReadUInt16()",
                "short" => "reader.ReadInt16()",
                "DWORD" or "ObjectId" or "LandcellId" or "uint" => "reader.ReadUInt32()",
                "int" => "reader.ReadInt32()",
                "ulong" => "reader.ReadUInt64()",
                "long" => "reader.ReadInt64()",
                "float" => "reader.ReadSingle()",
                "double" => "reader.ReadDouble()",
                "bool" => $"reader.ReadBool({size})",
                "byte" => "reader.ReadByte()",
                "sbyte" => "reader.ReadSByte()",
                "PackedWORD" => "reader.ReadPackedWORD()",
                "Vector3" => "reader.ReadVector3()",
                "Quaternion" => "reader.ReadQuaternion()",
                "DataID" or "DataId" or "PackedDWORD" => "reader.ReadPackedDWORD()",
                "CompressedUInt" => "reader.ReadCompressedUInt()",
                "DataIdOfKnownType" => $"reader.ReadDataIdOfKnownType({size})",
                "guid" => "reader.ReadGuid()",
                _ => $"reader.ReadItem<{type}>()"
            };
        }

        public static string GetBinaryWriterForType(string type) {
            return type switch {
                "WORD" or "SpellId" or "ushort" => "WriteUInt16",
                "short" => "WriteInt16",
                "DWORD" or "ObjectId" or "LandcellId" or "uint" => "WriteUInt32",
                "int" => "WriteInt32",
                "ulong" => "WriteUInt64",
                "long" => "WriteInt64",
                "float" => "WriteSingle",
                "double" => "WriteDouble",
                "bool" => "WriteBool",
                "byte" => "WriteByte",
                "sbyte" => "WriteSByte",
                "Vector3" => "WriteVector3",
                "Quaternion" => "WriteQuaternion",
                "CompressedUInt" => "WriteCompressedUInt",
                "DataIdOfKnownType" => "WriteDataIdOfKnownType",
                "guid" => "WriteGuid",
                _ => $"WriteItem<{type}>"
            };
        }
    }
}
