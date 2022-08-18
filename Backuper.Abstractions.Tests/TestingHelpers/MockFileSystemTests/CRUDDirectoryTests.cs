namespace Backuper.Abstractions.Tests.TestingHelpers.MockFileSystemTests;

public class CRUDDirectoryTests {

    [Theory]
    [InlineData("D:\\DirectoryName\\Yo\\sup")]
    [InlineData("D:\\Dir\\SomeFolder\\empty")]
    public void CreateDirectoryAndParents(string directoryName) {

        //arrange
        var fileSystem = new MockFileSystem();

        List<string> expectedDirectories = new();
        string? curr = directoryName;
        while(curr != null) {
            expectedDirectories.Add(curr);
            curr = new DirectoryInfo(curr).Parent?.FullName;
        }

        //act
        fileSystem.CreateDirectory(directoryName);

        //assert
        Assert.All(expectedDirectories, x => fileSystem.DirectoryExists(x));

    }

    [Fact]
    public void DeleteDirectory() {

        //arrange
        var fileSystem = new MockFileSystem();
        string directoryName = "D:\\Some\\Folder\\And\\Directory";
        fileSystem.CreateDirectory(directoryName);

        //act
        var exist = fileSystem.DirectoryExists(directoryName);
        fileSystem.DeleteDirectory(directoryName);
        var stillExists = fileSystem.DirectoryExists(directoryName);
        var parentDirExists = fileSystem.DirectoryExists(new DirectoryInfo(directoryName).Parent!.FullName);

        //assert
        Assert.True(exist);
        Assert.False(stillExists);
        Assert.True(parentDirExists);

    }

    [Fact]
    public void DeleteDirectoryChildren() {

        //arrange
        var fileSystem = new MockFileSystem();
        string childrenDirectory = "D:\\Some\\Folder\\And\\Directory";
        string fileName = Path.Combine(childrenDirectory, "File.txt");
        string parentDirectory = new DirectoryInfo(childrenDirectory).Parent!.FullName;
        
        fileSystem.CreateDirectory(childrenDirectory);
        fileSystem.CreateFile(fileName, Array.Empty<string>());

        //act
        var exist = fileSystem.DirectoryExists(childrenDirectory);
        fileSystem.DeleteDirectory(parentDirectory);

        //assert
        Assert.True(exist);
        Assert.False(fileSystem.DirectoryExists(childrenDirectory));
        Assert.False(fileSystem.FileExists(fileName));

    }

}