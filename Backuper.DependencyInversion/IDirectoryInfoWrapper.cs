namespace Backuper.DependencyInversion;

public interface IDirectoryInfoWrapper {

    /// <summary>
    /// Gets the full path.
    /// </summary>
    string FullName { get; }

    /// <summary>
    /// Gets the name of this <see cref="IDirectoryInfoWrapper"/> instance.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the creation time of the element represented by this <see cref="IDirectoryInfoWrapper"/>.
    /// </summary>
    DateTime CreationTimeUtc { get; }

    IEnumerable<IDirectoryInfoWrapper> EnumerateDirectories(string searchPattern, SearchOption searchOption);
    IEnumerable<IDirectoryInfoWrapper> EnumerateDirectories();
    Task CopyToAsync(string path);
    void Delete(bool recursively);

}