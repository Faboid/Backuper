using Backuper.Abstractions;
using Backuper.Utils;
using Microsoft.Extensions.Logging;

namespace Backuper.Core;

/// <summary>
/// Provides methods to get and set the paths used throughout the application.
/// </summary>
public class PathsHandler {

    private readonly ILogger<PathsHandler>? _logger;
    private readonly IDirectoryInfoProvider _directoryInfoProvider;
    private readonly IFileInfoProvider _fileInfoProvider;
    private readonly Settings _settings;
    private const string backupersDirectoryKey = "BackupersDirectory";
    private const string backupsDirectoryKey = "BackupsDirectory";

    /// <summary>
    /// Raises an event when the backups' path is changed.
    /// </summary>
    public event Action? BackupsPathChanged;

    /// <summary>
    /// Instances <see cref="PathsHandler"/>.
    /// </summary>
    /// <param name="directoryInfoProvider"></param>
    /// <param name="fileInfoProvider"></param>
    /// <param name="logger"></param>
    public PathsHandler(IDirectoryInfoProvider directoryInfoProvider, IFileInfoProvider fileInfoProvider, ILogger<PathsHandler>? logger = null) {
        _directoryInfoProvider = directoryInfoProvider;
        _fileInfoProvider = fileInfoProvider;
        _logger = logger;
        _directoryInfoProvider.FromDirectoryPath(DefaultPaths.DataDirectory).Create();
        _settings = new(_fileInfoProvider.FromFilePath(DefaultPaths.SettingsFile));
    }

    /// <summary>
    /// Retrieves the settings file path.
    /// </summary>
    /// <returns></returns>
    public string GetSettingsFile() => DefaultPaths.SettingsFile;

    /// <summary>
    /// Retrieves the backupers directory path.
    /// </summary>
    /// <returns></returns>
    public string GetBackupersDirectory() => _settings.Get(backupersDirectoryKey).Or(DefaultPaths.BackupersDirectory)!;

    /// <summary>
    /// Retrieves the backups directory path.
    /// </summary>
    /// <returns></returns>
    public string GetBackupsDirectory() => _settings.Get(backupsDirectoryKey).Or(DefaultPaths.BackupsDirectory)!;

    /// <summary>
    /// Tries to reset the backups directory path to <see cref="DefaultPaths.BackupsDirectory"/>.
    /// </summary>
    /// <returns></returns>
    public Task<BackupersMigrationResult> ResetBackupsDirectory() => SetBackupsDirectoryAsync(DefaultPaths.BackupsDirectory);

    /// <summary>
    /// Sets a new path for the backups directory. Tries to migrate all existing backups to it, then returns the result.
    /// </summary>
    /// <param name="newPath"></param>
    /// <returns></returns>
    public async Task<BackupersMigrationResult> SetBackupsDirectoryAsync(string newPath) {

        if(newPath == GetBackupsDirectory()) {
            return BackupersMigrationResult.AlreadyThere;
        }

        if(!IsPathValid(newPath)) {
            return BackupersMigrationResult.InvalidPath;
        }

        var newDir = _directoryInfoProvider.FromDirectoryPath(newPath);
        if(newDir.Exists && (newDir.EnumerateDirectories().Any() || newDir.EnumerateFiles().Any())) {
            return BackupersMigrationResult.TargetDirectoryIsNotEmpty;
        }
        
        var currentPath = GetBackupsDirectory();

        try {
            _logger?.LogInformation("Beginning to set new main backups directory.");
            var currDir = _directoryInfoProvider.FromDirectoryPath(currentPath);
            _logger?.LogInformation("Migrating backups from {OldPath} to {NewPath}", currentPath, newPath);
            await currDir.CopyToAsync(newPath);
            _logger?.LogInformation("Migrated successfully from {OldPath} to {NewPath}", currentPath, newPath);
            _logger?.LogInformation("Saving new path to settings.");
            _settings.Set(backupsDirectoryKey, newPath);
            _logger?.LogInformation("Saved new backups path successfully.");
            _logger?.LogInformation("Deleting old backups...");
            currDir.Delete(true);
            _logger?.LogInformation("Deleted old backups successfully.");
        } catch(Exception ex) {

            _logger?.LogError(ex, "An error has occurred while changing the main backups directory from {OldPath} to {NewPath}.", currentPath, newPath);
            return BackupersMigrationResult.Failure;
        }

        _logger?.LogInformation("The main backups directory has been migrated successfully.");
        OnBackupsPathChanged();
        return BackupersMigrationResult.Success;

    }

    private static bool IsPathValid(string newPath) {

        return 
            !string.IsNullOrWhiteSpace(newPath) 
            && Path.IsPathRooted(newPath) 
            && newPath.Where(x => x == ':').Count() == 1
            && !Path.GetInvalidPathChars().Any(x => newPath.Contains(x));

    }

    private void OnBackupsPathChanged() => BackupsPathChanged?.Invoke();

    public enum BackupersMigrationResult {
        Unknown,
        Failure,
        Success,
        InvalidPath,
        TargetDirectoryIsNotEmpty,
        AlreadyThere,
    }

}

/// <summary>
/// Provides a set of static readonly paths.
/// </summary>
public static class DefaultPaths {

    /// <summary>
    /// The directory that contains the exe this application is running from.
    /// </summary>
    public static string WorkingDirectory { get; } = AppDomain.CurrentDomain.BaseDirectory;

    /// <summary>
    /// The directory that contains all data related to this application.
    /// </summary>
    public static string DataDirectory { get; } = Path.Combine(WorkingDirectory, "Data");

    /// <summary>
    /// The directory that contains all logs.
    /// </summary>
    public static string LogsDirectory { get; } = Path.Combine(DataDirectory, "Logs");

    /// <summary>
    /// Directory that contains all the backuper's data.
    /// </summary>
    public static string BackupersDirectory { get; } = Path.Combine(DataDirectory, "Backupers");

    /// <summary>
    /// Directory that contains all backups.
    /// </summary>
    public static string BackupsDirectory { get; } = Path.Combine(DataDirectory, "BackupSaves");

    /// <summary>
    /// The settings file.
    /// </summary>
    public static string SettingsFile { get; } = Path.Combine(DataDirectory, "Config.txt");

}