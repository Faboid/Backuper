using Backuper.Abstractions;
using Backuper.Abstractions.TestingHelpers;
using Backuper.Core.Services;
using Backuper.Extensions;
using Moq;

namespace Backuper.Core.Tests.Services;

public class DirectoryBackuperServiceTests {

    public DirectoryBackuperServiceTests() {
        _dateTimeProvider = new DateTimeProvider();
        _fileSystem = new MockFileSystem();
        _directoryProvider = new MockDirectoryInfoProvider(_fileSystem);
        _fileProvider = new MockFileInfoProvider(_fileSystem);
        _sutFactory = new BackuperServiceFactory(_directoryProvider, _fileProvider);
        _pathsBuilderService = new PathsBuilderService(new(_directoryProvider, _fileProvider), _dateTimeProvider, _directoryProvider);

        _sourceDirectoriesFullPath = _directories.Select(x => new MockDirectory(BuildPathFromSource(x.Path), x.LastWriteTime)).ToArray();
        _sourceFilesFullPath = _files.Select(x => new MockFile(BuildPathFromSource(x.Path), x.Text, x.LastWriteTime)).ToArray();
    }

    private static readonly DateTime _baseTime = new(2022, 11, 7);

    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPathsBuilderService _pathsBuilderService;
    private readonly IBackuperServiceFactory _sutFactory;
    private readonly IMockFileSystem _fileSystem;

    private readonly IDirectoryInfoProvider _directoryProvider;
    private readonly IFileInfoProvider _fileProvider;

    private readonly string _mainSourceDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SomeSourceDirectory");
    private readonly DateTime _mainSourceDirectoryLastWriteTimeUTC = _baseTime.Subtract(TimeSpan.FromDays(10));

    private readonly MockDirectory[] _sourceDirectoriesFullPath;
    private readonly MockDirectory[] _directories = new MockDirectory[] {
        new("Another", _baseTime.Subtract(TimeSpan.FromHours(3))),
        new("Another\\Directory", _baseTime.Subtract(TimeSpan.FromHours(2)))
    };

    private readonly MockFile[] _sourceFilesFullPath;
    private readonly MockFile[] _files = new MockFile[] {
        new("File.txt", new string[] { "Header", "Body", "Footer" }, _baseTime.Subtract(TimeSpan.FromDays(2))),
        new("Another\\SomeOtherNestedFile.rar", new string[] { "Hello there!" }, _baseTime.Subtract(TimeSpan.FromHours(1)))
    };

    private record MockDirectory(string Path, DateTime LastWriteTime);
    private record MockFile(string Path, string[] Text, DateTime LastWriteTime);

    [Fact]
    public async Task BackupAsyncCallsCopyAsync() {

        //arrange
        var mockDir = new Mock<IDirectoryInfo>();
        var sut = new DirectoryBackuperService(mockDir.Object);
        var newVerPath = _pathsBuilderService.GenerateNewBackupVersionDirectory("SomeBackuper");

        //act
        await sut.BackupAsync(newVerPath);

        //assert
        mockDir.Verify(x => x.CopyToAsync(It.Is<string>(x => x == newVerPath)));
        mockDir.VerifyNoOtherCalls();

    }


    [Fact]
    public void GetCorrectSourceLastWriteTimeUTC() {

        //arrange
        SetUpSource();
        var sut = _sutFactory.CreateBackuperService(_mainSourceDirectory);

        //act
        var actual = sut.GetSourceLastWriteTimeUTC();

        //assert
        Assert.Equal(ExpectedSourceLastWriteTimeUTC(), actual);

    }


    private void SetUpSource() {
        _fileSystem.Reset();
        _fileSystem.CreateDirectory(_mainSourceDirectory, default);
        _sourceDirectoriesFullPath.ForEach(x => _fileSystem.CreateDirectory(x.Path, x.LastWriteTime));
        _sourceFilesFullPath.ForEach(x => _fileSystem.CreateFile(x.Path, x.Text, x.LastWriteTime));
    }

    private DateTime ExpectedSourceLastWriteTimeUTC() {
        var dirTime = _sourceDirectoriesFullPath.Select(x => x.LastWriteTime);
        var fileTime = _sourceFilesFullPath.Select(x => x.LastWriteTime);

        var latest = dirTime
            .Concat(fileTime)
            .Max();

        return new[] { latest, _mainSourceDirectoryLastWriteTimeUTC }.Max();
    }

    private string BuildPathFromSource(string path) => Path.Combine(_mainSourceDirectory, path);

}