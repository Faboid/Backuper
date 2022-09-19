using Backuper.Extensions;

namespace Backuper.Abstractions.Tests.TestingHelpers;

public class MockDirectoryInfoTests {

    public MockDirectoryInfoTests() {
        _fullPathDirectories = _directories
            .Select(x => Path.Combine(_mainDir, x))
            .ToList();

        _fullPathFiles = _files
            .Select(x => new File(Path.Combine(_mainDir, x.Path), x.Text))
            .ToList();
    }

    private record File(string Path, params string[] Text);

    private readonly string _mainDir = Directory.GetCurrentDirectory();

    private readonly List<string> _fullPathDirectories;
    private readonly List<string> _directories = new() {
        "Dir", "Dir\\Nested", "Dir\\Nested\\More",
        "Another", "Another\\Here"
    };

    private readonly List<File> _fullPathFiles;
    private readonly List<File> _files = new() {
        new("SomeText.txt", "Hello", "SecondLine"),
        new("Dir\\SomeOtherText.txt"),
        new("Dir\\Nested\\ClickHere.rar"),
        new("Another\\music.wav")
    };

    private IMockFileSystem SetUpFileSystem() {
        var fileSystem = new MockFileSystem();
        _fullPathDirectories.ForEach(x => fileSystem.CreateDirectory(x));
        _fullPathFiles.ForEach(x => fileSystem.CreateFile(x.Path, x.Text));
        return fileSystem;
    }

    [Theory]
    [InlineData("F:\\SomePath")]
    public async Task CopyToAsync_AllGetsCopied(string newPath) {

        //arrange
        var fileSystem = SetUpFileSystem();
        var sut = new MockDirectoryInfo(_mainDir, fileSystem);

        var expectedDirectories = _directories
            .Select(x => Path.Combine(newPath, x));

        var expectedFiles = _files
            .Select(x => new File(Path.Combine(newPath, x.Path), x.Text));

        //act
        await sut.CopyToAsync(newPath);
        var texts = expectedFiles.Select(x => fileSystem.ReadFile(x.Path));

        //assert
        expectedDirectories
            .ForEach(x => Assert.True(fileSystem.DirectoryExists(x), $"Directory {x} didn't exist."));

        expectedFiles
            .ForEach(x => Assert.True(fileSystem.FileExists(x.Path), $"File {x.Path} didn't exist."))
            .ForEach(x => Assert.Equal(x.Text, fileSystem.ReadFile(x.Path)));

    }


}