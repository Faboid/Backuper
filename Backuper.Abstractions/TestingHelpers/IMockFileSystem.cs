namespace Backuper.Abstractions.TestingHelpers;

public interface IMockFileSystem {

    /// <summary>
    /// <inheritdoc cref="Directory.CreateDirectory(string)"/>
    /// </summary>
    /// <param name="path"></param>
    void CreateDirectory(string path);

    /// <summary>
    /// Creates a directory with a custom creation time. If one exists already, it will be overwritten.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="creationTime"></param>
    void CreateDirectory(string path, DateTime creationTime);

    /// <summary>
    /// <inheritdoc cref="File.WriteAllLines(string, IEnumerable{string})"/>
    /// </summary>
    /// <param name="path"></param>
    /// <param name="lines"></param>
    void CreateFile(string path, string[] lines);

    /// <summary>
    /// Creates a file, write the given lines, and sets a custom time. If the file exists already, it gets overwritten.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="lines"></param>
    /// <param name="customTime"></param>
    void CreateFile(string path, string[] lines, DateTime customTime);

    /// <summary>
    /// Deletes a directory from the specified path.
    /// </summary>
    /// <param name="path"></param>
    void DeleteDirectory(string path);

    /// <summary>
    /// <inheritdoc cref="File.Delete(string)"/>
    /// </summary>
    /// <param name="path"></param>
    void DeleteFile(string path);

    /// <summary>
    /// <inheritdoc cref="Directory.Exists(string?)"/>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    bool DirectoryExists(string path);

    /// <summary>
    /// <inheritdoc cref="Directory.EnumerateDirectories(string)"/>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    IEnumerable<IDirectoryInfo> EnumerateDirectories(string path);

    /// <summary>
    /// <inheritdoc cref="Directory.EnumerateDirectories(string, string, SearchOption)"/>
    /// </summary>
    /// <param name="path"></param>
    /// <param name="searchPattern"></param>
    /// <param name="searchOption"></param>
    /// <returns></returns>
    IEnumerable<IDirectoryInfo> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption);

    /// <summary>
    /// <inheritdoc cref="Directory.EnumerateFiles(string)"/>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    IEnumerable<IFileInfo> EnumerateFiles(string path);

    /// <summary>
    /// <inheritdoc cref="Directory.EnumerateFiles(string, string, SearchOption)"/>
    /// </summary>
    /// <param name="path"></param>
    /// <param name="searchPattern"></param>
    /// <param name="searchOption"></param>
    /// <returns></returns>
    IEnumerable<IFileInfo> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);

    /// <summary>
    /// <inheritdoc cref="File.Exists(string?)"/>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    bool FileExists(string path);

    /// <summary>
    /// <inheritdoc cref="Directory.GetLastWriteTimeUtc(string)"/>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    DateTime? GetDirectoryLastWriteTimeUTC(string path);

    /// <summary>
    /// <inheritdoc cref="File.ReadLines(string)"/> Throws <see cref="KeyNotFoundException"/> if the file doesn't exist.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    string[] ReadFile(string path);
    
    /// <summary>
    /// Empties the file system of all directories and files.
    /// </summary>
    void Reset();
}