namespace Backuper.Extensions.Tests; 

public class FileInfoExtensionsTests {

    [Fact]
    public async Task CopyFileCorrectly() {

        //arrange
        string sourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), "testFile.txt");
        string fileText = "This is the file's content. It must b e copied correctly.";
        FileInfo fileInfo = new FileInfo(sourceFilePath);
        string newPath = Path.Combine(Directory.GetCurrentDirectory(), "copyTestFile.txt");
        
        File.WriteAllText(sourceFilePath, fileText);

        //act
        await fileInfo.CopyToAsync(newPath);

        //assert
        Assert.True(File.Exists(newPath));
        Assert.Equal(fileText, File.ReadAllText(newPath));

    }

}
