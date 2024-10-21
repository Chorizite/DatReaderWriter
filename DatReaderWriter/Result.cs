using System;
using System.Collections.Generic;
using System.Linq;

namespace ACClientLib.DatReaderWriter {
    public readonly struct Result<T, E> {
        private readonly bool _success;

        /// <summary>
        /// Result
        /// </summary>
        public readonly T? Value;

        /// <summary>
        /// The error, if any
        /// </summary>
        public readonly E? Error;

        /// <summary>
        /// Was the operation successful
        /// </summary>
        public readonly bool Success;

        private Result(T? value, E? error, bool success) {
            Value = value;
            Error = error;
            Success = success;
        }

        /// <summary>
        /// Create a successful result
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Result<T, E> FromSuccess(T value) {
            return new(value, default(E), true);
        }

        /// <summary>
        /// Create a failed result
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static Result<T, E> FromError(E error) {
            return new(default(T), error, false);
        }

        /// <summary>
        /// Create a result
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Result<T, E>(T value) => new(value, default(E), true);

        /// <summary>
        /// Create a failed result
        /// </summary>
        /// <param name="error"></param>
        public static implicit operator Result<T, E>(E error) => new(default(T), error, false);

        /// <summary>
        /// Create a result
        /// </summary>
        /// <param name="result"></param>
        public static implicit operator bool(Result<T, E> result) => result.Success;
    }
}
