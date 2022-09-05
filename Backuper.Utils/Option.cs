using Backuper.Utils.Options;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Backuper.Utils.Tests")]
namespace Backuper.Utils;

/// <summary>
/// Provides methods to instantiate <see cref="Option{TValue}"/> and <see cref="Option{TValue, TError}"/>.
/// </summary>
public static class Option {

    /// <summary>
    /// Instantiates a some <see cref="Option{TValue}"/> with the given <paramref name="value"/>.
    /// </summary>
    public static Option<TValue> Some<TValue>(TValue value) => new(value);
    /// <summary>
    /// Instantiates a none <see cref="Option{TValue}"/>.
    /// </summary>
    public static Option<TValue> None<TValue>() => new();

    /// <summary>
    /// Instantiates a some <see cref="Option{TValue, TError}"/> with the given <paramref name="value"/>.
    /// </summary>
    public static Option<TValue, TError> Some<TValue, TError>(TValue value) => new(value);
    /// <summary>
    /// Instantiates a error <see cref="Option{TValue, TError}"/> with the given <paramref name="error"/>.
    /// </summary>
    public static Option<TValue, TError> Error<TValue, TError>(TError error) => new(error);
    /// <summary>
    /// Instantiates a none <see cref="Option{TValue, TError}"/>.
    /// </summary>
    public static Option<TValue, TError> None<TValue, TError>() => new();

}

/// <summary>
/// Represents a value that can be some or none.
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
public struct Option<TValue> : IOption<TValue> {

    private IOption<TValue> GetOption => _option ?? new None<TValue>();
    private readonly IOption<TValue> _option;

    /// <summary>
    /// Instantiates Some when <paramref name="value"/> is not null, and None when it is.
    /// </summary>
    /// <param name="value"></param>
    public Option(TValue value) {
        _option = value != null ?
            new Some<TValue>(value) :
            new None<TValue>();
    }

    /// <summary>
    /// Instantiates a None option.
    /// </summary>
    public Option() {
        _option = new None<TValue>();
    }

    private Option(IOption<TValue> option) {
        _option = option;
    }

    public OptionResult Result() => GetOption.Result();
    public T Match<T>(Func<TValue, T> some, Func<T> none) => GetOption.Match(some, none);
    public Option<T> Bind<T>(Func<TValue, Option<T>> func) => GetOption.Bind(func);
    public async Task<Option<T>> BindAsync<T>(Func<TValue, Task<Option<T>>> func) => await GetOption.BindAsync(func).ConfigureAwait(false);
    public TValue? Or(TValue? def) => GetOption.Or(def);

    //static constructors
    public static Option<TValue> Some(TValue value) => new(value);
    public static Option<TValue> None() => new();

    //implicit operators
    public static implicit operator Option<TValue>(Some<TValue> some) => new(some);
    public static implicit operator Option<TValue>(None<TValue> none) => new(none);
    public static implicit operator Option<TValue>(TValue value) => new(value);
    //default implicits to None<>

}

/// <summary>
/// Represents a value that can be some, error, or none.
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
/// <typeparam name="TError">The error type.</typeparam>
public struct Option<TValue, TError> : IOption<TValue, TError> {

    private IOption<TValue, TError> GetOption => _option ?? new None<TValue, TError>();
    private readonly IOption<TValue, TError> _option;

    /// <summary>
    /// Instantiates Some when <paramref name="value"/> is not null, and None when it is.
    /// </summary>
    /// <param name="value"></param>
    public Option(TValue value) {
        _option = value != null ?
            new Some<TValue, TError>(value) :
            new None<TValue, TError>();
    }

    /// <summary>
    /// Instantiates Error when <paramref name="error"/> is not null, and None when it is.
    /// </summary>
    /// <param name="error"></param>
    public Option(TError error) {
        _option = error != null ?
            new Error<TValue, TError>(error) :
            new None<TValue, TError>();
    }

    /// <summary>
    /// Instantiates a None option.
    /// </summary>
    public Option() {
        _option = new None<TValue, TError>();
    }

    private Option(IOption<TValue, TError> option) {
        _option = option;
    }

    public OptionResult Result() => GetOption.Result();
    public T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none) => GetOption.Match(some, error, none);
    public Option<T, TError> Bind<T>(Func<TValue, Option<T, TError>> func) => GetOption.Bind(func);
    public async Task<Option<T, TError>> BindAsync<T>(Func<TValue, Task<Option<T, TError>>> func) => await GetOption.BindAsync(func).ConfigureAwait(false);
    public TValue? Or(TValue? def) => GetOption.Or(def);
    public TError? OrError(TError? def) => GetOption.OrError(def);


    //static constructors
    public static Option<TValue, TError> Some(TValue value) => new(value);
    public static Option<TValue, TError> Error(TError error) => new(error);
    public static Option<TValue, TError> None() => new();

    //implicit operators
    public static implicit operator Option<TValue, TError>(Some<TValue, TError> some) => new(some);
    public static implicit operator Option<TValue, TError>(Error<TValue, TError> error) => new(error);
    public static implicit operator Option<TValue, TError>(None<TValue, TError> none) => new(none);
    public static implicit operator Option<TValue, TError>(TValue value) => new(value);
    public static implicit operator Option<TValue, TError>(TError error) => new(error);
    //default implicits to None<>

}