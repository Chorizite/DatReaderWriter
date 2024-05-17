using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ACClientLib.DatReaderWriter.Tests.Lib {
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class DataValuesAttribute : Attribute {
        public object?[] Values { get; }

        public DataValuesAttribute(params object?[] values) {
            Values = values;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ExcludeCombinationAttribute : Attribute {
        public object?[] Values { get; }

        public ExcludeCombinationAttribute(params object?[] values) {
            Values = values;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CombinatorialDataAttribute : Attribute, ITestDataSource {
        public IEnumerable<object[]> GetData(MethodInfo methodInfo) {
            // Retrieve parameters values
            var values = methodInfo.GetParameters().Select(p => p.GetCustomAttribute<DataValuesAttribute>()?.Values ?? throw new InvalidOperationException("Combinatorial test requires all parameters to have the [DataValues] attribute set")).ToArray();
            var indices = new int[values.Length];

            // Retrieve any excluded combinations
            var excluded = methodInfo.GetCustomAttributes<ExcludeCombinationAttribute>(true);

            // Combine all the values
            while (true) {
                // Create new arguments
                var arg = new object?[indices.Length];
                for (int i = 0; i < indices.Length; i++) {
                    arg[i] = values[i][indices[i]];
                }

                // Check if needs to be excluded
                if (!excluded.Any(e => e.Values.Zip(arg).All(v => v.First?.Equals(v.Second) == true))) {
                    yield return arg!;
                }

                // Increment indices
                for (int i = indices.Length - 1; i >= 0; i--) {
                    indices[i]++;
                    if (indices[i] >= values[i].Length) {
                        indices[i] = 0;

                        if (i == 0)
                            yield break;
                    }
                    else
                        break;
                }
            }
        }

        public string? GetDisplayName(MethodInfo methodInfo, object?[]? data) {
            if (data is not null) {
                var parameters = methodInfo.GetParameters();
                return string.Format(CultureInfo.CurrentCulture, "{0} ({1})", [ methodInfo.Name, string.Join(",", data.Select((d, i) => {
                    return $"{parameters[i].Name}={GetDisplayName(d)}";
                })) ]);
            }

            return null;
        }

        private object GetDisplayName(object? d) {
            if (d is null) return "null";
            if (d is uint du) return $"0x{du:X8}";
            return d.ToString() ?? string.Empty;
        }
    }
}
