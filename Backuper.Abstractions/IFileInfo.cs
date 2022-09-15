namespace Backuper.Abstractions;

public interface IFileInfo {

    /// <summary>
    /// Gets the full path.
    /// </summary>
    string FullName { get; }

    /// <summary>
    /// Gets the name of this <see cref="IDirectoryInfo"/> instance.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the creation time of the element represented by this <see cref="IDirectoryInfo"/>.
    /// </summary>
    DateTime CreationTimeUtc { get; }

    /// <summary>
    /// Gets the time this file was last written to.
    /// </summary>
    DateTime LastWriteTimeUtc { get; }

    /// <summary>
    /// Gets a value indicating whether the directory exists.
    /// </summary>
    bool Exists { get; }

    /// <summary>
    /// Copies the file asynchronously to <paramref name="path"/>.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    Task CopyToAsync(string path);

    /// <summary>
    /// Reads the lines of a file.
    /// </summary>
    /// <returns></returns>
    IEnumerable<string> ReadLines();

    /// <summary>
    /// Creates a new file, writes a collection of strings to the file, and then closes the file.
    /// </summary>
    /// <param name="lines"></param>
    void WriteAllLines(IEnumerable<string> lines);
}
