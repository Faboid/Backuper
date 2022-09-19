using Backuper.Core.Services;

namespace Backuper.Core.Tests.Services;

public class CorruptedBackuperServiceTests {

    private readonly CorruptedBackuperService _sut = new();

    [Fact]
    public async Task ReturnsCorruptedOnBackupResult() {
        Assert.Equal(BackupResult.Corrupted, await _sut.BackupAsync(""));
    }

    [Fact]
    public void ReturnsNoneWhenCheckingSourceLastWriteTime() {
        Assert.Equal(Utils.Options.OptionResult.None, _sut.GetSourceLastWriteTimeUTC().Result());
    }

}