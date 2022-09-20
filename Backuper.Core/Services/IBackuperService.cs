using Backuper.Utils;

namespace Backuper.Core.Services;

/// <summary>
/// Handles backing up the source.
/// </summary>
public interface IBackuperService {

    /// <summary>
    /// Tries to back up the source and returns the result.
    /// </summary>
    /// <param name="newVersionPath"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<BackupResult> BackupAsync(string newVersionPath, CancellationToken token = default);

    /// <summary>
    /// Returns the source's last write time UTC.
    /// </summary>
    /// <returns></returns>
    Option<DateTime> GetSourceLastWriteTimeUTC();

}

/// <summary>
/// The result of a backup.
/// </summary>
public enum BackupResult {

    /// <summary>
    /// The backup has failed for an unknown reason.
    /// </summary>
    Failure,

    /// <summary>
    /// The backup has succeeded.
    /// </summary>
    Success,

    /// <summary>
    /// The source path is missing.
    /// </summary>
    Hibernating,

    /// <summary>
    /// The backuper data is corrupted.
    /// </summary>
    Corrupted
}