using Backuper.Core.Services;

namespace Backuper.Core.Tests.Services;

public class HibernatingBackuperServiceTests {

    private readonly HibernatingBackuperService _sut = new();

    [Fact]
    public async Task ReturnsHibernateOnBackupResult() {
        Assert.Equal(BackupResult.Hibernating, await _sut.BackupAsync(""));
    }

    [Fact]
    public void ReturnsNoneWhenCheckingSourceLastWriteTime() {
        Assert.Equal(Utils.Options.OptionResult.None, _sut.GetSourceLastWriteTimeUTC().Result());
    }

}