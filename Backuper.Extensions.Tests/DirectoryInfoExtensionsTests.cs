namespace Backuper.Extensions.Tests; 

public class DirectoryInfoExtensionsTests {

    [Fact]
    public async Task CopyDirectoryCorrectly() {

        //arrange
        string dirPath = Path.Combine(Directory.GetCurrentDirectory(), "DirectoryInfoExtensionsTestsDirectory");
        DirectoryInfo dirInfo = new DirectoryInfo(dirPath);

        string nestedDir = Path.Combine(dirPath, "Nested");
        string nestedFile = Path.Combine(nestedDir, "SomeFile.txt");
        string nestedFileText = "Test, test";

        string copyDir = Path.Combine(Directory.GetCurrentDirectory(), "DirectoryInfoExtensionsTestsDirectoryCopy");
        string nestedCopyDir = Path.Combine(copyDir, "Nested");
        string nestedFileCopy = Path.Combine(nestedCopyDir, "SomeFile.txt");

        try {

            Directory.CreateDirectory(dirPath);
            Directory.CreateDirectory(nestedDir);
            File.WriteAllText(nestedFile, nestedFileText);

            //act
            await dirInfo.CopyToAsync(copyDir);

            //assert
            Assert.True(Directory.Exists(copyDir));
            Assert.True(Directory.Exists(nestedCopyDir));
            Assert.True(File.Exists(nestedFileCopy));
            Assert.Equal(nestedFileText, File.ReadAllText(nestedFileCopy));

        } finally {

            Directory.Delete(dirPath, true);
            Directory.Delete(copyDir, true);

        }
    
    }

}
