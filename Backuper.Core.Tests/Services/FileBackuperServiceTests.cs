using Backuper.Abstractions;
using Backuper.Core.Services;
using Moq;

namespace Backuper.Core.Tests.Services;

public class FileBackuperServiceTests {

    [Fact]
    public async Task CreatesVersionDirectoryBeforeBackuping() {

        //arrange
        string sourceName = "SomeFile.rar";
        bool created = false;
        var versionPath = Path.Combine(Directory.GetCurrentDirectory(), "VersionDirectory");
        
        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.Setup(x => x.Create()).Callback(SetCreatedToTrue);
        
        var directoryProviderMock = new Mock<IDirectoryInfoProvider>();
        directoryProviderMock.Setup(x => x.FromDirectoryPath(versionPath)).Returns(directoryInfoMock.Object);
        
        var fileMock = new Mock<IFileInfo>();
        fileMock.Setup(x => x.Name).Returns(sourceName);
        fileMock.Setup(x => x.CopyToAsync(It.IsAny<string>())).Callback(ThrowIfDirectoryHasntBeenCreated);

        var sut = new FileBackuperService(fileMock.Object, directoryProviderMock.Object);

        //act
        await sut.BackupAsync(versionPath);

        //assert
        directoryInfoMock.Verify(x => x.Create());

        //help methods
        void SetCreatedToTrue() => created = true;
        void ThrowIfDirectoryHasntBeenCreated() {
            if(!created) {
                throw new DirectoryNotFoundException("Tried to start the file backup before creating the version folder.");
            }
        }
    }

    [Fact]
    public async Task CallsCopyToWithCorrectArguments() {

        //arrange
        string sourceName = "SomeFile.rar";
        var versionPath = Path.Combine(Directory.GetCurrentDirectory(), "VersionDirectory");
        var directoryProviderMock = new Mock<IDirectoryInfoProvider>();
        directoryProviderMock.Setup(x => x.FromDirectoryPath(versionPath)).Returns(Mock.Of<IDirectoryInfo>());
        var fileMock = new Mock<IFileInfo>();
        fileMock.Setup(x => x.Name).Returns(sourceName);
        var sut = new FileBackuperService(fileMock.Object, directoryProviderMock.Object);

        //act
        await sut.BackupAsync(versionPath);

        //assert
        fileMock.Verify(x => x.Name);
        fileMock.Verify(x => x.CopyToAsync(It.Is<string>(x => x == Path.Combine(versionPath, sourceName))));

    }

    [Fact]
    public void GetFileLastWriteTime() {

        //arrange
        var expected = new DateTime(2000, 2, 2);
        var fileMock = new Mock<IFileInfo>();
        fileMock.Setup(x => x.LastWriteTimeUtc).Returns(expected);
        var sut = new FileBackuperService(fileMock.Object, Mock.Of<IDirectoryInfoProvider>());

        //act
        var actual = sut.GetSourceLastWriteTimeUTC();

        //assert
        Assert.Equal(expected, actual);
        fileMock.Verify(x => x.LastWriteTimeUtc);

    }

}