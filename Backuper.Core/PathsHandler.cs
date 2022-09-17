using Backuper.Abstractions;
using Backuper.Utils;
using Microsoft.Extensions.Logging;

namespace Backuper.Core;

public class PathsHandler {

    private readonly ILogger<PathsHandler>? _logger;
    private readonly IDirectoryInfoProvider _directoryInfoProvider;
    private readonly IFileInfoProvider _fileInfoProvider;
    private readonly Settings _settings;
    private const string backupersDirectoryKey = "BackupersDirectory";
    private const string backupsDirectoryKey = "BackupsDirectory";

    public event Action? BackupersPathChanged;

    public PathsHandler(IDirectoryInfoProvider directoryInfoProvider, IFileInfoProvider fileInfoProvider, ILogger<PathsHandler>? logger = null) {
        _directoryInfoProvider = directoryInfoProvider;
        _fileInfoProvider = fileInfoProvider;
        _logger = logger;
        _settings = new(_fileInfoProvider.FromFilePath(DefaultPaths.SettingsFile));
    }

    public string GetSettingsFile() => DefaultPaths.SettingsFile;
    public string GetBackupsDirectory() => _settings.Get(backupsDirectoryKey).Or(DefaultPaths.BackupsDirectory)!;
    public string GetBackupersDirectory() => _settings.Get(backupersDirectoryKey).Or(DefaultPaths.BackupersDirectory)!;
    public Task<BackupersMigrationResult> ResetBackupersDirectory() => SetBackupersDirectoryAsync(DefaultPaths.BackupersDirectory);

    public async Task<BackupersMigrationResult> SetBackupersDirectoryAsync(string newPath) {

        if(newPath == GetBackupersDirectory()) {
            return BackupersMigrationResult.AlreadyThere;
        }

        if(!IsPathValid(newPath)) {
            return BackupersMigrationResult.InvalidPath;
        }

        var newDir = _directoryInfoProvider.FromDirectoryPath(newPath);
        if(newDir.Exists && (newDir.EnumerateDirectories().Any() || newDir.EnumerateFiles().Any())) {
            return BackupersMigrationResult.TargetDirectoryIsNotEmpty;
        }
        
        var currentPath = GetBackupersDirectory();

        try {
            _logger?.LogInformation("Beginning to set new main backups directory.");
            var currDir = _directoryInfoProvider.FromDirectoryPath(currentPath);
            _logger?.LogInformation("Migrating backups from {OldPath} to {NewPath}", currentPath, newPath);
            await currDir.CopyToAsync(newPath);
            _logger?.LogInformation("Migrated successfully from {OldPath} to {NewPath}", currentPath, newPath);
            _logger?.LogInformation("Saving new path to settings.");
            _settings.Set(backupersDirectoryKey, newPath);
            _logger?.LogInformation("Saved new backupers path successfully.");
            _logger?.LogInformation("Deleting old backups...");
            currDir.Delete(true);
            _logger?.LogInformation("Deleted old backups successfully.");
        } catch(Exception ex) {

            _logger?.LogError(ex, "An error has occurred while changing the main backups directory from {OldPath} to {NewPath}.", currentPath, newPath);
            return BackupersMigrationResult.Failure;
        }

        _logger?.LogInformation("The main backups directory has been migrated successfully.");
        OnBackupersPathChanged();
        return BackupersMigrationResult.Success;

    }

    private static bool IsPathValid(string newPath) {

        return 
            !string.IsNullOrWhiteSpace(newPath) 
            && Path.IsPathRooted(newPath) 
            && newPath.Where(x => x == ':').Count() == 1
            && !Path.GetInvalidPathChars().Any(x => newPath.Contains(x));

    }

    private void OnBackupersPathChanged() => BackupersPathChanged?.Invoke();

    public enum BackupersMigrationResult {
        Unknown,
        Failure,
        Success,
        InvalidPath,
        TargetDirectoryIsNotEmpty,
        AlreadyThere,
    }

}

public static class DefaultPaths {

    public static readonly string WorkingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()!.Location)!;
    public static readonly string BackupsDirectory = Path.Combine(WorkingDirectory, "Backups");
    public static readonly string BackupersDirectory = Path.Combine(WorkingDirectory, "Backupers");
    public static readonly string SettingsFile = Path.Combine(WorkingDirectory, "Settings.txt");

}