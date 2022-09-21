namespace Backuper.Core.Saves;

/// <summary>
/// Provides methods to interact with the backuper database.
/// </summary>
public interface IDBConnection {

    /// <summary>
    /// Determines whether the specified backuper exists.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Exists(string name);

    /// <summary>
    /// Enumerates all existing backupers.
    /// </summary>
    /// <returns></returns>
    IEnumerable<string> EnumerateNames();

    /// <summary>
    /// Creates a new backuper save with its string[] representation. If the backuper exists already, it will get overwritten.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="lines"></param>
    /// <returns></returns>
    Task WriteAllLinesAsync(string name, string[] lines);

    /// <summary>
    /// Retrieves a string[] representation of a backuper.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<string[]> ReadAllLinesAsync(string name);

    /// <summary>
    /// Deletes the backuper.
    /// </summary>
    /// <param name="name"></param>
    void Delete(string name);

}

