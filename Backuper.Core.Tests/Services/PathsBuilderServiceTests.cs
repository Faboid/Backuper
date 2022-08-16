using Backuper.Abstractions;
using Backuper.Core.Services;
using Backuper.Extensions;
using Moq;

namespace Backuper.Core.Tests.Services;

public class PathsBuilderServiceTests {

    private readonly IDateTimeProvider _dateTimeProvider = new DateTimeProvider();
    private readonly IDirectoryInfoProvider _directoryInfoProvider = new DirectoryInfoProvider();

    [Theory]
    [InlineData("ghjrhwk")] //paths doesn't currently check for validity.
    [InlineData(@"D://SomePath")]
    public void GenerateCorrectDirectories(string mainDir) {

        //arrange
        string bcpName = "Heyyo";
        var paths = new PathsBuilderService(mainDir, _dateTimeProvider, _directoryInfoProvider);

        //act
        mainDir = Path.Combine(mainDir, "Backuper");
        var expectedBin = Path.Combine(mainDir, "Bin", bcpName);
        var expectedBackups = Path.Combine(mainDir, "Backups", bcpName);

        //assert
        Assert.Equal(expectedBin, paths.GetBinDirectory(bcpName));
        Assert.Equal(expectedBackups, paths.GetBackuperDirectory(bcpName));

    }

    [Fact]
    public void GenerateParseableVersionName() {

        //arrange
        string mainDir = Directory.GetCurrentDirectory();
        var name = "SomeName";
        var paths = new PathsBuilderService(mainDir, _dateTimeProvider, _directoryInfoProvider);
        var backupsDirectory = paths.GetBackuperDirectory(name);

        //act
        var version = paths.GenerateNewBackupVersionDirectory(backupsDirectory);
        DateTime parsedTime = paths.VersionNameToDateTime(version);

        //assert
        Assert.StartsWith(paths.GetBackuperDirectory(name), version);
        Assert.NotEqual(default, parsedTime);

    }

    [Fact]
    public void GenerateCorrectVersionNumber() {
        string GetDirName(string path) => new DirectoryInfo(path).Name;
        string GetVersionString(string path) => GetDirName(path)[1..GetDirName(path).IndexOf(']')];
        int GetVersionNumber(string path) => int.Parse(GetVersionString(path));

        //arrange
        string mainDir = Path.Combine(Directory.GetCurrentDirectory(), "Versions");
        string backuperName = "nameHere";

        try {
            PathsBuilderService paths = new(mainDir, _dateTimeProvider, _directoryInfoProvider);

            //act
            var noDirPathResult = paths.GenerateNewBackupVersionDirectory(backuperName);
            var noDirResult = GetVersionNumber(noDirPathResult);
            Directory.CreateDirectory(noDirPathResult);
            var oneDirResult = GetVersionNumber(paths.GenerateNewBackupVersionDirectory(backuperName));

            //this is to skip ten versions and check if the version number functions correctly
            //even with version over one digit.
            _ = Enumerable.Range(0, 10)
                .Select(x => paths.GenerateNewBackupVersionDirectory(backuperName))
                .ForEach(x => Directory.CreateDirectory(x))
                .ToList();

            var overTenDirPath = paths.GenerateNewBackupVersionDirectory(backuperName);
            var overTenDirResult = GetVersionNumber(overTenDirPath);

            Directory.CreateDirectory(overTenDirPath);
            var newPaths = new PathsBuilderService(mainDir, _dateTimeProvider, _directoryInfoProvider);
            var newPathsVersionResult = GetVersionNumber(newPaths.GenerateNewBackupVersionDirectory(backuperName));

            //assert
            Assert.Equal(1, noDirResult);
            Assert.Equal(2, oneDirResult);
            Assert.Equal(12, overTenDirResult);
            Assert.Equal(13, newPathsVersionResult);

        } finally {

            //dispose
            Directory.Delete(mainDir, true);
        }

    }

    [Fact]
    public void GeneratePreciseVersionName() {

        //arrange
        string mainDir = Directory.GetCurrentDirectory();
        DateTime time = DateTime.Now;
        Mock<IDateTimeProvider> mockDateTimeProvider = new();
        mockDateTimeProvider.Setup(x => x.Now).Returns(() => time);
        PathsBuilderService paths = new(mainDir, mockDateTimeProvider.Object, _directoryInfoProvider);
        
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
        PathsBuilderService paths = new(Directory.GetCurrentDirectory(), _dateTimeProvider, _directoryInfoProvider);

        //act
        var versDir = paths.GenerateNewBackupVersionDirectory("someName");
        var isValid = new DirectoryInfo(versDir).Name.Any(x => invalid.Contains(x));

        //assert
        Assert.False(isValid);

    }

}
