using Backuper.Abstractions;
using Backuper.Abstractions.TestingHelpers;
using Backuper.Core.Services;

namespace Backuper.Core.Tests.Services;

public class BackuperServiceFactoryTests {

    public BackuperServiceFactoryTests() {
        _fileSystem = new MockFileSystem();
        _fileInfoProvider = new MockFileInfoProvider(_fileSystem);
        _directoryInfoProvider = new MockDirectoryInfoProvider(_fileSystem);
        _sut = new BackuperServiceFactory(_directoryInfoProvider, _fileInfoProvider);
    }

    private readonly IFileInfoProvider _fileInfoProvider;
    private readonly IDirectoryInfoProvider _directoryInfoProvider;
    private readonly IBackuperServiceFactory _sut;
    private readonly IMockFileSystem _fileSystem;

    [Theory]
    [InlineData("vrwelge")]
    [InlineData("c:\\Hello\\Sup\\rgjrtebt\\text.yep")]
    [InlineData("d:\\Directory\\Does\\Not\\Exist")]
    public void NonExistentSource_ThrowInvalidDataException(string path) {
        Assert.Throws<InvalidDataException>(() => _sut.CreateBackuperService(path));
    }

    [Theory]
    [InlineData("f:\\Somepath\\Directory")]
    public void CreateDirectoryInfo(string path) {

        //arrange
        _fileSystem.CreateDirectory(path);

        //act
        var info = _sut.CreateBackuperService(path);

        //assert
        Assert.NotNull(info);
        Assert.IsType<DirectoryBackuperService>(info);

    }

    [Theory]
    [InlineData("somepath\\hello.txt")]
    public void CreateFileInfo(string path) {

        //arrange
        _fileSystem.CreateFile(path, Array.Empty<string>());

        //act
        var info = _sut.CreateBackuperService(path);

        //assert
        Assert.NotNull(info);
        Assert.IsType<FileBackuperService>(info);

    }

}