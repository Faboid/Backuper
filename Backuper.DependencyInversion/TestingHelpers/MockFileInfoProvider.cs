namespace Backuper.DependencyInversion.TestingHelpers;

public class MockFileInfoProvider : IFileInfoProvider {

    private readonly MockFileSystem _mockFileSystem;

    public MockFileInfoProvider(MockFileSystem mockFileSystem) {
        _mockFileSystem = mockFileSystem;
    }

    public IFileInfo FromFilePath(string fullName) => new MockFileInfo(fullName, _mockFileSystem);

}

