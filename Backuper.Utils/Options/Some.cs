using Backuper.Utils;

namespace Backuper.Utils.Options;

/// <summary>
/// Represents a Some <see cref="IOption{TValue}"/>.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public struct Some<TValue> : IOption<TValue> {

    private readonly TValue value;

    /// <summary>
    /// Initializes <see cref="Some{TValue}"/> with the given <paramref name="value"/>.
    /// </summary>
    /// <param name="value"></param>
    public Some(TValue value) {
        this.value = value;
    }

    public OptionResult Result() => OptionResult.Some;
    public T Match<T>(Func<TValue, T> some, Func<T> none) => some.Invoke(value);
    public Option<T> Bind<T>(Func<TValue, Option<T>> func) => func.Invoke(value);
    public async Task<Option<T>> BindAsync<T>(Func<TValue, Task<Option<T>>> func) => await func.Invoke(value);
    public TValue Or(TValue def) => value;


    public static implicit operator Some<TValue>(TValue value) => new(value);

}

/// <summary>
/// Represents a Some <see cref="IOption{TValue, TError}"/>.
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TError"></typeparam>
public struct Some<TValue, TError> : IOption<TValue, TError> {

    private readonly TValue value;

    /// <summary>
    /// Initializes <see cref="Some{TValue, TError}"/> with the given <paramref name="value"/>.
    /// </summary>
    /// <param name="value"></param>
    public Some(TValue value) {
        this.value = value;
    }

    public OptionResult Result() => OptionResult.Some;
    public T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none) => some.Invoke(value);
    public Option<T, TError> Bind<T>(Func<TValue, Option<T, TError>> func) => func.Invoke(value);
    public async Task<Option<T, TError>> BindAsync<T>(Func<TValue, Task<Option<T, TError>>> func) => await func.Invoke(value);
    public TValue Or(TValue def) => value;
    public TError OrError(TError def) => def;


    public static implicit operator Some<TValue, TError>(TValue value) => new(value);

}
