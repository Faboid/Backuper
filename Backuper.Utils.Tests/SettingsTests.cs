using Backuper.Abstractions;
using Backuper.Abstractions.TestingHelpers;
using Backuper.Utils.Options;

namespace Backuper.Utils.Tests;

public class SettingsTests {

    public SettingsTests() {
        string somePath = "F:\\Dir\\Settings.txt";

        IMockFileSystem mockFileSystem = new MockFileSystem();
        IFileInfoProvider fileInfoProvider= new MockFileInfoProvider(mockFileSystem);
        settings = new Settings(fileInfoProvider.FromFilePath(somePath));
    }

    private readonly Settings settings;

    [Theory]
    [InlineData("one", "isOne")]
    [InlineData("one", "here")]
    [InlineData("three", "where")]
    public void CreateAndGet(string key, string value) {

        //act
        settings.Set(key, value);

        //assert
        Assert.Equal(value, settings.Get(key).Or("missing"));

    }

    [Theory]
    [InlineData("one", "isOne")]
    [InlineData("one", "here")]
    [InlineData("three", "where")]
    public void OverrideAndGet(string key, string value) {

        //act
        settings.Set(key, "SomeOtherValue");
        settings.Set(key, value);

        //assert
        Assert.Equal(value, settings.Get(key).Or("missing"));

    }

    [Fact]
    public void ReturnsNoneIfKeyDoesNotExist() {

        //arrange
        string key = "someNotExistingKey";

        //act
        var result = settings.Get(key);

        //assert
        Assert.Equal(OptionResult.None, result.Result());

    }

}