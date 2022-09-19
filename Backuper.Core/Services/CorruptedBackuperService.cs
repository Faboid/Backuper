using Backuper.Utils;

namespace Backuper.Core.Services;

internal class CorruptedBackuperService : IBackuperService {
    public Task<BackupResult> BackupAsync(string newVersionPath, CancellationToken token = default) => Task.FromResult(BackupResult.Corrupted);
    public Option<DateTime> GetSourceLastWriteTimeUTC() => Option.None<DateTime>();
}