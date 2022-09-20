using Backuper.Abstractions;
using Microsoft.Extensions.Logging;

namespace Backuper.Core.Services;

/// <summary>
/// Handles setting up everything needed to boot the application automatically.
/// </summary>
public class AutoBootService {

    /// <summary>
    /// Instances <see cref="AutoBootService"/>.
    /// </summary>
    /// <param name="shortcutProvider"></param>
    /// <param name="logger"></param>
    public AutoBootService(IShortcutProvider shortcutProvider, ILogger<AutoBootService>? logger = null) {
        _shortcut = shortcutProvider.FromShortcutPaths(_shortcutPath, _pathToExe)
            .SetArguments(StartupArguments)
            .SetDescription("Boots Backuper.exe to execute a full automatic backup.");
        _logger = logger;
    }

    private readonly ILogger<AutoBootService>? _logger;
    private readonly IShortcut _shortcut;
    private static readonly string _pathToExe = Environment.ProcessPath!;
    private static readonly string _startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
    private static readonly string _shortcutPath = Path.Combine(_startupPath, "Backuper.lnk");

    /// <summary>
    /// The argument given when the application gets booted automatically.
    /// </summary>
    public const string StartupArguments = "AutoStartup";

    /// <summary>
    /// Sets whether the application should be instanced automatically on windows' boot.
    /// </summary>
    /// <param name="active"></param>
    public void Set(bool active) {
        if(active) {
            _logger?.LogInformation("Activating auto-boot at [{Path}]...", _shortcutPath);
            _shortcut.Create();
            _logger?.LogInformation("Activated auto-boot. Shortcut at [{ShortcutPath}].", _shortcutPath);
        } else {
            _logger?.LogInformation("Turning off auto-boot...");
            _shortcut.Delete();
            _logger?.LogInformation("Turned off auto-boot.");
        }
    }

    /// <summary>
    /// Gets whether the application is currently set to be instanced automatically on windows' boot.
    /// </summary>
    /// <returns></returns>
    public bool Get() => _shortcut.Exists();

}