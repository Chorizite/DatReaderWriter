using System;
using System.Collections.Concurrent;

namespace DatReaderWriter.Lib.IO
{
    /// <summary>
    /// Caches compiled parameterless constructor delegates by type, replacing
    /// Activator.CreateInstance for hot paths. Lives in its own class so that
    /// accessing the cache does not trigger the static constructor of any
    /// consumer type (e.g. DatBinReader's encoding initialization).
    /// </summary>
    internal static class ObjectFactory
    {
        private static readonly ConcurrentDictionary<Type, Func<object>> _cache = new();

        /// <summary>
        /// Get or create a cached parameterless constructor delegate for the given type.
        /// </summary>
        public static Func<object> GetFactory(Type type)
        {
            return _cache.GetOrAdd(type, t =>
            {
                var expr = System.Linq.Expressions.Expression.Lambda<Func<object>>(
                    System.Linq.Expressions.Expression.Convert(
                        System.Linq.Expressions.Expression.New(t),
                        typeof(object)));
                return expr.Compile();
            });
        }

        /// <summary>
        /// Create a new instance of T using a cached factory delegate.
        /// </summary>
        public static T CreateInstance<T>()
        {
            return (T)GetFactory(typeof(T))();
        }
    }
}
