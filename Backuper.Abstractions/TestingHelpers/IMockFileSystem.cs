namespace Backuper.Abstractions.TestingHelpers;

public interface IMockFileSystem {
    void CreateDirectory(string path);
    void CreateDirectory(string path, DateTime creationTime);
    void CreateFile(string path, string[] lines);
    void DeleteDirectory(string path);
    void DeleteFile(string path);
    bool DirectoryExists(string path);
    IEnumerable<IDirectoryInfo> EnumerateDirectories(string path);
    IEnumerable<IDirectoryInfo> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption);
    IEnumerable<IFileInfo> EnumerateFiles(string path);
    IEnumerable<IFileInfo> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);
    bool FileExists(string path);
    string[] ReadFile(string path);
    void Reset();
}