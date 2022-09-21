namespace Backuper.Abstractions;

/// <summary>
/// Provides properties and instance methods for creation, copying, deletion, moving, and opening of files. Aids in the creation of <see cref="FileStream"/> objects.
/// </summary>
public interface IFileInfo {

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
    /// <inheritdoc cref="FileInfo.Exists"/>
    /// </summary>
    bool Exists { get; }

    /// <summary>
    /// Copies the file asynchronously to <paramref name="path"/>.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    Task CopyToAsync(string path);

    /// <summary>
    /// <inheritdoc cref="FileInfo.Create"/>
    /// </summary>
    void Create();

    /// <summary>
    /// <inheritdoc cref="File.ReadLines(string)"/>
    /// </summary>
    /// <returns></returns>
    IEnumerable<string> ReadLines();

    /// <summary>
    /// <inheritdoc cref="File.WriteAllLines(string, IEnumerable{string})"/>
    /// </summary>
    /// <param name="lines"></param>
    void WriteAllLines(IEnumerable<string> lines);
}
