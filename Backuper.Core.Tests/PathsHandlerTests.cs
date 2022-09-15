using Backuper.Abstractions;
using Backuper.Abstractions.TestingHelpers;
using Moq;

namespace Backuper.Core.Tests;

public class PathsHandlerTests {

	[Theory]
	[InlineData("")]
	[InlineData(null)]
	[InlineData("SomeNotValidPath")]
	[InlineData("C:\\suy\\here:newpath")]
    public async Task InvalidPaths(string newPath) {

		var sut = GetCleanPathsHandler();
		var result = await sut.SetBackupersDirectoryAsync(newPath);
		Assert.Equal(PathsHandler.BackupersMigrationResult.InvalidPath, result);

	}

	[Fact]
	public void GetDefaultIfTheSettingsFileIsEmpty() {

		//arrange
		var sut = GetCleanPathsHandler();

		//assert
		Assert.Equal(DefaultPaths.BackupsDirectory, sut.GetBackupsDirectory());
		Assert.Equal(DefaultPaths.BackupersDirectory, sut.GetBackupersDirectory());

	}

	[Fact]
	public async Task EditBackupersPath() {

		//arrange
		var sut = GetCleanPathsHandler();
		var newPath = NewValidPath;

		//act
		var result = await sut.SetBackupersDirectoryAsync(newPath);
		var actualNewPath = sut.GetBackupersDirectory();
		var resetResult = await sut.ResetBackupersDirectory();

        //assert
        Assert.Equal(PathsHandler.BackupersMigrationResult.Success, result);
        Assert.Equal(PathsHandler.BackupersMigrationResult.Success, resetResult);
        Assert.Equal(newPath, actualNewPath);
        Assert.Equal(DefaultPaths.BackupersDirectory, sut.GetBackupersDirectory());

	}

	[Fact]
	public async Task MigrateBackups() {

        //arrange
        var fileSystem = new MockFileSystem();
        var directoryInfoProviderMock = new Mock<IDirectoryInfoProvider>();
		var directoryInfoMock = new Mock<IDirectoryInfo>();
		directoryInfoMock.Setup(x => x.Exists).Returns(false);
        directoryInfoProviderMock.Setup(x => x.FromDirectoryPath(It.IsAny<string>())).Returns(directoryInfoMock.Object);
		var fileInfoProvider = new MockFileInfoProvider(fileSystem);
        var pathsHandler = new PathsHandler(directoryInfoProviderMock.Object, fileInfoProvider);

		//act
		var result = await pathsHandler.SetBackupersDirectoryAsync(NewValidPath);

		//assert
		Assert.Equal(PathsHandler.BackupersMigrationResult.Success, result);
		directoryInfoMock.Verify(x => x.CopyToAsync(NewValidPath));

    }

	[Fact]
	public async Task ErroringWillNotChangeExisting() {

        //arrange
        var fileSystem = new MockFileSystem();
        var directoryInfoProviderMock = new Mock<IDirectoryInfoProvider>();
        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.Setup(x => x.Exists).Returns(false);
		directoryInfoMock.Setup(x => x.CopyToAsync(It.IsAny<string>())).ThrowsAsync(new InvalidOperationException());
        directoryInfoProviderMock.Setup(x => x.FromDirectoryPath(It.IsAny<string>())).Returns(directoryInfoMock.Object);
        var fileInfoProvider = new MockFileInfoProvider(fileSystem);
        var pathsHandler = new PathsHandler(directoryInfoProviderMock.Object, fileInfoProvider);

		//act
		var result = await pathsHandler.SetBackupersDirectoryAsync(NewValidPath);

		//assert
		Assert.Equal(PathsHandler.BackupersMigrationResult.Failure, result);
		Assert.Equal(DefaultPaths.BackupersDirectory, pathsHandler.GetBackupersDirectory());

    }

	private static string NewValidPath => Path.Combine(Directory.GetCurrentDirectory(), "New");

	private static PathsHandler GetCleanPathsHandler() {
		var fileSystem = new MockFileSystem();
		var directoryInfoProvider = new MockDirectoryInfoProvider(fileSystem);
		var fileInfoProvider = new MockFileInfoProvider(fileSystem);
		var pathsHandler = new PathsHandler(directoryInfoProvider, fileInfoProvider);
		return pathsHandler;
	}

}