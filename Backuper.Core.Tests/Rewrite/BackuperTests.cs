using Backuper.Core.Models;
using Backuper.Core.Rewrite;
using Backuper.Core.Services;
using Backuper.Core.Tests.Mocks;
using Backuper.Core.Validation;
using Backuper.Core.Versioning;
using Moq;

namespace Backuper.Core.Tests.Rewrite;

public class BackuperTests {

    private static readonly string _existingDirectoryPath = Directory.GetCurrentDirectory();

    [Fact]
    public async Task Backup_CallsBackupingMethod() {

        //arrange
        var now = DateTime.UtcNow;
        string newVersionPath = "ANewVersionPath";
        var info = GetGenericData();
        var serviceMock = new Mock<IBackuperService>();
        var versioningMock = new Mock<IBackuperVersioning>();
        versioningMock.Setup(x => x.GenerateNewBackupVersionDirectory()).Returns(newVersionPath);
        versioningMock.Setup(x => x.GetLastBackupTimeUTC()).Returns(now.Subtract(TimeSpan.FromDays(1)));
        serviceMock.Setup(x => x.GetSourceLastWriteTimeUTC()).Returns(now);

        var sut = new Backuper(info, serviceMock.Object, Mock.Of<IBackuperConnection>(), versioningMock.Object, ValidatorMocks.GetAlwaysValid());

        //act
        var actual = await sut.BackupAsync();

        //assert
        Assert.Equal(BackupResponseCode.Success, actual);
        versioningMock.Verify(x => x.GenerateNewBackupVersionDirectory());
        serviceMock.Verify(x => x.BackupAsync(newVersionPath, It.IsAny<CancellationToken>()));
        versioningMock.Verify(x => x.DeleteExtraVersions(info.MaxVersions));

    }
    

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

    [Theory]
    [InlineData(BackuperValid.Unknown, EditBackuperResponseCode.Unknown)]
    [InlineData(BackuperValid.NameHasIllegalCharacters, EditBackuperResponseCode.NameContainsIllegalCharacters)]
    [InlineData(BackuperValid.NameIsEmpty, EditBackuperResponseCode.NewNameIsEmptyOrWhiteSpaces)]
    [InlineData(BackuperValid.ZeroOrNegativeMaxVersions, EditBackuperResponseCode.NewMaxVersionsIsZeroOrNegative)]
    public async Task Edit_ReturnsCorrectErrorCode(BackuperValid fromValidator, EditBackuperResponseCode expected) {

        //arrange
        var validatorMock = new Mock<IBackuperValidator>();

        //set to return valid only during the creation of the object
        validatorMock.Setup(x => x.IsValid(It.IsAny<BackuperInfo>())).Returns(BackuperValid.Valid);
        var sut = new Backuper(GetGenericData(), Mock.Of<IBackuperService>(), Mock.Of<IBackuperConnection>(), Mock.Of<IBackuperVersioning>(), validatorMock.Object);
        validatorMock.Setup(x => x.IsValid(It.IsAny<BackuperInfo>())).Returns(fromValidator);

        //act
        var actual = await sut.EditAsync(GetGenericData());

        //assert
        Assert.Equal(actual, expected);

    }

    [Fact]
    public async Task Edit_ReturnsExistsAlreadyResult() {

        //arrange
        var connectionMock = new Mock<IBackuperConnection>();
        var info = GetGenericData();
        connectionMock.Setup(x => x.Exists(info.Name)).Returns(true);
        var sut = new Backuper(info, Mock.Of<IBackuperService>(), connectionMock.Object, Mock.Of<IBackuperVersioning>(), ValidatorMocks.GetAlwaysValid());

        //act
        var actual = await sut.EditAsync(info);

        //assert
        Assert.Equal(EditBackuperResponseCode.NewNameIsOccupied, actual);

    }

    [Fact]
    public async Task Edit_PrioritizesMakingSureSourceIsUnchanged() {

        //arrange
        var info = GetGenericData();
        var sut = new Backuper(info, Mock.Of<IBackuperService>(), Mock.Of<IBackuperConnection>(), Mock.Of<IBackuperVersioning>(), ValidatorMocks.GetAlwaysValid());

        //By changing this directly, we're also testing if the properties in the class are actually read-only
        //as they should be changed only through the EditAsync method, changing the object that was used as a base
        //should not change the private values
        info.SourcePath = "";

        //act
        var actual = await sut.EditAsync(info);

        //assert
        Assert.Equal(EditBackuperResponseCode.SourceCannotBeChanged, actual);

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
        connectionMock.Verify(x => x.Delete(info.Name));
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