using Backuper.Core.Models;
using Backuper.Core.Services;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Backuper.Core.Tests")]
namespace Backuper.Core;

/// <summary>
/// Provides properties and instance methods for creation, modification, and bin of backups.
/// </summary>
public interface IBackuper : IDisposable {

    /// <summary>
    /// <inheritdoc cref="BackuperInfo.Name"/>
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// <inheritdoc cref="BackuperInfo.SourcePath"/>
    /// </summary>
    public string SourcePath { get; }

    /// <summary>
    /// <inheritdoc cref="BackuperInfo.MaxVersions"/>
    /// </summary>
    public int MaxVersions { get; }

    /// <summary>
    /// <inheritdoc cref="IBackuperService.BackupAsync"/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<BackupResponseCode> BackupAsync(CancellationToken token = default);

    /// <summary>
    /// Tries to edit the backuper with the values of <paramref name="newInfo"/>, then returns the result. 
    /// <br/><br/>Changing <see cref="BackuperInfo.SourcePath"/> is not allowed.
    /// </summary>
    /// <param name="newInfo"></param>
    /// <returns></returns>
    Task<EditBackuperResponseCode> EditAsync(BackuperInfo newInfo);

    /// <summary>
    /// Bins the backuper. All backups will be moved to a bin path, and the data pertaining this backuper will be deleted.
    /// </summary>
    /// <returns></returns>
    Task BinAsync();

    /// <summary>
    /// Determines whether the last backup is more recent than the source's last write time.
    /// </summary>
    /// <returns></returns>
    public bool IsUpdated();

}
