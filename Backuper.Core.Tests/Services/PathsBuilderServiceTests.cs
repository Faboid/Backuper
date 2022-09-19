using Backuper.Abstractions;
using Backuper.Abstractions.TestingHelpers;
using Backuper.Core.Services;
using Backuper.Extensions;
using Moq;

namespace Backuper.Core.Tests.Services;

public class PathsBuilderServiceTests {

    public PathsBuilderServiceTests() {
        var fileSystem = new MockFileSystem();
        _directoryInfoProvider = new MockDirectoryInfoProvider(fileSystem);
        _fileInfoProvider = new MockFileInfoProvider(fileSystem);
        _pathsHandler = new(_directoryInfoProvider, _fileInfoProvider);
    }

    private readonly IDateTimeProvider _dateTimeProvider = new DateTimeProvider();
    private readonly IDirectoryInfoProvider _directoryInfoProvider;
    private readonly IFileInfoProvider _fileInfoProvider;
    private readonly PathsHandler _pathsHandler;

    [Fact]
    public void GenerateCorrectDirectories() {

        //arrange
        string mainDir = _pathsHandler.GetBackupsDirectory();
        string bcpName = "Heyyo";
        var paths = new PathsBuilderService(_pathsHandler, _dateTimeProvider, _directoryInfoProvider);

        //act
        mainDir = Path.Combine(mainDir, "Backuper");
        var expectedBin = Path.Combine(mainDir, "Bin", bcpName);
        var expectedBackups = Path.Combine(mainDir, "Backups", bcpName);

        //assert
        Assert.Equal(expectedBin, paths.GetBinDirectory(bcpName));
        Assert.Equal(expectedBackups, paths.GetBackupsDirectory(bcpName));

    }

    [Fact]
    public void GenerateParseableVersionName() {

        //arrange
        var name = "SomeName";
        var paths = new PathsBuilderService(_pathsHandler, _dateTimeProvider, _directoryInfoProvider);
        var backupsDirectory = paths.GetBackupsDirectory(name);

        //act
        var version = paths.GenerateNewBackupVersionDirectory(backupsDirectory);
        DateTime parsedTime = paths.VersionNameToDateTime(version);

        //assert
        Assert.StartsWith(paths.GetBackupsDirectory(name), version);
        Assert.NotEqual(default, parsedTime);

    }

    [Fact]
    public void GenerateCorrectVersionNumber() {
        string GetDirName(string path) => new DirectoryInfo(path).Name;
        string GetVersionString(string path) => GetDirName(path)[1..GetDirName(path).IndexOf(']')];
        int GetVersionNumber(string path) => int.Parse(GetVersionString(path));

        //arrange
        string backuperName = "nameHere";
        PathsBuilderService paths = new(_pathsHandler, _dateTimeProvider, _directoryInfoProvider);

        //act
        var noDirPathResult = paths.GenerateNewBackupVersionDirectory(backuperName);
        var noDirResult = GetVersionNumber(noDirPathResult);
        _directoryInfoProvider.FromDirectoryPath(noDirPathResult).Create();
        var oneDirResult = GetVersionNumber(paths.GenerateNewBackupVersionDirectory(backuperName));

        //this is to skip ten versions and check if the version number functions correctly
        //even with version over one digit.
        _ = Enumerable.Range(0, 10)
            .Select(x => paths.GenerateNewBackupVersionDirectory(backuperName))
            .Select(x => _directoryInfoProvider.FromDirectoryPath(x))
            .ForEach(x => x.Create());

        var overTenDirPath = paths.GenerateNewBackupVersionDirectory(backuperName);
        var overTenDirResult = GetVersionNumber(overTenDirPath);

        _directoryInfoProvider.FromDirectoryPath(overTenDirPath).Create();
        var newPaths = new PathsBuilderService(_pathsHandler, _dateTimeProvider, _directoryInfoProvider);
        var newPathsVersionResult = GetVersionNumber(newPaths.GenerateNewBackupVersionDirectory(backuperName));

        //assert
        Assert.Equal(1, noDirResult);
        Assert.Equal(2, oneDirResult);
        Assert.Equal(12, overTenDirResult);
        Assert.Equal(13, newPathsVersionResult);

    }

    [Fact]
    public void GeneratePreciseVersionName() {

        //arrange
        DateTime time = DateTime.Now;
        Mock<IDateTimeProvider> mockDateTimeProvider = new();
        mockDateTimeProvider.Setup(x => x.Now).Returns(() => time);
        PathsBuilderService paths = new(_pathsHandler, mockDateTimeProvider.Object, _directoryInfoProvider);

        //act
        var version = paths.GenerateNewBackupVersionDirectory("SomeName");
        DateTime parsedTime = paths.VersionNameToDateTime(version);

        //assert
        Assert.Equal(time, parsedTime, TimeSpan.FromSeconds(1));

    }

    [Fact]
    public void GenerateValidPathName() {

        //arrange
        var invalid = Path.GetInvalidFileNameChars();
        PathsBuilderService paths = new(_pathsHandler, _dateTimeProvider, _directoryInfoProvider);

        //act
        var versDir = paths.GenerateNewBackupVersionDirectory("someName");
        var isValid = new DirectoryInfo(versDir).Name.Any(x => invalid.Contains(x));

        //assert
        Assert.False(isValid);

    }

}
