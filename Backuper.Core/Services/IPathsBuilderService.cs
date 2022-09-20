namespace Backuper.Core.Services; 

public interface IPathsBuilderService {

    /// <summary>
    /// Raises an event when the backups path is changed.
    /// </summary>
    event Action? BackupsPathChanged;

    DateTime VersionNameToDateTime(string versionPath);
    string GenerateNewBackupVersionDirectory(string backuperName);
    string GetBackupsDirectory(string name);
    string GetBinDirectory(string name);

}
