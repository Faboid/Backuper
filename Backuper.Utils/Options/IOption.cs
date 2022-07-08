using Backuper.Utils;

namespace Backuper.Utils.Options;

/// <summary>
/// Represents an option result that could be either Some, Error, or None.
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TError"></typeparam>
internal interface IOption<TValue, TError> {

    /// <summary>
    /// Returns a <see cref="OptionResult"/> to signify what state <see cref="IOption{TValue}"/> is in.
    /// </summary>
    OptionResult Result();

    /// <summary>
    /// Matches which <see cref="Func{TResult}"/> to execute based on whether it's currently Some, Error, or None.
    /// </summary>
    T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none);

    /// <summary>
    /// Executed the given <paramref name="func"/> only if it's currently Some.
    /// </summary>
    Option<T, TError> Bind<T>(Func<TValue, Option<T, TError>> func);

    /// <summary>
    /// Executed asynchronously the given <paramref name="func"/> only if it's currently Some.
    /// </summary>
    Task<Option<T, TError>> BindAsync<T>(Func<TValue, Task<Option<T, TError>>> func);

    /// <summary>
    /// Returns the stored value if Some, otherwise returns <paramref name="def"/>.
    /// </summary>
    TValue Or(TValue def);

    /// <summary>
    /// Returns the stored value if Error, otherwise returns <paramref name="def"/>.
    /// </summary>
    TError OrError(TError def);

}

/// <summary>
/// Represents an option result that could be either Some or None.
/// </summary>
/// <typeparam name="TValue"></typeparam>
internal interface IOption<TValue> {

    /// <summary>
    /// Returns a <see cref="OptionResult"/> to signify what state <see cref="IOption{TValue}"/> is in.
    /// </summary>
    OptionResult Result();

    /// <summary>
    /// Matches which <see cref="Func{TResult}"/> to execute based on whether it's currently Some or None.
    /// </summary>
    T Match<T>(Func<TValue, T> some, Func<T> none);

    /// <summary>
    /// Executed the given <paramref name="func"/> only if it's currently Some.
    /// </summary>
    Option<T> Bind<T>(Func<TValue, Option<T>> func);

    /// <summary>
    /// Executed asynchronously the given <paramref name="func"/> only if it's currently Some.
    /// </summary>
    Task<Option<T>> BindAsync<T>(Func<TValue, Task<Option<T>>> func);

    /// <summary>
    /// Returns the stored value if Some, otherwise returns <paramref name="def"/>.
    /// </summary>
    TValue Or(TValue def);

}

