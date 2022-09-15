using Backuper.Abstractions;
using Backuper.Utils;

namespace Backuper.Core;

public class PathsHandler {

    private readonly IDirectoryInfoProvider _directoryInfoProvider;
    private readonly IFileInfoProvider _fileInfoProvider;
    private readonly Settings _settings;
    private const string backupersDirectoryKey = "BackupersDirectory";
    private const string backupsDirectoryKey = "BackupsDirectory";

    public PathsHandler(IDirectoryInfoProvider directoryInfoProvider, IFileInfoProvider fileInfoProvider) {
        _directoryInfoProvider = directoryInfoProvider;
        _fileInfoProvider = fileInfoProvider;
        _settings = new(_fileInfoProvider.FromFilePath(DefaultPaths.SettingsFile));
    }

    public string GetSettingsFile() => DefaultPaths.SettingsFile;
    public string GetBackupsDirectory() => _settings.Get(backupsDirectoryKey).Or(null) ?? DefaultPaths.BackupsDirectory;
    public string GetBackupersDirectory() => _settings.Get(backupersDirectoryKey).Or(null) ?? DefaultPaths.BackupersDirectory;
    public Task<BackupersMigrationResult> ResetBackupersDirectory() => SetBackupersDirectoryAsync(DefaultPaths.BackupersDirectory);

    public async Task<BackupersMigrationResult> SetBackupersDirectoryAsync(string newPath) {

        if(!IsPathValid(newPath)) {
            return BackupersMigrationResult.InvalidPath;
        }

        var newDir = _directoryInfoProvider.FromDirectoryPath(newPath);
        if(newDir.Exists && (newDir.EnumerateDirectories().Any() || newDir.EnumerateFiles().Any())) {
            return BackupersMigrationResult.TargetDirectoryIsNotEmpty;
        }

        try {
            var currentPath = GetBackupersDirectory();
            var currDir = _directoryInfoProvider.FromDirectoryPath(currentPath);
            await currDir.CopyToAsync(newPath);
            _settings.Set(backupersDirectoryKey, newPath);
            currDir.Delete(true);
        } catch(Exception) {
            return BackupersMigrationResult.Failure;
        }

        return BackupersMigrationResult.Success;

    }

    private static bool IsPathValid(string newPath) {

        return 
            !string.IsNullOrWhiteSpace(newPath) 
            && Path.IsPathRooted(newPath) 
            && newPath.Where(x => x == ':').Count() == 1
            && !Path.GetInvalidPathChars().Any(x => newPath.Contains(x));

    }

    public enum BackupersMigrationResult {
        Unknown,
        Failure,
        Success,
        InvalidPath,
        TargetDirectoryIsNotEmpty,
    }

}

public static class DefaultPaths {

    public static readonly string WorkingDirectory = Directory.GetCurrentDirectory();
    public static readonly string BackupsDirectory = Path.Combine(WorkingDirectory, "Backups");
    public static readonly string BackupersDirectory = Path.Combine(WorkingDirectory, "Backupers");
    public static readonly string SettingsFile = Path.Combine(WorkingDirectory, "Settings.txt");

}