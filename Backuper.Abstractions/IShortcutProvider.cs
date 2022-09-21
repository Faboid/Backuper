namespace Backuper.Abstractions;

/// <summary>
/// Provides a set of methods to create a <see cref="IShortcut"/>
/// </summary>
public interface IShortcutProvider {

    /// <summary>
    /// Initializes an instance of <see cref="IShortcut"/> on the specified <paramref name="shortcutPath"/> that points at <paramref name="targetPath"/>.
    /// </summary>
    /// <param name="shortcutPath"></param>
    /// <param name="targetPath"></param>
    /// <returns></returns>
    IShortcut FromShortcutPaths(string shortcutPath, string targetPath);
}
