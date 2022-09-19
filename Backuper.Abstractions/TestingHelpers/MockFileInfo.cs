namespace Backuper.Abstractions.TestingHelpers;

public class MockFileInfo : IFileInfo {

    private readonly IMockFileSystem _mockFileSystem;
    private readonly DateTime _customTime;

    public string FullName { get; init; }
    public string Name { get; init; }

    public MockFileInfo(string fullName, IMockFileSystem mockFileSystem) : this(fullName, mockFileSystem, DateTime.Now) { }
    public MockFileInfo(string fullName, IMockFileSystem mockFileSystem, DateTime customTime) {
        FullName = fullName;
        Name = new FileInfo(fullName).Name;
        _mockFileSystem = mockFileSystem;
        _customTime = customTime;
    }

    public bool Exists => _mockFileSystem.FileExists(FullName);
    public DateTime CreationTimeUtc => _customTime;
    public DateTime LastWriteTimeUtc => _customTime;

    public string[] ReadAllLines() => _mockFileSystem.ReadFile(FullName);
    public void WriteAllLines(string[] lines) => _mockFileSystem.CreateFile(FullName, lines);
    public void Delete() => _mockFileSystem.DeleteFile(FullName);

    public Task CopyToAsync(string path) {
        _mockFileSystem.CreateFile(path, ReadAllLines());
        return Task.CompletedTask;
    }

    public IEnumerable<string> ReadLines() => ReadAllLines();
    public void WriteAllLines(IEnumerable<string> lines) => _mockFileSystem.CreateFile(FullName, lines.ToArray());
    public void Create() => _mockFileSystem.CreateFile(FullName, Array.Empty<string>());
}

