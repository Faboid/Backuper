namespace Backuper.Core.Services;

/// <summary>
/// Handles backuping the source.
/// </summary>
public interface IBackuperService {

    Task BackupAsync(string newVersionPath, CancellationToken token = default);
    DateTime GetSourceLastWriteTimeUTC();

}
