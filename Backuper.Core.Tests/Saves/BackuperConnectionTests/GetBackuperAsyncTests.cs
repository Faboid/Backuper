using Backuper.Core.Models;
using Backuper.Core.Saves;
using Backuper.Core.Saves.DBConnections;

namespace Backuper.Core.Tests.Saves.BackuperConnectionTests; 

public class GetBackuperAsyncTests {

    public GetBackuperAsyncTests() {
        dbConn = new MemoryDBConnection();
        sut = new(dbConn, mainPath);
    }

    readonly MemoryDBConnection dbConn;
    readonly BackuperConnection sut;
    readonly string mainPath = Path.Combine(Directory.GetCurrentDirectory(), "TestBackupers");
    string GetBackuperPath(string name) => Path.Combine(mainPath, $"{name}.txt");

    [Fact]
    public async Task GetBackuperCorrectly() {

        //arrange
        string name = "aName";
        BackuperInfo expected = new(name, Directory.GetCurrentDirectory(), 3, false);
        string path = GetBackuperPath(name);
        await dbConn.WriteAllLinesAsync(path, expected.ToStrings());

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
