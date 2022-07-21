using Backuper.Core.Models;
using Backuper.Core.Saves;
using Backuper.Core.Saves.DBConnections;

namespace Backuper.Core.Tests.Saves.BackuperConnectionTests; 

public class UpdateBackuperAsyncTests {

    public UpdateBackuperAsyncTests() {
        dbConn = new MemoryDBConnection();
        sut = new(dbConn);
    }

    readonly MemoryDBConnection dbConn;
    readonly BackuperConnection sut;

    [Fact]
    public async Task UpdateBackuperCorrectly() {

        //arrange
        string name = "aName";
        BackuperInfo starting = new(name, Directory.GetCurrentDirectory(), 1, true);
        BackuperInfo expected = new(name, Directory.GetCurrentDirectory(), 3, false);
        await dbConn.WriteAllLinesAsync(name, starting.ToStrings());

        //act
        var result = await sut.UpdateBackuperAsync(name, "  ", expected.MaxVersions, expected.UpdateOnBoot);
        var actual = BackuperInfo.Parse(await dbConn.ReadAllLinesAsync(name));

        //assert
        Assert.Equal(BackuperConnection.UpdateBackuperCode.Success, result);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.SourcePath, actual.SourcePath);
        Assert.Equal(expected.MaxVersions, actual.MaxVersions);
        Assert.Equal(expected.UpdateOnBoot, actual.UpdateOnBoot);

    }

    [Fact]
    public async Task NotExistentBackuper_ReturnsCorrectCode() {

        var result = await sut.UpdateBackuperAsync("backuper");
        Assert.Equal(BackuperConnection.UpdateBackuperCode.BackuperDoesNotExist, result);
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("")]
    [InlineData(null)]
    public async Task HandlesInvalidNames(string name) {

        var result = await sut.UpdateBackuperAsync(name);
        Assert.Equal(BackuperConnection.UpdateBackuperCode.NameNotValid, result);
    }

}
