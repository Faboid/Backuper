using Backuper.Core.Services;
using Backuper.Core.Models;
namespace Backuper.Core.Versioning;

/// <summary>
/// Provides a set of methods to handle the versions of a specific backuper.
/// </summary>
public interface IBackuperVersioning {

    /// <summary>
    /// <inheritdoc cref="PathsBuilderService.GenerateNewBackupVersionDirectory(string)"/>
    /// </summary>
    /// <returns></returns>
    string GenerateNewBackupVersionDirectory();

    /// <summary>
    /// Migrates the backups of the backuper to a new path based on the <paramref name="newName"/>.
    /// </summary>
    /// <param name="newName"></param>
    /// <returns></returns>
    Task MigrateTo(string newName);

    /// <summary>
    /// Moves existing backups of this backuper into the bin folder. They will stay there until they are manually deleted.
    /// </summary>
    /// <returns></returns>
    Task Bin();

    /// <summary>
    /// Deletes the older versions that excess <see cref="BackuperInfo.MaxVersions"/>.
    /// </summary>
    /// <param name="maxVersions"></param>
    void DeleteExtraVersions(int maxVersions);

    /// <summary>
    /// Gets the last time a backup has executed successfully.
    /// </summary>
    /// <returns></returns>
    DateTime GetLastBackupTimeUTC();

}
