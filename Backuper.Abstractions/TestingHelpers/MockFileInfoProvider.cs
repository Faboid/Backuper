﻿namespace Backuper.Abstractions.TestingHelpers;

public class MockFileInfoProvider : IFileInfoProvider {

    private readonly IMockFileSystem _mockFileSystem;

    public MockFileInfoProvider(IMockFileSystem mockFileSystem) {
        _mockFileSystem = mockFileSystem;
    }

    public IFileInfo FromFilePath(string fullName) => new MockFileInfo(fullName, _mockFileSystem);
    public IFileInfo CreateWithCustomCreationTime(string path, DateTime creationTime) {
        return new MockFileInfo(path, _mockFileSystem, creationTime);
    }

}

