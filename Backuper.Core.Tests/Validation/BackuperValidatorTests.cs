using Backuper.Core.Validation;

namespace Backuper.Core.Tests.Validation; 

public class BackuperValidatorTests {

    private readonly IBackuperValidator _sut = new BackuperValidator();

    private static object[] InvalidChars() => new object[] { new object[] { new string(Path.GetInvalidFileNameChars()), NameValid.HasIllegalCharacters } };

    [Theory]
    [InlineData("ValidName", NameValid.Valid)]
    [InlineData(null, NameValid.EmptyOrWhiteSpace)]
    [InlineData("", NameValid.EmptyOrWhiteSpace)]
    [InlineData("     ", NameValid.EmptyOrWhiteSpace)]
    [MemberData(nameof(InvalidChars))]
    public void IsNameValid_CorrectCode(string name, NameValid expected) {

        var actual = _sut.IsNameValid(name);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(2, MaxVersionsValid.Valid)]
    [InlineData(0, MaxVersionsValid.LessThanOne)]
    [InlineData(-1, MaxVersionsValid.LessThanOne)]
    public void IsMaxVersionsValid_CorrectCode(int maxVersiong, MaxVersionsValid expected) {

        var actual = _sut.IsMaxVersionsValid(maxVersiong);
        Assert.Equal(expected, actual);
    }

    private static object[] RealDirectoryTestData() => new object[] { new object[] { Directory.GetCurrentDirectory(), SourcePathValid.Valid } };

    [Theory]
    [MemberData(nameof(RealDirectoryTestData))]
    [InlineData(null, SourcePathValid.EmptyOrWhiteSpace)]
    [InlineData("", SourcePathValid.EmptyOrWhiteSpace)]
    [InlineData("    ", SourcePathValid.EmptyOrWhiteSpace)]
    [InlineData("L::\\Yo\\DoesThisExist.nope", SourcePathValid.DoesNotExist)]
    public void IsSourcePathValid_CorrectCode(string path, SourcePathValid expected) {

        var actual = _sut.IsSourcePathValid(path);
        Assert.Equal(expected, actual);
    }

}
