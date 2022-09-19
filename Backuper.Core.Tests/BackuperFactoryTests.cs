using Backuper.Abstractions;
using Backuper.Abstractions.TestingHelpers;
using Backuper.Core.Models;
using Backuper.Core.Saves;
using Backuper.Core.Saves.DBConnections;
using Backuper.Core.Services;
using Backuper.Core.Tests.Mocks;
using Backuper.Core.Validation;
using Backuper.Core.Versioning;
using Backuper.Extensions;
using Backuper.Utils;
using Backuper.Utils.Options;
using Moq;
using static Backuper.Core.BackuperFactory;

namespace Backuper.Core.Tests;

public class BackuperFactoryTests {

    public BackuperFactoryTests() {

        _dateTimeProvider = new DateTimeProvider();
        _fileInfoProvider = new MockFileInfoProvider(_fileSystem);
        _directoryInfoProvider = new MockDirectoryInfoProvider(_fileSystem);
        _pathsBuilderService = new PathsBuilderService(new(_directoryInfoProvider, _fileInfoProvider), _dateTimeProvider, _directoryInfoProvider);
        _backuperServiceFactory = new BackuperServiceFactory(_directoryInfoProvider, _fileInfoProvider);
        _backuperVersioningFactory = new BackuperVersioningFactory(_pathsBuilderService, _directoryInfoProvider);
        _fileSystem.CreateDirectory(_existingDirectoryPath);
    }

    private readonly string _existingDirectoryPath = Directory.GetCurrentDirectory();

    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMockFileSystem _fileSystem = new MockFileSystem();
    private readonly IFileInfoProvider _fileInfoProvider;
    private readonly IDirectoryInfoProvider _directoryInfoProvider;
    private readonly IBackuperServiceFactory _backuperServiceFactory;
    private readonly IPathsBuilderService _pathsBuilderService;
    private readonly IBackuperVersioningFactory _backuperVersioningFactory;
    private readonly IBackuperConnection _connection = new BackuperConnection(new MemoryDBConnection());

    [Theory]
    [InlineData(BackuperValid.NameIsEmpty, CreateBackuperFailureCode.NameIsEmpty)]
    [InlineData(BackuperValid.NameHasIllegalCharacters, CreateBackuperFailureCode.NameHasIllegalCharacters)]
    [InlineData(BackuperValid.SourceIsEmpty, CreateBackuperFailureCode.SourceIsEmpty)]
    [InlineData(BackuperValid.SourceDoesNotExist, CreateBackuperFailureCode.SourceDoesNotExist)]
    [InlineData(BackuperValid.ZeroOrNegativeMaxVersions, CreateBackuperFailureCode.ZeroOrNegativeMaxVersions)]
    [InlineData(BackuperValid.Unknown, CreateBackuperFailureCode.Unknown)]
    public async Task ReturnsErrorWhenGivenInvalidValues(BackuperValid invalidResultErrorCode, CreateBackuperFailureCode expected) {

        //arrange
        var mockedValidator = new Mock<IBackuperValidator>();
        mockedValidator.Setup(x => x.IsValid(It.IsAny<BackuperInfo>())).Returns(invalidResultErrorCode);
        var sut = new BackuperFactory(_backuperVersioningFactory, _backuperServiceFactory, _connection, mockedValidator.Object);

        //act
        var actual = await sut.CreateBackuper(new("SomeName", _existingDirectoryPath, 5, false));

        //assert
        Assert.Equal(expected, actual);

    }

    [Fact]
    public async Task NameIsOccupiedResult() {

        //arrange
        var mockedConnection = new Mock<IBackuperConnection>();
        mockedConnection.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
        var sut = new BackuperFactory(_backuperVersioningFactory, _backuperServiceFactory, mockedConnection.Object, ValidatorMocks.GetAlwaysValid());

        //act
        var actual = await sut.CreateBackuper(new("SomeName", _existingDirectoryPath, 5, false));

        //assert
        Assert.Equal(CreateBackuperFailureCode.NameIsOccupied, actual);

    }

    [Fact]
    public async Task SavesBackuperToConnection() {

        //arrange
        var sut = new BackuperFactory(_backuperVersioningFactory, _backuperServiceFactory, _connection, ValidatorMocks.GetAlwaysValid());
        var info = new BackuperInfo("SomeName", _existingDirectoryPath, 3, false);

        //act
        var actual = await sut.CreateBackuper(info);
        var createdResult = await _connection.GetAsync(info.Name);

        //assert
        Assert.Equal(info.Name, createdResult.Name);
        Assert.Equal(info.SourcePath, createdResult.SourcePath);
        Assert.Equal(info.MaxVersions, createdResult.MaxVersions);
        Assert.Equal(info.UpdateOnBoot, createdResult.UpdateOnBoot);

        Assert.Equal(OptionResult.Some, actual.Result());

        var createdBackuper = actual.Or(default)!;
        Assert.NotNull(createdBackuper);
        Assert.Equal(info.Name, createdBackuper.Name);
        Assert.Equal(info.SourcePath, createdBackuper.SourcePath);
        Assert.Equal(info.MaxVersions, createdBackuper.MaxVersions);
        Assert.Equal(info.UpdateOnBoot, createdBackuper.UpdateOnBoot);
    }

    [Fact]
    public async Task CreatesNewVersionsAndServicesPerBackuper() {

        var connectionMock = new Mock<IBackuperConnection>();
        connectionMock.Setup(x => x.GetAllBackupersAsync()).Returns(GroupOptions());
        var versioningFactoryMock = new Mock<IBackuperVersioningFactory>();
        var serviceFactoryMock = new Mock<IBackuperServiceFactory>();
        versioningFactoryMock.Setup(x => x.CreateVersioning(It.IsAny<string>())).Returns<string>(x => _backuperVersioningFactory.CreateVersioning(x));
        serviceFactoryMock.Setup(x => x.CreateBackuperService(It.IsAny<string>())).Returns<string>(x => _backuperServiceFactory.CreateBackuperService(x));

        BackuperFactory sut = new(versioningFactoryMock.Object, serviceFactoryMock.Object, connectionMock.Object, ValidatorMocks.GetAlwaysValid());

        await foreach(var backuper in sut.LoadBackupers()) {

            versioningFactoryMock.Verify(x => x.CreateVersioning(backuper.Name));
            serviceFactoryMock.Verify(x => x.CreateBackuperService(backuper.Name));

        }

    }

    [Fact]
    public async Task CreatesCorruptedWhenGivenNames() {

        var connectionMock = new Mock<IBackuperConnection>();
        var name = "SomeNewUnusedName";
        connectionMock.Setup(x => x.GetAllBackupersAsync()).Returns(GroupOptions(name));
        var sut = InstanceWithMockedConnection(connectionMock.Object);

        await foreach(var backuper in sut.LoadBackupers()) {

            Assert.Equal(name, backuper.Name);
            Assert.Equal("Unknown", backuper.SourcePath);
            Assert.Equal(999, backuper.MaxVersions);

            var result = await backuper.BackupAsync();
            Assert.Equal(BackupResponseCode.Corrupted, result);

        }

    }

    [Fact]
    public async Task ThrowsWhenLoadingOptionNone() {

        var connectionMock = new Mock<IBackuperConnection>();
        connectionMock.Setup(x => x.GetAllBackupersAsync()).Returns(GroupOptions(Option.None<BackuperInfo, string>()));
        var sut = InstanceWithMockedConnection(connectionMock.Object);
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.LoadBackupers().ToListAsync());

    }

    private static IAsyncEnumerable<Option<BackuperInfo, string>> GroupOptions(params Option<BackuperInfo, string>[] options) {
        return options.ToAsyncEnumerable();
    }

    private BackuperFactory InstanceWithMockedConnection(IBackuperConnection connection) 
        => new(_backuperVersioningFactory, _backuperServiceFactory, connection, ValidatorMocks.GetAlwaysValid());

}