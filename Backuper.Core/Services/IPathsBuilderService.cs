namespace Backuper.Core.Services; 

public interface IPathsBuilderService {

    event Action? BackupersPathChanged;

    DateTime VersionNameToDateTime(string versionPath);
    string GenerateNewBackupVersionDirectory(string backuperName);
    string GetBackupsDirectory(string name);
    string GetBinDirectory(string name);

}
