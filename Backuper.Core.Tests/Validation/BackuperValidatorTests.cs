using Backuper.Abstractions.TestingHelpers;
using Backuper.Core.Models;
using Backuper.Core.Validation;

namespace Backuper.Core.Tests.Validation; 

public class BackuperValidatorTests {

    public BackuperValidatorTests() {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(_existingPath);

        var fileInfoProvider = new MockFileInfoProvider(fileSystem);
        var dirInfoProvider = new MockDirectoryInfoProvider(fileSystem);
        _sut = new BackuperValidator(dirInfoProvider, fileInfoProvider);
    }

    private static readonly string _existingPath = Directory.GetCurrentDirectory();
    private readonly IBackuperValidator _sut;

    private static BackuperInfo GetValidBase(string name = "SomeName") => new BackuperInfo(name, _existingPath, 3, false);
    
    private static IEnumerable<object[]> GenericIsValidData() {
        static object[] NewCase(BackuperInfo info, BackuperValid expected) => new object[] { info, expected };
        
        yield return NewCase(GetValidBase(), BackuperValid.Valid);
        yield return NewCase(null!, BackuperValid.IsNull);
    }

    [Theory]
    [MemberData(nameof(GenericIsValidData))]
    public void IsValid_CorrectCode(BackuperInfo info, BackuperValid expected) {

        var actual = _sut.IsValid(info);
        Assert.Equal(expected, actual);

    }

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

    private static object[] RealDirectoryTestData() => new object[] { new object[] { _existingPath, SourcePathValid.Valid } };

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
