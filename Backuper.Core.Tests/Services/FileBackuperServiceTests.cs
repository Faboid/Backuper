using Backuper.Abstractions;
using Backuper.Core.Services;
using Moq;

namespace Backuper.Core.Tests.Services;

public class FileBackuperServiceTests {

    [Fact]
    public async Task CallsCopyToWithCorrectArguments() {

        //arrange
        
        string sourceName = "SomeFile.rar";
        var versionPath = Path.Combine(Directory.GetCurrentDirectory(), "VersionDirectory");
        var fileMock = new Mock<IFileInfo>();
        fileMock.Setup(x => x.Name).Returns(sourceName);
        var sut = new FileBackuperService(fileMock.Object);

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
        var sut = new FileBackuperService(fileMock.Object);

        //act
        var actual = sut.GetSourceLastWriteTimeUTC();

        //assert
        Assert.Equal(expected, actual);
        fileMock.Verify(x => x.LastWriteTimeUtc);

    }

}