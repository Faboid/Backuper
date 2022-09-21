namespace Backuper.Abstractions;

/// <summary>
/// Provides instance methods for creation and deletion of shortcuts.
/// </summary>
public interface IShortcut {

    /// <summary>
    /// Gets a value indicating whether a shortcut exists.
    /// </summary>
    /// <returns></returns>
    bool Exists();

    /// <summary>
    /// Creates a shortcut.
    /// </summary>
    void Create();

    /// <summary>
    /// Deletes a shortcut.
    /// </summary>
    void Delete();

    /// <summary>
    /// Sets the shortcut's arguments. Must be set before creating.
    /// </summary>
    /// <param name="arguments"></param>
    /// <returns></returns>
    IShortcut SetArguments(string arguments);

    /// <summary>
    /// Sets the shortcut's description. Must be set before creating.
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    IShortcut SetDescription(string description);
}
