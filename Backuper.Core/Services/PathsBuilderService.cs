using System.Globalization;

namespace Backuper.Core.Services; 

public class PathsBuilderService : IPathsBuilderService {

    public PathsBuilderService(string mainBackupersDirectory) {
        _mainBackupersDirectory = mainBackupersDirectory;
    }

    private static readonly DateTimeFormatInfo format = new();
    private readonly string _mainBackupersDirectory;

    public string GetBackuperDirectory(string name) => Path.Combine(_mainBackupersDirectory, "Backups", name);
    public string GetBinDirectory(string name) => Path.Combine(_mainBackupersDirectory, "Bin", name);

    public DateTime VersionNameToDateTime(string versionPath) {
        var name = new DirectoryInfo(versionPath).Name.Replace('—', ':');
        name = name[Math.Max(0, name.IndexOf(']') + 1)..]; //skip version number
        return DateTime.ParseExact(name, format.UniversalSortableDateTimePattern, format);
    }

    public string GenerateNewBackupVersionDirectory(string backuperName) {
        return GenerateNewBackupVersionDirectory(backuperName, DateTime.Now);
    }

    internal string GenerateNewBackupVersionDirectory(string backuperName, DateTime dateTime) {
        var backupsDirectory = GetBackuperDirectory(backuperName);
        var dateAsString = dateTime.ToString(format.UniversalSortableDateTimePattern).Replace(':', '—');
        var version = $"[{GetLatestVersionNumber(backupsDirectory)}]{dateAsString}";
        return Path.Combine(backupsDirectory, version);
    }

    /// <summary>
    /// Returns the latest version number, or 0 if there aren't any.
    /// </summary>
    /// <returns></returns>
    private static int GetLatestVersionNumber(string backupsDirectory) {

        if(!Directory.Exists(backupsDirectory)) {
            return 0;
        }

        return Directory
            .EnumerateDirectories(backupsDirectory)
            .Select(x => new DirectoryInfo(x).Name)
            .Select(x => x[1..Math.Max(x.IndexOf(']'), 1)])
            .Select(x => (valid: int.TryParse(x, out var val), value: val))
            .Where(x => x.valid)
            .Select(x => x.value)
            .DefaultIfEmpty(0)
            .Max();

    }

}
