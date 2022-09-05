namespace Backuper.Utils.Options;

/// <summary>
/// Represents a None <see cref="IOption{TValue}"/>.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public struct None<TValue> : IOption<TValue> {

    /// <summary>
    /// Initializes <see cref="None{TValue}"/>.
    /// </summary>
    public None() { }

    public OptionResult Result() => OptionResult.None;
    public T Match<T>(Func<TValue, T> some, Func<T> none) => none.Invoke();
    public Option<T> Bind<T>(Func<TValue, Option<T>> func) => new None<T>();
    public Task<Option<T>> BindAsync<T>(Func<TValue, Task<Option<T>>> func) => Task.FromResult<Option<T>>(new None<T>());
    public TValue? Or(TValue? def) => def;

}

/// <summary>
/// Represents a None <see cref="IOption{TValue, TError}"/>.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public struct None<TValue, TError> : IOption<TValue, TError> {

    /// <summary>
    /// Initializes <see cref="None{TValue, TError}"/>.
    /// <param name="value"></param>
    public None() { }

    public OptionResult Result() => OptionResult.None;
    public T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none) => none.Invoke();
    public Option<T, TError> Bind<T>(Func<TValue, Option<T, TError>> func) => new None<T, TError>();
    public Task<Option<T, TError>> BindAsync<T>(Func<TValue, Task<Option<T, TError>>> func) => Task.FromResult<Option<T, TError>>(new None<T, TError>());
    public TValue? Or(TValue? def) => def;
    public TError? OrError(TError? def) => def;

}
