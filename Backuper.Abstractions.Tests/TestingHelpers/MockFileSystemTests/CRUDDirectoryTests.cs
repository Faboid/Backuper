namespace Backuper.Abstractions.Tests.TestingHelpers.MockFileSystemTests;

public class CRUDDirectoryTests {

    [Theory]
    [InlineData("D:\\DirectoryName\\Yo\\sup")]
    [InlineData("D:\\Dir\\SomeFolder\\empty")]
    public void CreateDirectory(string directoryName) {

        //arrange
        var fileSystem = new MockFileSystem();

        //act
        fileSystem.CreateDirectory(directoryName);
        var exists = fileSystem.DirectoryExists(directoryName);

        //assert
        Assert.True(exists);

    }

    [Fact]
    public void DeleteDirectory() {

        //arrange
        var fileSystem = new MockFileSystem();
        string fileName = "D:\\Some\\Folder\\And\\Directory";
        fileSystem.CreateDirectory(fileName);

        //act
        var exist = fileSystem.DirectoryExists(fileName);
        fileSystem.DeleteDirectory(fileName);
        var stillExists = fileSystem.DirectoryExists(fileName);

        //assert
        Assert.True(exist);
        Assert.False(stillExists);

    }

}