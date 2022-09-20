namespace Backuper.Abstractions;

/// <summary>
/// Provides a set of methods to create a <see cref="IDirectoryInfo"/>
/// </summary>
public interface IDirectoryInfoProvider {

    /// <summary>
    /// Initializes an instance of <see cref="IDirectoryInfo"/> on the specified path.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    IDirectoryInfo FromDirectoryPath(string path);
}
