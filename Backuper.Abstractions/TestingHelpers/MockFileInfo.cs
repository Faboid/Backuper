namespace Backuper.Abstractions.TestingHelpers;

public class MockFileInfo : IFileInfo {

    private readonly IMockFileSystem _mockFileSystem;

    public string FullName { get; init; }
    public string Name { get; init; }

    public MockFileInfo(string fullName, IMockFileSystem mockFileSystem) {
        FullName = fullName;
        Name = new FileInfo(fullName).Name;
        _mockFileSystem = mockFileSystem;
    }

    public bool Exists => _mockFileSystem.FileExists(FullName);
    public DateTime CreationTimeUtc => throw new NotImplementedException("MockFileInfo doesn't implement CreationTimeUtc.");
    public DateTime LastWriteTimeUtc => throw new NotImplementedException("MockFileInfo doesn't implement LastWriteTimeUtc");

    public string[] ReadAllLines() => _mockFileSystem.ReadFile(FullName);
    public void WriteAllLines(string[] lines) => _mockFileSystem.CreateFile(FullName, lines);
    public void Delete() => _mockFileSystem.DeleteFile(FullName);

    public Task CopyToAsync(string path) {
        _mockFileSystem.CreateFile(path, ReadAllLines());
        return Task.CompletedTask;
    }
}

