namespace Backuper.DependencyInversion;

public interface IDirectoryInfo {

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
    /// Gets a value indicating whether the directory exists.
    /// </summary>
    bool Exists { get; }

    IEnumerable<IFileInfo> EnumerateFiles();
    IEnumerable<IFileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption);

    IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption);
    IEnumerable<IDirectoryInfo> EnumerateDirectories();

    /// <summary>
    /// Copies the directory and its contents asynchronously to <paramref name="path"/>.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    Task CopyToAsync(string path);
    void Delete(bool recursively);

}