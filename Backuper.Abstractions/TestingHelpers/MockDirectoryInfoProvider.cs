namespace Backuper.Abstractions.TestingHelpers;

public class MockDirectoryInfoProvider : IDirectoryInfoProvider {

    private readonly IMockFileSystem _mockFileSystem;

    public MockDirectoryInfoProvider(IMockFileSystem mockFileSystem) {
        _mockFileSystem = mockFileSystem;
    }

    public IDirectoryInfo FromDirectoryPath(string path) {
        return new MockDirectoryInfo(path, _mockFileSystem);
    }

    public IDirectoryInfo CreateWithCustomCreationTime(string path, DateTime creationTime) {
        return new MockDirectoryInfo(path, _mockFileSystem, creationTime);
    }

}
