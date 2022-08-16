using Backuper.Extensions;
using System.Text.RegularExpressions;
namespace Backuper.DependencyInversion.TestingHelpers;

public class MockFileSystem {

    private readonly Dictionary<string, string[]> _files = new();
    private readonly Dictionary<string, DateTime> _directories = new();

    private readonly MockFileInfoProvider _fileInfoProvider;
    private readonly MockDirectoryInfoProvider _directoryInfoProvider;

    public MockFileSystem() {
        _fileInfoProvider = new(this);
        _directoryInfoProvider = new(this);
    }

    public bool FileExists(string path) {
        return _files.ContainsKey(path);
    }

    public void DeleteFile(string path) {
        _files.Remove(path);
    }

    public void CreateFile(string path, string[] lines) {
        _files.Add(path, lines);
    }

    public string[] ReadFile(string path) {
        return _files[path];
    }

    public IEnumerable<IFileInfo> EnumerateFiles(string path) {
        return _files.Keys
            .Where(x => x.StartsWith(path) && new FileInfo(x).DirectoryName == path)
            .Select(x => _fileInfoProvider.FromFilePath(x));
    }

    public IEnumerable<IFileInfo> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) {

        if(searchOption == SearchOption.TopDirectoryOnly) {
            return EnumerateFiles(path)
                .Where(x => Regex.IsMatch(x.FullName, searchPattern));
        }

        return _files.Keys
            .Where(x => x.StartsWith(path) && Regex.IsMatch(x, searchPattern))
            .Select(x => _fileInfoProvider.FromFilePath(x));
    }

    public void CreateDirectory(string path) => CreateDirectory(path, DateTime.Now);

    public void CreateDirectory(string path, DateTime creationTime) {
        var directories = PathToStack(path);
        foreach(var dir in directories) {
            if(!_directories.ContainsKey(dir.FullName)) {
                _directories.Add(dir.FullName, creationTime);
            }
        }
    }

    public void DeleteDirectory(string path) {
        EnumerateDirectories(path, "*", SearchOption.AllDirectories).ForEach(x => _directories.Remove(x.FullName));
        _directories.Remove(path);
    }

    public IEnumerable<IDirectoryInfo> EnumerateDirectories(string path) {
        if(path[^1] != Path.DirectorySeparatorChar) {
            path += Path.DirectorySeparatorChar;
        }

        var parentName = new DirectoryInfo(path).Name;
        return _directories.Keys
            .Where(x => x.StartsWith(path) && x != path && new DirectoryInfo(x).Parent?.Name == parentName)
            .Select(x => _directoryInfoProvider.CreateWithCustomCreationTime(x, _directories[x]));
    }

    public IEnumerable<IDirectoryInfo> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) {
        if(path[^1] != Path.DirectorySeparatorChar) {
            path += Path.DirectorySeparatorChar;
        }

        if(searchOption == SearchOption.TopDirectoryOnly) {
            return EnumerateDirectories(path)
                .Where(x => Regex.IsMatch(x.FullName, searchPattern));
        }

        return _directories.Keys
            .Where(x => x.StartsWith(path) && x != path && Regex.IsMatch(x, searchPattern))
            .Select(x => _directoryInfoProvider.CreateWithCustomCreationTime(x, _directories[x]));
    }

    public bool DirectoryExists(string path) {
        return _directories.ContainsKey(path);
    }

    private static Stack<DirectoryInfo> PathToStack(string path) {
        Stack<DirectoryInfo> directories = new();

        DirectoryInfo? currInfo = new(path);
        while(currInfo != null) {
            directories.Push(currInfo);
            currInfo = currInfo.Parent;
        }

        return directories;
    }

}