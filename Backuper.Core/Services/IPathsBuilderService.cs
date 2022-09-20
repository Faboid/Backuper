namespace Backuper.Core.Services; 

/// <summary>
/// Provides a set of methods to generate paths specific to backupers.
/// </summary>
public interface IPathsBuilderService {

    /// <summary>
    /// Raises an event when the backups path is changed.
    /// </summary>
    event Action? BackupsPathChanged;

    /// <summary>
    /// Converts a previously-generated version path to a <see cref="DateTime"/> object corresponding its value.
    /// </summary>
    /// <param name="versionPath"></param>
    /// <returns></returns>
    DateTime VersionNameToDateTime(string versionPath);

    /// <summary>
    /// Generates a new version path to use in a new backup.
    /// </summary>
    /// <param name="backuperName"></param>
    /// <returns></returns>
    string GenerateNewBackupVersionDirectory(string backuperName);

    /// <summary>
    /// Gets the backups directory of this backuper.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    string GetBackupsDirectory(string name);

    /// <summary>
    /// Gets the bin directory of this backuper.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    string GetBinDirectory(string name);

}
