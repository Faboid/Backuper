using Backuper.Core.Models;
using Backuper.Core.Rewrite;
using Backuper.Core.Services;
using Backuper.Core.Tests.Mocks;
using Backuper.Core.Versioning;
using Moq;

namespace Backuper.Core.Tests.Rewrite;

public class BackuperTests {

    private static readonly string _existingDirectoryPath = Directory.GetCurrentDirectory();

    [Fact]
    public async Task Backup_IsAlreadyUpdated() {

        //arrange
        var lastBackupTime = DateTime.Now;
        var sourceLastWriteTime = lastBackupTime.Subtract(TimeSpan.FromDays(1));

        var serviceMock = new Mock<IBackuperService>();
        var versioningMock = new Mock<IBackuperVersioning>();
        versioningMock.Setup(x => x.GetLastBackupTimeUTC()).Returns(lastBackupTime);
        serviceMock.Setup(x => x.GetSourceLastWriteTimeUTC()).Returns(sourceLastWriteTime);

        var sut = new Backuper(GetGenericData(), serviceMock.Object, Mock.Of<IBackuperConnection>(), versioningMock.Object, ValidatorMocks.GetAlwaysValid());

        //act
        var actual = await sut.BackupAsync();

        //assert
        Assert.Equal(BackupResponseCode.AlreadyUpdated, actual);
        serviceMock.Verify(x => x.BackupAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());

    }

    [Fact]
    public async Task Bin_CallsVersioningBinAndDeletesBackuperConnection() {

        //arrange
        var connectionMock = new Mock<IBackuperConnection>();
        var versioningMock = new Mock<IBackuperVersioning>();

        var info = new BackuperInfo("SomeName", "SomePath", 4, false);
        var sut = new Backuper(info, Mock.Of<IBackuperService>(), connectionMock.Object, versioningMock.Object, ValidatorMocks.GetAlwaysValid());

        //act
        await sut.BinAsync();

        //assert
        connectionMock.Verify(x => x.Delete(It.Is<string>(x => x == info.Name)));
        versioningMock.Verify(x => x.Bin());

    }

    private static IEnumerable<object[]> IsUpdatedReturnsTheCorrectResultData() {
        static object[] NewCase(DateTime sourceTime, DateTime lastBackupTime) => new object[] { sourceTime, lastBackupTime };

        var middle = DateTime.Now;

        yield return NewCase(middle.Subtract(TimeSpan.FromHours(2)), middle);
        yield return NewCase(middle, middle.Subtract(TimeSpan.FromHours(2)));
        yield return NewCase(middle, middle);
    }

    [Theory]
    [MemberData(nameof(IsUpdatedReturnsTheCorrectResultData))]
    public void IsUpdatedReturnsTheCorrectResult(DateTime sourceTime, DateTime lastBackupTime) {

        //arrange
        var expected = sourceTime <= lastBackupTime;
        var versioningMock = new Mock<IBackuperVersioning>();
        var serviceMock = new Mock<IBackuperService>();
        versioningMock.Setup(x => x.GetLastBackupTimeUTC()).Returns(lastBackupTime);
        serviceMock.Setup(x => x.GetSourceLastWriteTimeUTC()).Returns(sourceTime);

        var sut = new Backuper(GetGenericData(), serviceMock.Object, Mock.Of<IBackuperConnection>(), versioningMock.Object, ValidatorMocks.GetAlwaysValid());

        //act
        var actual = sut.IsUpdated();

        //assert
        Assert.Equal(expected, actual);

    }

    private static BackuperInfo GetGenericData(string name = "SomeName") {
        return new BackuperInfo(name, Directory.GetCurrentDirectory(), 3, false);
    }

}