using Backuper.Utils;
using Backuper.Utils.Options;

namespace Backuper.TestUtils;

/// <summary>
/// Provides sugar to assert <see cref="Option{TValue}"/> and <see cref="Option{TValue, TError}"/>.
/// </summary>
public static class OptionAsserts {

    /// <summary>
    /// Asserts that <paramref name="option"/>'s <see cref="Option{TValue}.Result"/> equals <paramref name="expected"/>.
    /// </summary>
    /// <typeparam name="TSome"></typeparam>
    /// <param name="option"></param>
    /// <param name="expected"></param>
    public static void Is<TSome>(this Option<TSome> option, OptionResult expected) {
        var actual = option.Result();
        Assert(expected, actual);
    }

    /// <summary>
    /// Asserts that <paramref name="option"/>'s <see cref="Option{TValue, TError}.Result"/> equals <paramref name="expected"/>.
    /// </summary>
    /// <typeparam name="TSome"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <param name="option"></param>
    /// <param name="expected"></param>
    public static void Is<TSome, TError>(this Option<TSome, TError> option, OptionResult expected) {
        var actual = option.Result();
        Assert(expected, actual);
    }

    private static void Assert(OptionResult expected, OptionResult actual) {
        if(actual != expected) {
            throw new WrongResultAssertException($"Expected {expected} result, but it was {actual}.");
        }
    }

}

/// <summary>
/// Custom exception to distinguish the assertion failure to any other exception.
/// </summary>
internal class WrongResultAssertException : Exception {
    public WrongResultAssertException(string? message) : base(message) { }
}