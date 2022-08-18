using Backuper.Extensions;
namespace Backuper.Abstractions.Tests.TestingHelpers.MockFileSystemTests;

public class EnumerateFilesTests {

    public EnumerateFilesTests() {
        _sut = new();
        _sut.CreateDirectory(_directory);
        _filesFullPath = _files.Select(x => Path.Combine(_directory, x)).ToArray();
        _filesFullPath.ForEach(x => _sut.CreateFile(x, Array.Empty<string>()));
    }

    private readonly string _directory = "D:\\Some\\Folder\\And\\Directory";
    private readonly string[] _filesFullPath;
    private readonly string[] _files = new string[] { "FirstFile.txt", "SecondFile.rar", "SomeExecutable.exe", "Nested\\AndFileHere.txt" };
    private readonly MockFileSystem _sut;

    [Fact]
    public void EnumerateTopDirectoryFiles() {

        //arrange
        var expected = _files
            .Where(x => !x.Contains('\\'))
            .Select(x => Path.Combine(_directory, x))
            .ToArray();

        //act
        var noArgumentsActual = _sut
            .EnumerateFiles(_directory)
            .Select(x => x.FullName)
            .ToArray();

        var actual = _sut
            .EnumerateFiles(_directory, "*", SearchOption.TopDirectoryOnly)
            .Select(x => x.FullName)
            .ToArray();

        //assert
        Assert.Equal(expected, noArgumentsActual);
        Assert.Equal(expected, actual);

    }

    [Fact]
    public void EnumerateAllFiles() {

        //act
        var obtainedFiles = _sut
            .EnumerateFiles(_directory, "*", SearchOption.AllDirectories)
            .Select(x => x.FullName)
            .ToArray();

        //assert
        Assert.Equal(_filesFullPath.Length, obtainedFiles.Length);
        Assert.Equal(_filesFullPath, obtainedFiles);

    }

    [Theory]
    [InlineData("*")]
    [InlineData("*.txt")]
    [InlineData("none")]
    public void EnumerateChosenFiles(string searchPattern) {

        //arrange
        var expected = _filesFullPath
            .Where(x => SearchPattern.Match(x, searchPattern))
            .ToArray();

        //act
        var actual = _sut
            .EnumerateFiles(_directory, searchPattern, SearchOption.AllDirectories)
            .Select(x => x.FullName)
            .ToArray();

        //assert
        Assert.Equal(expected, actual);

    }

}