namespace Backuper.Abstractions;

/// <summary>
/// Provides a set of methods to create a <see cref="IFileInfo"/>
/// </summary>
public interface IFileInfoProvider {

    /// <summary>
    /// Initializes an instance of <see cref="IFileInfo"/> on the specified path.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    IFileInfo FromFilePath(string path);
}
