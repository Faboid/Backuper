using Backuper.Abstractions;
using Backuper.Abstractions.TestingHelpers;
using Backuper.Core.Services;
using Backuper.Core.Versioning;
using Backuper.Extensions;
using Moq;

namespace Backuper.Core.Tests.Versioning; 

public class BackuperVersioningTests {

    public BackuperVersioningTests() {
        _fileSystem = new MockFileSystem();
        _dateTimeProvider = new DateTimeProvider();
        _directoryInfoProvider = new MockDirectoryInfoProvider(_fileSystem);
        _pathsBuilderService = new PathsBuilderService(_mainDirectory, _dateTimeProvider, _directoryInfoProvider);
        _sutFactory = new BackuperVersioningFactory(_pathsBuilderService, _directoryInfoProvider);
    }

    private readonly string _mainDirectory = Path.Combine(Directory.GetCurrentDirectory(), "BackuperVersioningTestsMainDirectory");
    private readonly IBackuperVersioningFactory _sutFactory;
    private readonly IDirectoryInfoProvider _directoryInfoProvider;
    private readonly IPathsBuilderService _pathsBuilderService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMockFileSystem _fileSystem;

    [Fact]
    public async Task BinCorrectly() {

        //arrange
        ResetFileSystem();
        string backuperName = "SomeName";
        var backuperPath = _pathsBuilderService.GetBackuperDirectory(backuperName);
        var lastVerDir = _pathsBuilderService.GenerateNewBackupVersionDirectory(backuperName);
        string filePath = Path.Combine(lastVerDir, "Hello.txt");
        string[] fileText = new string[] { "Header", "Body", "Footer" };
        _fileSystem.CreateDirectory(lastVerDir);
        _fileSystem.CreateFile(filePath, fileText);

        var sut = _sutFactory.CreateVersioning(backuperName);

        var expectedNewFilePath = filePath.Replace("Backups", "Bin");

        //act
        await sut.Bin();

        //assert
        Assert.False(_fileSystem.DirectoryExists(backuperPath));
        Assert.True(_fileSystem.DirectoryExists(lastVerDir.Replace("Backups", "Bin")));
        Assert.True(_fileSystem.FileExists(expectedNewFilePath));
        Assert.Equal(fileText, _fileSystem.ReadFile(expectedNewFilePath));

    }

    [Theory]
    [InlineData("ANewName")]
    [InlineData("SomeOtherNa!me")]
    public async Task MigrateCorrectly(string newName) {

        //arrange
        ResetFileSystem();
        string backuperName = "SomeName";
        var backuperPath = _pathsBuilderService.GetBackuperDirectory(backuperName);
        var lastVerDir = _pathsBuilderService.GenerateNewBackupVersionDirectory(backuperName);
        string filePath = Path.Combine(lastVerDir, "Hello.txt");
        string[] fileText = new string[] { "Header", "Body", "Footer" };
        _fileSystem.CreateDirectory(lastVerDir);
        _fileSystem.CreateFile(filePath, fileText);

        var sut = _sutFactory.CreateVersioning(backuperName);

        var expectedNewFilePath = filePath.Replace(backuperName, newName);

        //act
        await sut.MigrateTo(newName);

        //assert
        Assert.False(_fileSystem.DirectoryExists(backuperPath));
        Assert.True(_fileSystem.DirectoryExists(_pathsBuilderService.GetBackuperDirectory(newName)));
        Assert.True(_fileSystem.DirectoryExists(lastVerDir.Replace(backuperName, newName)));
        Assert.True(_fileSystem.FileExists(expectedNewFilePath));
        Assert.Equal(fileText, _fileSystem.ReadFile(expectedNewFilePath));

    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(12)]
    public void DeleteExtraVersions_DeletesExtra(int maxVersions) {
        int GetCurrentVersions(string name) 
            => _directoryInfoProvider
                .FromDirectoryPath(_pathsBuilderService.GetBackuperDirectory(name))
                .EnumerateDirectories()
                .Count();

        //arrange
        ResetFileSystem();
        var backuperName = "AFittingNameForThis";
        var sut = _sutFactory.CreateVersioning(backuperName);

        Enumerable
            .Range(0, 15)
            .Select(x => _pathsBuilderService.GenerateNewBackupVersionDirectory(backuperName))
            .ForEach(x => _fileSystem.CreateDirectory(x));

        var startingVersiong = GetCurrentVersions(backuperName);

        //act
        sut.DeleteExtraVersions(maxVersions);

        //assert
        Assert.Equal(maxVersions, GetCurrentVersions(backuperName));

    }

    [Theory]
    [InlineData("SomeName")]
    [InlineData("AnotherName")]
    public void GenerateNewVersionWithInterface(string backuperName) {

        //arrange
        Mock<IPathsBuilderService> _mockPathsBuilderService = new();
        _mockPathsBuilderService.Setup(x => x.GetBackuperDirectory(It.IsAny<string>())).Returns<string>(x => x);
        _mockPathsBuilderService.Setup(x => x.GetBinDirectory(It.IsAny<string>())).Returns<string>(x => x);
        BackuperVersioning sut = new(backuperName, _mockPathsBuilderService.Object, _directoryInfoProvider);

        //act
        _ = sut.GenerateNewBackupVersionDirectory();

        //assert
        _mockPathsBuilderService.Verify(x => x.GenerateNewBackupVersionDirectory(It.Is(backuperName, StringComparer.Ordinal)));

    }

    private void ResetFileSystem() {
        _fileSystem.Reset();
        _fileSystem.CreateDirectory(_mainDirectory);
    }
}
