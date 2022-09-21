using Backuper.Abstractions;
using System.Globalization;

namespace Backuper.Core.Services;

public class PathsBuilderService : IPathsBuilderService {

    public event Action? BackupsPathChanged;

    public PathsBuilderService(PathsHandler pathsHandler, IDateTimeProvider dateTimeProvider, IDirectoryInfoProvider directoryInfoProvider) {
        _pathsHandler = pathsHandler;
        _dateTimeProvider = dateTimeProvider;
        _directoryInfoProvider = directoryInfoProvider;
        _pathsHandler.BackupsPathChanged += () => BackupsPathChanged?.Invoke();
    }

    private static readonly DateTimeFormatInfo format = new();

    private readonly IDirectoryInfoProvider _directoryInfoProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly PathsHandler _pathsHandler;

    public string GetBackupsDirectory(string name) => Path.Combine(_pathsHandler.GetBackupsDirectory(), "Backups", name);
    public string GetBinDirectory(string name) => Path.Combine(_pathsHandler.GetBackupsDirectory(), "Bin", name);

    public DateTime VersionNameToDateTime(string versionPath) {
        var name = _directoryInfoProvider.FromDirectoryPath(versionPath).Name.Replace('—', ':');
        name = name[Math.Max(0, name.IndexOf(']') + 1)..]; //skip version number
        return DateTime.ParseExact(name, format.UniversalSortableDateTimePattern, format);
    }

    public string GenerateNewBackupVersionDirectory(string backuperName) {
        return GenerateNewBackupVersionDirectory(backuperName, _dateTimeProvider.Now);
    }

    internal string GenerateNewBackupVersionDirectory(string backuperName, DateTime dateTime) {
        var backupsDirectory = GetBackupsDirectory(backuperName);
        var dateAsString = dateTime.ToString(format.UniversalSortableDateTimePattern).Replace(':', '—');
        var version = $"[{GetLatestVersionNumber(backupsDirectory) + 1}]{dateAsString}";
        return Path.Combine(backupsDirectory, version);
    }

    /// <summary>
    /// Returns the latest version number, or 0 if there aren't any.
    /// </summary>
    /// <returns></returns>
    private int GetLatestVersionNumber(string backupsDirectoryPath) {

        var backupsDirectory = _directoryInfoProvider.FromDirectoryPath(backupsDirectoryPath);

        if(!backupsDirectory.Exists) {
            return 0;
        }

        return backupsDirectory
            .EnumerateDirectories()
            .Select(x => x.Name)
            .Select(x => x[1..Math.Max(x.IndexOf(']'), 1)])
            .Select(x => (valid: int.TryParse(x, out var val), value: val))
            .Where(x => x.valid)
            .Select(x => x.value)
            .DefaultIfEmpty(0)
            .Max();

    }

}
