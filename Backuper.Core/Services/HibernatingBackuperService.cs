using Backuper.Utils;

namespace Backuper.Core.Services;

public class HibernatingBackuperService : IBackuperService {
    public Task<BackupResult> BackupAsync(string newVersionPath, CancellationToken token = default) => Task.FromResult(BackupResult.Hibernating);
    public Option<DateTime> GetSourceLastWriteTimeUTC() => Option.None<DateTime>();
}