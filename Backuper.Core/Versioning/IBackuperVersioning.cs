namespace Backuper.Core.Versioning; 

public interface IBackuperVersioning {

    string GenerateNewBackupVersionDirectory();
    Task MigrateTo(string newName);
    Task Bin();
    void DeleteExtraVersions(int maxVersions);
    DateTime GetLastBackupTimeUTC();

}
