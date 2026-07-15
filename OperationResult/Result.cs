using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OperationResult.Tags;

namespace OperationResult
{
    /// <summary>
    /// Result of operation (no result type, only Error field)
    /// </summary>
    public readonly struct Result
    {
        private readonly ErrorStack? _error;
        private readonly bool _isOk;
        
        public bool IsOk => _isOk;
        public bool IsError => !_isOk;

        public Result()
        {
            _isOk = true;
        }

        internal Result(ErrorStack stack)
        {
            _isOk = false;
            _error = stack;
        }
        
        public Result Context(string message)
        {
            if (!_isOk) _error!.Context(message);
            return this;
        }
        
        public string GetErrorMessage() => _error?.ToString() ?? string.Empty;

        public static implicit operator bool(Result result)
        {
            return result._isOk;
        }

        public static implicit operator Result(SuccessTag _)
        {
            return new();
        }

        // public static implicit operator Result(ErrorTag _)
        // {
        //     return new(false);
        // }

        public static implicit operator Result(ErrorTag<string> tag)
        {
            return new(tag.Error);
        }

        public static implicit operator Result(Exception e)
        {
            return new(e);
        }

        public static implicit operator Result(ErrorStack e)
        {
            return new(e);
        }

        public static implicit operator Result(List<ErrorStack> e)
        {
            ErrorStack error = e[0];
            error.AttachAll(e[1..]);
            return new(error);
        }

        private Result<T> ConvertError<T>()
        {
            return _error!;
        }

        public Result<T> And<T>(Result<T> other)
        {
            return _isOk && other.IsOk || _isOk ? other : ConvertError<T>();
        }

        public Result<T> AndThen<T>(Func<Result<T>> f)
        {
            return _isOk ? f() : ConvertError<T>();
        }

        public Result Or(Result other)
        {
            return _isOk ? this : other;
        }

        public Result OrElse(Func<ErrorStack, Result> f)
        {
            return _isOk ? this : f(_error!);
        }

        public Result Inspect(Action f)
        {
            if (_isOk) f();
            return this;
        }

        public Result InspectErr(Action<ErrorStack> f)
        {
            if (!_isOk) f(_error!);
            return this;
        }

        /// <summary>
        /// Maps a Result to the typed Result of a closure, leaving an Err value untouched.
        /// </summary>
        public Result<T> Map<T>(Func<T> f)
        {
            return _isOk ? f() : ConvertError<T>();
        }

        /// <summary>
        /// Returns the provided default (if Err), or the result of a closure (if Ok).
        /// </summary>
        public T MapOr<T>(T def, Func<T> f)
        {
            return _isOk ? f() : def;
        }

        /// <summary>
        /// Maps a Result to T by applying fallback function def to a contained Err value, or the result of a closure.
        ///
        /// This function can be used to unpack a successful result while handling an error.
        /// </summary>
        public T MapOrElse<T>(Func<ErrorStack, T> def, Func<T> f)
        {
            return _isOk ? f() : def(_error!);
        }

        /// <summary>
        /// Maps a Result to a T by the result of a closure if the result is Ok, otherwise if Err, returns the default value for the type T.
        /// </summary>
        public T MapOrDefault<T>(Func<T> f) where T : notnull
        {
            return _isOk ? f() : default!;
        }

        /// <summary>
        /// Throws error if this is an error. Only included to match spec.
        /// </summary>
        public void Unwrap()
        {
            if (!_isOk) throw new InvalidOperationException($"Unwrap called on Err value: {_error!}");
        }

        /// <summary>
        /// Does nothing. Only included to match spec.
        /// </summary>
        public void UnwrapOr() { }

        /// <summary>
        /// You probably want InspectErr. This is only included to match spec.
        /// </summary>
        public void UnwrapOrElse(Action<ErrorStack> def)
        {
            if (!_isOk) def(_error!);
        }

        /// <summary>
        /// Does nothing. Only included to match spec.
        /// </summary>
        public void Ok() { }
    }

    /// <summary>
    /// Result of operation (with string stack Error field)
    /// </summary>
    /// <typeparam name="T">Type of Value field</typeparam>
    public readonly struct Result<T>
    {
        private readonly T? _value;
        private readonly ErrorStack? _error;
        private readonly bool _isOk;

        public T? Value => _value;

        public bool IsOk => _isOk;
        public bool IsError => !_isOk;

        internal Result(T? result = default)
        {
            _isOk = true;
            _value = result;
        }

        internal Result(ErrorStack stack)
        {
            _isOk = false;
            _error = stack;
        }

        public Result<T> Context(string message)
        {
            if (!_isOk) _error!.Context(message);
            return this;
        }

        public bool TryGetValue([NotNullWhen(true)] ref T? result, [NotNullWhen(false)] out ErrorStack? stack)
        {
            result = _value;
            stack = _error;
            return _isOk;
        }

        public string GetErrorMessage() => _error?.ToString() ?? string.Empty;

        public static implicit operator bool(Result<T> result)
        {
            return result._isOk;
        }

        public static implicit operator Result<T>(T? result)
        {
            return new Result<T>(result);
        }

        public static implicit operator Result<T>(SuccessTag<T> tag)
        {
            return new Result<T>(tag.Value);
        }

        // public static implicit operator Result<T>(ErrorTag tag)
        // {
        //     return new(false);
        // }

        public static implicit operator Result<T>(ErrorTag<string> tag)
        {
            return new(tag.Error);
        }

        public static implicit operator Result<T>(Exception e)
        {
            return new(e);
        }

        public static implicit operator Result<T>(ErrorTag<Exception> tag)
        {
            return new(tag.Error);
        }

        public static implicit operator Result<T>(ErrorStack e)
        {
            return new(e);
        }

        public static implicit operator Result<T>(List<ErrorStack> e)
        {
            ErrorStack error = e[0];
            error.AttachAll(e[1..]);
            return new(error);
        }

        private Result<U> ConvertError<U>()
        {
            return _error!;
        }

        public Result Convert()
        {
            return !_isOk ? _error! : Helpers.Ok();
        }

        /// <summary>
        /// Returns res if the result is Ok, otherwise returns the Err value of self.
        ///
        /// Arguments passed to And are eagerly evaluated; if you are passing the result of a function call, it is recommended to use AndThen, which is lazily evaluated.
        /// </summary>
        /// <param name="res"></param>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        public Result<U> And<U>(Result<U> res)
        {
            return _isOk && res._isOk || _isOk ? res : ConvertError<U>();
        }

        /// <summary>
        /// Calls f if the result is Ok, otherwise returns the Err value of self.
        ///
        /// This function can be used for control flow based on Result values.
        /// </summary>
        /// <param name="f"></param>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        public Result<U> AndThen<U>(Func<T, Result<U>> f)
        {
            return _isOk ? f(_value!) : ConvertError<U>();
        }

        /// <summary>
        /// Returns res if the result is Err, otherwise returns the Ok value of self.
        ///
        /// Arguments passed to Or are eagerly evaluated; if you are passing the result of a function call, it is recommended to use OrElse, which is lazily evaluated.
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public Result<T> Or(Result<T> res)
        {
            return _isOk ? this : res;
        }

        /// <summary>
        /// Calls f if the result is Err, otherwise returns the Ok value of self.
        ///
        /// This function can be used for control flow based on result values.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public Result<T> OrElse(Func<ErrorStack, Result<T>> f)
        {
            return _isOk ? this : f(_error!);
        }

        /// <summary>
        /// Calls a function with a reference to the contained value if Ok.
        ///
        /// Returns the original result.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public Result<T> Inspect(Action<T> f)
        {
            if (_isOk) f(_value!);
            return this;
        }

        /// <summary>
        /// Calls a function with a reference to the contained value if Err.
        ///
        /// Returns the original result.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public Result<T> InspectErr(Action<ErrorStack> f)
        {
            if (!_isOk) f(_error!);
            return this;
        }

        /// <summary>
        /// Maps a Result of T to Result of U by applying a function to a contained Ok value, leaving an Err value untouched.
        ///
        /// This function can be used to compose the results of two functions.
        /// </summary>
        /// <param name="f"></param>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        public Result<U> Map<U>(Func<T, U> f)
        {
            return _isOk ? f(Value!) : ConvertError<U>();
        }

        /// <summary>
        /// Returns the provided default (if Err), or applies a function to the contained value (if Ok).
        ///
        /// Arguments passed to MapOr are eagerly evaluated; if you are passing the result of a function call, it is recommended to use MapOrElse, which is lazily evaluated.
        /// </summary>
        /// <param name="def"></param>
        /// <param name="f"></param>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        public U MapOr<U>(U def, Func<T, U> f)
        {
            return _isOk ? f(Value!) : def;
        }

        /// <summary>
        /// Maps a Result of T to U by applying fallback function def to a contained Err value, or function f to a contained Ok value.
        ///
        /// This function can be used to unpack a successful result while handling an error.
        /// </summary>
        /// <param name="def"></param>
        /// <param name="f"></param>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        public U MapOrElse<U>(Func<ErrorStack, U> def, Func<T, U> f)
        {
            return _isOk ? f(_value!) : def(_error!);
        }

        /// <summary>
        /// Maps a Result of T to a U by applying function f to the contained value if the result is Ok, otherwise if Err, returns the default value for the type U.
        /// </summary>
        /// <param name="f">The provided function to apply to the contained value.</param>
        /// <typeparam name="U">The type to transform the contained value into.</typeparam>
        /// <returns>The result of applying f to the contained value, or the default value for non-nullable type U</returns>
        public U MapOrDefault<U>(Func<T, U> f) where U : notnull
        {
            return _isOk ? f(_value!) : default!;
        }

        /// <summary>
        /// Returns the contained Ok value
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">If called on a contained Err value</exception>
        public T Unwrap()
        {
            return _isOk ? _value! : throw new InvalidOperationException($"Unwrap called on Err value: {_error!}");
        }

        /// <summary>
        /// Returns the contained Ok value or a provided default.
        /// </summary>
        /// <param name="def">The value to be returned if this is an Err value</param>
        /// <returns></returns>
        public T UnwrapOr(T def)
        {
            return _isOk ? _value! : def;
        }

        /// <summary>
        /// Returns the contained Ok value or computes it from a closure.
        /// </summary>
        /// <param name="def">The closure</param>
        /// <returns></returns>
        public T UnwrapOrElse(Func<ErrorStack, T> def)
        {
            return _isOk ? _value! : def(_error!);
        }

        /// <summary>
        /// If Ok, returns the contained value, otherwise if Err, returns the default value for that type.
        ///
        /// Due to language limitations, this function replaces UnwrapOrDefault, as they would serve the same purpose.
        /// </summary>
        /// <returns></returns>
        public T? Ok()
        {
            return _isOk ? _value : default;
        }
    }

    /// <summary>
    /// Result of operation (with Error field)
    /// </summary>
    /// <typeparam name="T">Type of Value field</typeparam>
    /// <typeparam name="E">Type of Error field</typeparam>
    public readonly struct Result<T, E>
    {
        private readonly bool isSuccess;

        public T? Value { get; }
        public readonly E? Error;

        public bool IsSuccess => isSuccess;
        public bool IsError => !isSuccess;

        private Result(T result)
        {
            isSuccess = true;
            Value = result;
            Error = default;
        }

        private Result(E error)
        {
            isSuccess = false;
            Value = default;
            Error = error;
        }

        public void Deconstruct(out T? result, out E? error)
        {
            result = Value;
            error = Error;
        }

        public static implicit operator bool(Result<T, E> result)
        {
            return result.isSuccess;
        }

        public static implicit operator Result<T, E>(T result)
        {
            return new Result<T, E>(result);
        }

        public static implicit operator Result<T, E>(E error)
        {
            return new Result<T, E>(error);
        }

        public static implicit operator Result<T, E>(SuccessTag<T> tag)
        {
            return new Result<T, E>(tag.Value);
        }

        public static implicit operator Result<T, E>(ErrorTag<E> tag)
        {
            return new Result<T, E>(tag.Error);
        }
    }

    /// <summary>
    /// Result of operation (with different Errors)
    /// </summary>
    /// <typeparam name="T">Type of Value field</typeparam>
    /// <typeparam name="E1">Type of first Error</typeparam>
    /// <typeparam name="E2">Type of second Error</typeparam>
    public readonly struct Result<T, E1, E2>
    {
        private readonly bool isSuccess;

        public readonly T? Value;
        public readonly object? Error;

        public bool IsSuccess => isSuccess;
        public bool IsError => !isSuccess;

        public bool HasError<E>() => Error is E;
        public E? GeE<E>() => (E?)Error;

        private Result(T result)
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

        public void Deconstruct(out T? result, out object? error)
        {
            result = Value;
            error = Error;
        }

        public static implicit operator bool(Result<T, E1, E2> result)
        {
            return result.isSuccess;
        }

        public static implicit operator Result<T, E1, E2>(T result)
        {
            return new Result<T, E1, E2>(result);
        }

        public static implicit operator Result<T, E1, E2>(E1 error)
        {
            return new Result<T, E1, E2>(error);
        }

        public static implicit operator Result<T, E1, E2>(E2 error)
        {
            return new Result<T, E1, E2>(error);
        }

        public static implicit operator Result<T, E1, E2>(SuccessTag<T> tag)
        {
            return new Result<T, E1, E2>(tag.Value);
        }

        public static implicit operator Result<T, E1, E2>(ErrorTag<E1> tag)
        {
            return new Result<T, E1, E2>(tag.Error);
        }

        public static implicit operator Result<T, E1, E2>(ErrorTag<E2> tag)
        {
            return new Result<T, E1, E2>(tag.Error);
        }
    }

    /// <summary>
    /// Result of operation (with different Errors)
    /// </summary>
    /// <typeparam name="T">Type of Value field</typeparam>
    /// <typeparam name="E1">Type of first Error</typeparam>
    /// <typeparam name="E2">Type of second Error</typeparam>
    /// <typeparam name="E3">Type of third Error</typeparam>
    public readonly struct Result<T, E1, E2, E3>
    {
        private readonly bool isSuccess;

        public readonly T? Value;
        public readonly object? Error;

        public bool IsSuccess => isSuccess;
        public bool IsError => !isSuccess;

        public bool HasError<E>() => Error is E;
        public E? GeE<E>() => (E?)Error;

        private Result(T result)
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

        public void Deconstruct(out T? result, out object? error)
        {
            result = Value;
            error = Error;
        }

        public static implicit operator bool(Result<T, E1, E2, E3> result)
        {
            return result.isSuccess;
        }

        public static implicit operator Result<T, E1, E2, E3>(T result)
        {
            return new Result<T, E1, E2, E3>(result);
        }

        public static implicit operator Result<T, E1, E2, E3>(E1 error)
        {
            return new Result<T, E1, E2, E3>(error);
        }

        public static implicit operator Result<T, E1, E2, E3>(E2 error)
        {
            return new Result<T, E1, E2, E3>(error);
        }

        public static implicit operator Result<T, E1, E2, E3>(E3 error)
        {
            return new Result<T, E1, E2, E3>(error);
        }

        public static implicit operator Result<T, E1, E2, E3>(SuccessTag<T> tag)
        {
            return new Result<T, E1, E2, E3>(tag.Value);
        }

        public static implicit operator Result<T, E1, E2, E3>(ErrorTag<E1> tag)
        {
            return new Result<T, E1, E2, E3>(tag.Error);
        }

        public static implicit operator Result<T, E1, E2, E3>(ErrorTag<E2> tag)
        {
            return new Result<T, E1, E2, E3>(tag.Error);
        }

        public static implicit operator Result<T, E1, E2, E3>(ErrorTag<E3> tag)
        {
            return new Result<T, E1, E2, E3>(tag.Error);
        }
    }
}
