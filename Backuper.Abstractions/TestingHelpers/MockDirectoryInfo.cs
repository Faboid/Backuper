using Backuper.Extensions;
namespace Backuper.Abstractions.TestingHelpers;

public class MockDirectoryInfo : IDirectoryInfo {

    private readonly IMockFileSystem _fileSystem;
    private readonly DirectoryInfo _info;
    private readonly DateTime _creationTime;

    public MockDirectoryInfo(string path, IMockFileSystem mockFileSystem) : this(path, mockFileSystem, DateTime.Now) { }

    public MockDirectoryInfo(string path, IMockFileSystem mockFileSystem, DateTime creationTime) {
        _info = new DirectoryInfo(path);
        _fileSystem = mockFileSystem;
        _creationTime = creationTime;
    }

    public string FullName => _info.FullName;
    public string Name => _info.Name;
    public DateTime CreationTimeUtc => _creationTime;
    public bool Exists => _fileSystem.DirectoryExists(FullName);

    public IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption) {
        return _fileSystem.EnumerateDirectories(FullName, searchPattern, searchOption);
    }

    public IEnumerable<IDirectoryInfo> EnumerateDirectories() {
        return _fileSystem.EnumerateDirectories(FullName);
    }

    public IEnumerable<IFileInfo> EnumerateFiles() {
        return _fileSystem.EnumerateFiles(FullName);
    }

    public IEnumerable<IFileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption) {
        return _fileSystem.EnumerateFiles(FullName, searchPattern, searchOption);
    }

    public Task CopyToAsync(string path) {
        _fileSystem
            .EnumerateDirectories(FullName, "*", SearchOption.AllDirectories)
            .Select(x => x.FullName.Replace(FullName, path))
            .ToList()
            .ForEach(x => _fileSystem.CreateDirectory(x));

        _fileSystem
            .EnumerateFiles(FullName, "*", SearchOption.AllDirectories)
            .Select(x => (File: x, NewFile: x.FullName.Replace(FullName, path)))
            .ToList()
            .ForEach(x => x.File.CopyToAsync(x.NewFile));

        return Task.CompletedTask;
    }

    public void Delete(bool recursively) {
        _fileSystem.DeleteDirectory(FullName);
    }

}
