namespace Backuper.Abstractions;

/// <summary>
/// Represents a wrapper for <see cref="DateTime"/>.
/// </summary>
public interface IDateTimeProvider {

    /// <summary>
    /// <inheritdoc cref="DateTime.Now"/>
    /// </summary>
    DateTime Now { get; }
}

public class DateTimeProvider : IDateTimeProvider {
    DateTime IDateTimeProvider.Now => DateTime.Now;

}