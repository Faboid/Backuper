namespace Backuper.Abstractions.Tests.TestingHelpers;

public class SearchPatternTests {

    [Theory]
    [InlineData("sdfkjahjklfse", ".")]
    [InlineData("sdfkjahjklfse", "**")]
    [InlineData("FileName.txt", "Fil?Name*")]
    [InlineData("FilmName.rar", "Fil?Name*.rar")]
    [InlineData("FilmName.rar", "???m???e*")]
    [InlineData("FileWasANameThatWasLovely.rar", "Fil*Name*Lovely*.rar")]
    public void Matches(string path, string searchPattern) 
        => Assert.True(SearchPattern.Match(path, searchPattern));

    [Theory]
    [InlineData("SomeFile.txt", "*.exe")]
    [InlineData("WasFile.exe", "???")]
    [InlineData("SomeLongName", "Long*")]
    [InlineData("SomeLongName", "*Long")]
    public void DoesNotMatch(string path, string searchPattern) 
        => Assert.False(SearchPattern.Match(path, searchPattern));

}