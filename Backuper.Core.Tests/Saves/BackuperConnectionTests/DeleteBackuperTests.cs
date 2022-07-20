using Backuper.Core.Models;
using Backuper.Core.Saves;
using Backuper.Core.Saves.DBConnections;

namespace Backuper.Core.Tests.Saves.BackuperConnectionTests; 

public class DeleteBackuperTests {

    public DeleteBackuperTests() {
        dbConn = new MemoryDBConnection();
        sut = new(dbConn);
    }

    readonly MemoryDBConnection dbConn;
    readonly BackuperConnection sut;

    [Fact]
    public async Task DeleteBackuperCorrectly() {

        //arrange
        string name = "aName";
        BackuperInfo expected = new(name, Directory.GetCurrentDirectory(), 3, false);
        await dbConn.WriteAllLinesAsync(name, expected.ToStrings());

        //act
        var exists = dbConn.Exists(name);
        var result = sut.DeleteBackuper(name);
        var stillExists = dbConn.Exists(name);

        //assert
        Assert.True(exists);
        Assert.Equal(BackuperConnection.DeleteBackuperCode.Success, result);
        Assert.False(stillExists);

    }

    //todo - implement bad paths tests

}
