namespace Backuper.DependencyInversion;

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
    /// Gets a value indicating whether the directory exists.
    /// </summary>
    bool Exists { get; }

}
