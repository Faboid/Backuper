using Backuper.Core.Models;
using Backuper.Core.Saves;
using Backuper.Core.Saves.DBConnections;

namespace Backuper.Core.Tests.Saves.BackuperConnectionTests; 

public class GetBackuperAsyncTests {

    public GetBackuperAsyncTests() {
        dbConn = new MemoryDBConnection();
        sut = new(dbConn);
    }

    readonly MemoryDBConnection dbConn;
    readonly BackuperConnection sut;

    [Fact]
    public async Task GetBackuperCorrectly() {

        //arrange
        string name = "aName";
        BackuperInfo expected = new(name, Directory.GetCurrentDirectory(), 3, false);
        await dbConn.WriteAllLinesAsync(name, expected.ToStrings());

        //act
        var result = (await sut.GetBackuperAsync(name)).Or(default!);

        //assert
        Assert.Equal(expected.Name, result.Name);
        Assert.Equal(expected.SourcePath, result.SourcePath);
        Assert.Equal(expected.MaxVersions, result.MaxVersions);
        Assert.Equal(expected.UpdateOnBoot, result.UpdateOnBoot);

    }

    //todo - implement bad paths tests

}
