using Backuper.Core.Models;
using Backuper.Core.Saves;
using Backuper.Core.Saves.DBConnections;
using Backuper.Extensions;
using Moq;

namespace Backuper.Core.Tests.Saves;

public class BackuperConnectionTests {

    public BackuperConnectionTests() {
        _dbConnection = new MemoryDBConnection();
        _sut = new BackuperConnection(_dbConnection);
    }

    private readonly IDBConnection _dbConnection;
    private readonly IBackuperConnection _sut;

    [Fact]
    public async Task CreateBackuper() {

        //arrange
        Reset();
        var expected = new BackuperInfo("SomeName", "SourcePathHere", 3, false);

        //act
        await _sut.SaveAsync(expected);
        var actual = await _sut.GetAsync(expected.Name);

        //assert
        Assert.True(_sut.Exists(expected.Name));
        AssertEqualInfo(expected, actual);

    }

    [Fact]
    public async Task OverWriteBackuper() {

        //arrange
        Reset();
        string oldName = "Name";
        await _sut.SaveAsync(new(oldName, "Path", 2, false));
        var expected = new BackuperInfo("NewName", "NewPath", 50, true);

        //act
        await _sut.OverwriteAsync(oldName, expected);
        var actual = await _sut.GetAsync(expected.Name);

        //assert
        Assert.False(_sut.Exists(oldName));
        Assert.True(_sut.Exists(expected.Name));
        AssertEqualInfo(expected, actual);

    }

    [Fact]
    public async Task DeleteBackuper() {

        //arrange
        Reset();
        string name = "SomeValueUsedAsName";
        var info = new BackuperInfo(name, "path", 2, false);
        await _sut.SaveAsync(info);

        //act
        _sut.Delete(name);

        //assert
        Assert.False(_sut.Exists(name));

    }

    private static IEnumerable<object[]> GetAllBackupersData() {
        static object[] NewTestCase(params BackuperInfo[] backupers) => new object[] { backupers };

        yield return NewTestCase();
        yield return NewTestCase(new("Somename", "pathhere", 2, true), new("AnotherName", "Secondpath", 1, false));
        yield return NewTestCase(new BackuperInfo("Name", "Sourcepath", 50, false));
    }

    [Theory]
    [MemberData(nameof(GetAllBackupersData))]
    public async Task GetAllBackupers(params BackuperInfo[] backupers) {

        //arrange
        Reset();
        backupers.ForEach(x => _sut.SaveAsync(x));

        //act
        var actual = await _sut
            .GetAllBackupersAsync()
            .Select(x => x.Or(null))
            .ToListAsync();

        //assert
        Enumerable
            .Range(0, backupers.Length)
            .ForEach(x => AssertEqualInfo(backupers[x], actual[x]!));
    }

    [Theory]
    [InlineData("SomeName", "SomeName", "D:\\SomePath", "5", "notValidBool")]
    [InlineData("SomeName", "SomeName", "D:\\SomePath", "5")]
    [InlineData("SomeName", "SomeName", "D:\\SomePath", "5", "true", "tooManyLines")]
    [InlineData("SomeName", "SomeName", "D:\\SomePath", "five", "true")]
    public async Task ReturnNameForCorruptedBackupers(string name, params string[] values) {

        //arrange
        var dbMock = new Mock<IDBConnection>();
        var sut = new BackuperConnection(dbMock.Object);

        dbMock.Setup(x => x.EnumerateNames()).Returns(new string[] { name });
        dbMock.Setup(x => x.ReadAllLinesAsync(name)).Returns<string>(x => Task.FromResult(values));

        //act
        var result = (await sut.GetAllBackupersAsync().ToListAsync()).First();

        //assert
        Assert.Equal(name, result);

    }

    private static void AssertEqualInfo(BackuperInfo expected, BackuperInfo actual) {
        Assert.NotNull(actual);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.SourcePath, actual.SourcePath);
        Assert.Equal(expected.MaxVersions, actual.MaxVersions);
        Assert.Equal(expected.UpdateOnBoot, actual.UpdateOnBoot);
    }

    private void Reset() {
        _dbConnection
            .EnumerateNames()
            .ToList()
            .ForEach(x => _dbConnection.Delete(x));
    }

}