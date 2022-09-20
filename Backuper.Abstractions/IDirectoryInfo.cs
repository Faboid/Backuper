namespace Backuper.Abstractions;

/// <summary>
/// Exposes instance methods for creating, moving, and enumerating through directories and subdirectories.
/// </summary>
public interface IDirectoryInfo {

    /// <summary>
    /// <inheritdoc cref="FileSystemInfo.FullName"/>
    /// </summary>
    string FullName { get; }

    /// <summary>
    /// Gets the name of this <see cref="IDirectoryInfo"/> instance.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// <inheritdoc cref="FileSystemInfo.CreationTimeUtc"/>
    /// </summary>
    DateTime CreationTimeUtc { get; }

    /// <summary>
    /// <inheritdoc cref="FileSystemInfo.LastWriteTimeUtc"/>
    /// </summary>
    DateTime LastWriteTimeUtc { get; }

    /// <summary>
    /// <inheritdoc cref="DirectoryInfo.Exists"/>
    /// </summary>
    bool Exists { get; }

    /// <summary>
    /// <inheritdoc cref="DirectoryInfo.EnumerateFiles"/>
    /// </summary>
    IEnumerable<IFileInfo> EnumerateFiles();

    /// <summary>
    /// <inheritdoc cref="DirectoryInfo.EnumerateFiles(string, SearchOption)"/>
    /// </summary>
    IEnumerable<IFileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption);

    /// <summary>
    /// <inheritdoc cref="DirectoryInfo.EnumerateDirectories(string, SearchOption)"/>
    /// </summary>
    IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption);

    /// <summary>
    /// <inheritdoc cref="DirectoryInfo.EnumerateDirectories"/>
    /// </summary>
    IEnumerable<IDirectoryInfo> EnumerateDirectories();

    /// <summary>
    /// Copies the directory and its contents asynchronously to <paramref name="path"/>.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    Task CopyToAsync(string path);

    /// <summary>
    /// <inheritdoc cref="DirectoryInfo.Delete(bool)"/>
    /// </summary>
    void Delete(bool recursively);

    /// <summary>
    /// <inheritdoc cref="DirectoryInfo.Create"/>
    /// </summary>
    void Create();
}