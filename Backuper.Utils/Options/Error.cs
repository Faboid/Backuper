namespace Backuper.Utils.Options;

/// <summary>
/// Represents an Error <see cref="IOption{TValue, TError}"/>.
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TError"></typeparam>
public struct Error<TValue, TError> : IOption<TValue, TError> {

    private readonly TError err;

    /// <summary>
    /// Initializes <see cref="Error{TValue, TError}"/> with the given <paramref name="error"/>.
    /// </summary>
    /// <param name="error"></param>
    public Error(TError error) {
        err = error;
    }

    public OptionResult Result() => OptionResult.Error;
    public T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none) => error.Invoke(err);
    public Option<T, TError> Bind<T>(Func<TValue, Option<T, TError>> func) => new Error<T, TError>(err);
    public Task<Option<T, TError>> BindAsync<T>(Func<TValue, Task<Option<T, TError>>> func) => Task.FromResult<Option<T, TError>>(new Error<T, TError>(err));
    public TValue? Or(TValue? def) => def;
    public TError OrError(TError? def) => err;


    public static implicit operator Error<TValue, TError>(TError error) => new(error);

}
