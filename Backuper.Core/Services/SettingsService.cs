using Backuper.Utils;
using static Backuper.Core.PathsHandler;

namespace Backuper.Core.Services;

/// <summary>
/// Provides centralized methods to get and set settings for future usages.
/// </summary>
public class SettingsService {

    private readonly PathsHandler _pathsHandler;
    private readonly AutoBootService _autoBootService;
    private readonly Settings _settings;

    public const string StartupArguments = AutoBootService.StartupArguments;

    private const string autoBootKey = "AutoBoot";
    private const string defaultAutoBoot = "True";

    /// <summary>
    /// Instances <see cref="SettingsService"/>.
    /// </summary>
    /// <param name="pathsHandler"></param>
    /// <param name="autoBootService"></param>
    /// <param name="settings"></param>
    public SettingsService(PathsHandler pathsHandler, AutoBootService autoBootService, Settings settings) {
        _pathsHandler = pathsHandler;
        _autoBootService = autoBootService;
        _settings = settings;
        EnsureConfigsAreApplied();
    }

    /// <summary>
    /// <inheritdoc cref="PathsHandler.GetSettingsFile"/>
    /// </summary>
    /// <returns></returns>
    public string GetSettingsFile() => _pathsHandler.GetSettingsFile();

    /// <summary>
    /// <inheritdoc cref="PathsHandler.GetBackupersDirectory"/>
    /// </summary>
    /// <returns></returns>
    public string GetBackupersDirectory() => _pathsHandler.GetBackupersDirectory();

    /// <summary>
    /// <inheritdoc cref="PathsHandler.GetBackupsDirectory"/>
    /// </summary>
    /// <returns></returns>
    public string GetBackupsDirectory() => _pathsHandler.GetBackupsDirectory();

    /// <summary>
    /// <inheritdoc cref="PathsHandler.ResetBackupsDirectory"/>
    /// </summary>
    /// <returns></returns>
    public Task<BackupersMigrationResult> ResetBackupsDirectory() => _pathsHandler.ResetBackupsDirectory();

    /// <summary>
    /// <inheritdoc cref="PathsHandler.SetBackupsDirectoryAsync(string)"/>
    /// </summary>
    /// <param name="newPath"></param>
    /// <returns></returns>
    public Task<BackupersMigrationResult> SetBackupsDirectoryAsync(string newPath) => _pathsHandler.SetBackupsDirectoryAsync(newPath);
    
    /// <summary>
    /// <inheritdoc cref="AutoBootService.Get"/>
    /// </summary>
    /// <returns></returns>
    public bool GetAutoBoot() => _autoBootService.Get();

    /// <summary>
    /// <inheritdoc cref="AutoBootService.Set(bool)"/>
    /// </summary>
    /// <param name="value"></param>
    public void SetAutoBoot(bool value) {
        _autoBootService.Set(value);
        _settings.Set(autoBootKey, value.ToString());
    }

    /// <summary>
    /// Ensures that the settings are applied. If, for example, the auto-boot settings is set to true, it ensures that everything is set up to auto-boot correctly.
    /// </summary>
    public void EnsureConfigsAreApplied() {
        
        if(!bool.TryParse(_settings.Get(autoBootKey).Or(defaultAutoBoot), out var autoBoolSetting)) {
            _settings.Set(autoBootKey, defaultAutoBoot);
            autoBoolSetting = bool.Parse(defaultAutoBoot);
        }

        if(_autoBootService.Get() != autoBoolSetting) {
            _autoBootService.Set(autoBoolSetting);
        }

    }

}