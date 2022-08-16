namespace Backuper.DependencyInversion.TestingHelpers; 

public class MockDirectoryInfoProvider : IDirectoryInfoProvider {

    private readonly MockFileSystem _mockFileSystem;

    public MockDirectoryInfoProvider(MockFileSystem mockFileSystem) {
        _mockFileSystem = mockFileSystem;
    }

    public IDirectoryInfo FromDirectoryPath(string path) {
        return new MockDirectoryInfo(path, _mockFileSystem);
    }

    public IDirectoryInfo CreateWithCustomCreationTime(string path, DateTime creationTime) {
        return new MockDirectoryInfo(path, _mockFileSystem, creationTime);
    }

}
