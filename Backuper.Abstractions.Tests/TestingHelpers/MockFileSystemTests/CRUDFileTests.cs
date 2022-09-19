namespace Backuper.Abstractions.Tests.TestingHelpers.MockFileSystemTests;

public class CRUDFileTests {

    [Theory]
    [InlineData("D:\\FileName\\Yo\\sup.txt", "Head", "Body", "Footer")]
    [InlineData("D:\\FileName\\SomeFolder\\empty.txt")]
    public void CreateThenReadFile(string fileName, params string[] data) {

        //arrange
        var fileSystem = new MockFileSystem();

        //act
        fileSystem.CreateFile(fileName, data);
        var createdData = fileSystem.ReadFile(fileName);
        var exists = fileSystem.FileExists(fileName);

        //assert
        Assert.True(exists);
        Assert.Equal(data.Length, createdData.Length);
        Assert.Equal(data, createdData);

    }

    [Fact]
    public void DeleteFile() {

        //arrange
        var fileSystem = new MockFileSystem();
        string fileName = "D:\\Some\\Folder\\And\\File.txt";
        fileSystem.CreateFile(fileName, new string[] { "sup!" });

        //act
        var exist = fileSystem.FileExists(fileName);
        fileSystem.DeleteFile(fileName);
        var stillExists = fileSystem.FileExists(fileName);

        //assert
        Assert.True(exist);
        Assert.False(stillExists);

    }

}