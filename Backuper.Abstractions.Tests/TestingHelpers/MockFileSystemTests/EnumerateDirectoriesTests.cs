using Backuper.Extensions;
namespace Backuper.Abstractions.Tests.TestingHelpers.MockFileSystemTests;

public class EnumerateDirectoriesTests {

    public EnumerateDirectoriesTests() {
        _sut = new MockFileSystem();
        _sut.CreateDirectory(_mainDirectory);

        _directoriesFullPath = _directories
            .Select(x => Path.Combine(_mainDirectory, x))
            .ToArray();

        _directoriesFullPath
            .ForEach(x => _sut.CreateDirectory(x));
    }

    private readonly string _mainDirectory = "D:\\Some\\Folder\\And\\Directory";
    private readonly string[] _directoriesFullPath;
    private readonly IMockFileSystem _sut;

    private readonly string[] _directories = new string[] { 
        "Dir", "Dir\\SomeNesting", "Dir\\SomeNesting\\Here", 
        "Another", "Another\\Sup", 
        "DirectChild", 
        "AnotherChild" 
    };


    [Fact]
    public void EnumerateTopDirectoryOnly() {

        //arrange
        var expected = _directories
            .Where(x => !x.Contains('\\'))
            .Select(x => Path.Combine(_mainDirectory, x))
            .ToArray();

        //act
        var noArgumentsActual = _sut
            .EnumerateDirectories(_mainDirectory)
            .Select(x => x.FullName)
            .ToArray();

        var actual = _sut
            .EnumerateDirectories(_mainDirectory, "*", SearchOption.TopDirectoryOnly)
            .Select(x => x.FullName)
            .ToArray();

        //assert
        Assert.Equal(expected, noArgumentsActual);
        Assert.Equal(expected, actual);

    }

    [Fact]
    public void EnumerateAllDirectories() {

        //act
        var obtainedFiles = _sut
            .EnumerateDirectories(_mainDirectory, "*", SearchOption.AllDirectories)
            .Select(x => x.FullName)
            .ToArray();

        //assert
        Assert.Equal(_directoriesFullPath.Length, obtainedFiles.Length);
        Assert.Equal(_directoriesFullPath, obtainedFiles);

    }


    [Theory]
    [InlineData("*")]
    [InlineData("*Dir*")]
    [InlineData("none")]
    public void EnumerateChosenDirectories(string searchPattern) {

        //arrange
        var expected = _directoriesFullPath
            .Where(x => SearchPattern.Match(x, searchPattern))
            .ToArray();

        //act
        var actual = _sut
            .EnumerateDirectories(_mainDirectory, searchPattern, SearchOption.AllDirectories)
            .Select(x => x.FullName)
            .ToArray();

        //assert
        Assert.Equal(expected, actual);

    }

}