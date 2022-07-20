using Backuper.Core.Models;
using Backuper.Core.Saves;
using Backuper.Core.Saves.DBConnections;

namespace Backuper.Core.Tests.Saves.BackuperConnectionTests; 

public class CreateBackuperAsyncTests {

    public CreateBackuperAsyncTests() {
        dbConn = new MemoryDBConnection();
        sut = new(dbConn, mainPath);
    }

    readonly MemoryDBConnection dbConn;
    readonly BackuperConnection sut;
    readonly string mainPath = Path.Combine(Directory.GetCurrentDirectory(), "TestBackupers");
    string GetBackuperPath(string name) => Path.Combine(mainPath, $"{name}.txt");

    [Fact]
    public async Task BackuperCreatedCorrectly() {

        //arrange
        string name = "someName";
        string sourcePath = Directory.GetCurrentDirectory();
        BackuperInfo expected = new(name, sourcePath, 3, false);
        string path = GetBackuperPath(name);

        //act
        await sut.CreateBackuperAsync(expected);
        var result = await dbConn.ReadAllLinesAsync(path);
        var actual = BackuperInfo.Parse(result);

        //assert
        Assert.True(dbConn.Exists(path), "The backuper wasn't created correctly.");
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.SourcePath, actual.SourcePath);
        Assert.Equal(expected.MaxVersions, actual.MaxVersions);
        Assert.Equal(expected.UpdateOnBoot, actual.UpdateOnBoot);

    }

    //todo - implement bad paths

}
