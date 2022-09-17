using Backuper.Utils;

namespace Backuper.Core.Services;

/// <summary>
/// Handles backuping the source.
/// </summary>
public interface IBackuperService {

    Task<BackupResult> BackupAsync(string newVersionPath, CancellationToken token = default);
    Option<DateTime> GetSourceLastWriteTimeUTC();

}

public enum BackupResult {
    Failure,
    Success,
    Hibernating
}