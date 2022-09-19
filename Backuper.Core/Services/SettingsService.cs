using Backuper.Utils;
using static Backuper.Core.PathsHandler;

namespace Backuper.Core.Services;

public class SettingsService {

    private readonly PathsHandler _pathsHandler;
    private readonly AutoBootService _autoBootService;
    private readonly Settings _settings;

    public const string StartupArguments = AutoBootService.StartupArguments;

    private const string autoBootKey = "AutoBoot";
    private const string defaultAutoBoot = "True";

    public SettingsService(PathsHandler pathsHandler, AutoBootService autoBootService, Settings settings) {
        _pathsHandler = pathsHandler;
        _autoBootService = autoBootService;
        _settings = settings;
        EnsureConfigsAreApplied();
    }

    public string GetSettingsFile() => _pathsHandler.GetSettingsFile();
    public string GetBackupersDirectory() => _pathsHandler.GetBackupersDirectory();
    public string GetBackupsDirectory() => _pathsHandler.GetBackupsDirectory();
    public Task<BackupersMigrationResult> ResetBackupsDirectory() => _pathsHandler.ResetBackupsDirectory();
    public Task<BackupersMigrationResult> SetBackupsDirectoryAsync(string newPath) => _pathsHandler.SetBackupsDirectoryAsync(newPath);
    
    
    public bool GetAutoBoot() => _autoBootService.Get();
    public void SetAutoBoot(bool value) {
        _autoBootService.Set(value);
        _settings.Set(autoBootKey, value.ToString());
    }

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