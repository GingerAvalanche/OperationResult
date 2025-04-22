using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using OperationResult.Tags;

namespace OperationResult
{
    /// <summary>
    /// Result of operation (without Error field)
    /// </summary>
    /// <typeparam name="TResult">Type of Value field</typeparam>
    public struct Result<TResult>
    {
        private readonly TResult? value;
        private ErrorStack? error;
        private readonly bool isSuccess;

        public TResult? Value => value;

        public bool IsSuccess => isSuccess;
        public bool IsError => !isSuccess;

        private Result(bool isSuccess, TResult? result = default)
        {
            this.isSuccess = isSuccess;
            value = result;
        }

        public Result<TResult> Context(string message)
        {
            error = error is null ? message : error.Context(message);
            return this;
        }

        public bool TryGetValue([NotNullWhen(true)] ref TResult? result, [NotNullWhen(false)] out ErrorStack? stack)
        {
            result = value;
            stack = error;
            return isSuccess;
        }

        public string GetErrorMessage() => error?.ToString() ?? string.Empty;

        public static implicit operator bool(Result<TResult> result)
        {
            return result.isSuccess;
        }

        public static implicit operator Result<TResult>(TResult? result)
        {
            return new Result<TResult>(true, result);
        }

        public static implicit operator Result<TResult>(SuccessTag<TResult> tag)
        {
            return new Result<TResult>(true, tag.Value);
        }

        public static implicit operator Result<TResult>(ErrorTag tag)
        {
            return new(false);
        }

        public static implicit operator Result<TResult>(ErrorTag<string> tag)
        {
            return new(false)
            {
                error = tag.Error,
            };
        }

        public static implicit operator Result<TResult>(Exception e)
        {
            return new(false)
            {
                error = e,
            };
        }

        public static implicit operator Result<TResult>(ErrorStack e)
        {
            return new(false)
            {
                error = e,
            };
        }

        public static implicit operator Result<TResult>(List<ErrorStack> e)
        {
            ErrorStack error = new();
            error.AttachAll(e);
            return new(false)
            {
                error = error,
            };
        }

        public Result<TResult2> ConvertError<TResult2>()
        {
            return new(false)
            {
                error = error,
            };
        }
    }

    /// <summary>
    /// Result of operation (with Error field)
    /// </summary>
    /// <typeparam name="TResult">Type of Value field</typeparam>
    /// <typeparam name="TError">Type of Error field</typeparam>
    public readonly struct Result<TResult, TError>
    {
        private readonly bool isSuccess;

        public TResult? Value { get; }
        public readonly TError? Error;

        public bool IsSuccess => isSuccess;
        public bool IsError => !isSuccess;

        private Result(TResult result)
        {
            isSuccess = true;
            Value = result;
            Error = default;
        }

        private Result(TError error)
        {
            isSuccess = false;
            Value = default;
            Error = error;
        }

        public void Deconstruct(out TResult? result, out TError? error)
        {
            result = Value;
            error = Error;
        }

        public static implicit operator bool(Result<TResult, TError> result)
        {
            return result.isSuccess;
        }

        public static implicit operator Result<TResult, TError>(TResult result)
        {
            return new Result<TResult, TError>(result);
        }

        public static implicit operator Result<TResult, TError>(TError error)
        {
            return new Result<TResult, TError>(error);
        }

        public static implicit operator Result<TResult, TError>(SuccessTag<TResult> tag)
        {
            return new Result<TResult, TError>(tag.Value);
        }

        public static implicit operator Result<TResult, TError>(ErrorTag<TError> tag)
        {
            return new Result<TResult, TError>(tag.Error);
        }
    }

    /// <summary>
    /// Result of operation (with different Errors)
    /// </summary>
    /// <typeparam name="TResult">Type of Value field</typeparam>
    /// <typeparam name="TError1">Type of first Error</typeparam>
    /// <typeparam name="TError2">Type of second Error</typeparam>
    public readonly struct Result<TResult, TError1, TError2>
    {
        private readonly bool isSuccess;

        public readonly TResult? Value;
        public readonly object? Error;

        public bool IsSuccess => isSuccess;
        public bool IsError => !isSuccess;

        public bool HasError<TError>() => Error is TError;
        public TError? GetError<TError>() => (TError?)Error;

        private Result(TResult result)
        {
            isSuccess = true;
            Value = result;
            Error = null;
        }

        private Result(object error)
        {
            isSuccess = false;
            Value = default;
            Error = error;
        }

        public void Deconstruct(out TResult? result, out object? error)
        {
            result = Value;
            error = Error;
        }

        public static implicit operator bool(Result<TResult, TError1, TError2> result)
        {
            return result.isSuccess;
        }

        public static implicit operator Result<TResult, TError1, TError2>(TResult result)
        {
            return new Result<TResult, TError1, TError2>(result);
        }

        public static implicit operator Result<TResult, TError1, TError2>(TError1 error)
        {
            return new Result<TResult, TError1, TError2>(error);
        }

        public static implicit operator Result<TResult, TError1, TError2>(TError2 error)
        {
            return new Result<TResult, TError1, TError2>(error);
        }

        public static implicit operator Result<TResult, TError1, TError2>(SuccessTag<TResult> tag)
        {
            return new Result<TResult, TError1, TError2>(tag.Value);
        }

        public static implicit operator Result<TResult, TError1, TError2>(ErrorTag<TError1> tag)
        {
            return new Result<TResult, TError1, TError2>(tag.Error);
        }

        public static implicit operator Result<TResult, TError1, TError2>(ErrorTag<TError2> tag)
        {
            return new Result<TResult, TError1, TError2>(tag.Error);
        }
    }

    /// <summary>
    /// Result of operation (with different Errors)
    /// </summary>
    /// <typeparam name="TResult">Type of Value field</typeparam>
    /// <typeparam name="TError1">Type of first Error</typeparam>
    /// <typeparam name="TError2">Type of second Error</typeparam>
    /// <typeparam name="TError3">Type of third Error</typeparam>
    public readonly struct Result<TResult, TError1, TError2, TError3>
    {
        private readonly bool isSuccess;

        public readonly TResult? Value;
        public readonly object? Error;

        public bool IsSuccess => isSuccess;
        public bool IsError => !isSuccess;

        public bool HasError<TError>() => Error is TError;
        public TError? GetError<TError>() => (TError?)Error;

        private Result(TResult result)
        {
            isSuccess = true;
            Value = result;
            Error = null;
        }

        private Result(object error)
        {
            isSuccess = false;
            Value = default;
            Error = error;
        }

        public void Deconstruct(out TResult? result, out object? error)
        {
            result = Value;
            error = Error;
        }

        public static implicit operator bool(Result<TResult, TError1, TError2, TError3> result)
        {
            return result.isSuccess;
        }

        public static implicit operator Result<TResult, TError1, TError2, TError3>(TResult result)
        {
            return new Result<TResult, TError1, TError2, TError3>(result);
        }

        public static implicit operator Result<TResult, TError1, TError2, TError3>(TError1 error)
        {
            return new Result<TResult, TError1, TError2, TError3>(error);
        }

        public static implicit operator Result<TResult, TError1, TError2, TError3>(TError2 error)
        {
            return new Result<TResult, TError1, TError2, TError3>(error);
        }

        public static implicit operator Result<TResult, TError1, TError2, TError3>(TError3 error)
        {
            return new Result<TResult, TError1, TError2, TError3>(error);
        }

        public static implicit operator Result<TResult, TError1, TError2, TError3>(SuccessTag<TResult> tag)
        {
            return new Result<TResult, TError1, TError2, TError3>(tag.Value);
        }

        public static implicit operator Result<TResult, TError1, TError2, TError3>(ErrorTag<TError1> tag)
        {
            return new Result<TResult, TError1, TError2, TError3>(tag.Error);
        }

        public static implicit operator Result<TResult, TError1, TError2, TError3>(ErrorTag<TError2> tag)
        {
            return new Result<TResult, TError1, TError2, TError3>(tag.Error);
        }

        public static implicit operator Result<TResult, TError1, TError2, TError3>(ErrorTag<TError3> tag)
        {
            return new Result<TResult, TError1, TError2, TError3>(tag.Error);
        }
    }
}
