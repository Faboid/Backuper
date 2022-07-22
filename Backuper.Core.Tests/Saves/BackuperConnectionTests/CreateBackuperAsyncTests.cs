using Backuper.Core.Models;
using Backuper.Core.Saves;
using Backuper.Core.Saves.DBConnections;

namespace Backuper.Core.Tests.Saves.BackuperConnectionTests; 

public class CreateBackuperAsyncTests {

    public CreateBackuperAsyncTests() {
        dbConn = new MemoryDBConnection();
        sut = new(dbConn);
    }

    readonly MemoryDBConnection dbConn;
    readonly BackuperConnection sut;

    [Fact]
    public async Task BackuperCreatedCorrectly() {

        //arrange
        string name = "someName";
        string sourcePath = Directory.GetCurrentDirectory();
        BackuperInfo expected = new(name, sourcePath, 3, false);

        //act
        await sut.CreateBackuperAsync(expected);
        var result = await dbConn.ReadAllLinesAsync(name);
        var actual = BackuperInfo.Parse(result);

        //assert
        Assert.True(dbConn.Exists(name), "The backuper wasn't created correctly.");
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.SourcePath, actual.SourcePath);
        Assert.Equal(expected.MaxVersions, actual.MaxVersions);
        Assert.Equal(expected.UpdateOnBoot, actual.UpdateOnBoot);

    }

    [Fact]
    public async Task HandlesDuplicateBackuperNames() {

        //arrange
        string name = "backuper";
        string sourcePath = Directory.GetCurrentDirectory();
        BackuperInfo infoFirst = new(name, sourcePath, 3, false);
        BackuperInfo infoSecond = new(name, sourcePath, 2, true);

        //act
        await sut.CreateBackuperAsync(infoFirst);
        var result = await sut.CreateBackuperAsync(infoSecond);

        //assert
        Assert.Equal(CreateBackuperCode.BackuperExistsAlready, result);

    }
    
    [Theory]
    [InlineData("   ")]
    [InlineData("")]
    [InlineData(null)]
    public async Task HandlesInvalidNames(string name) {

        var info = new BackuperInfo(name, Directory.GetCurrentDirectory(), 3, false);
        var result = await sut.CreateBackuperAsync(info);
        Assert.Equal(CreateBackuperCode.NameNotValid, result);

    }

}
