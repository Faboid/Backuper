using Backuper.Abstractions;

namespace Backuper.Core.Services;

/// <summary>
/// Handles setting up everything needed to boot the application automatically.
/// </summary>
public class AutoBootService {

    public AutoBootService(IShortcutProvider shortcutProvider) {
        _shortcut = shortcutProvider.FromShortcutPaths(_shortcutPath, _pathToExe)
            .SetArguments(StartupArguments)
            .SetDescription("Boots Backuper.exe to execute a full automatic backup.");
    }

    private readonly IShortcut _shortcut;
    private static readonly string _pathToExe = Environment.ProcessPath!;
    private static readonly string _startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
    private static readonly string _shortcutPath = Path.Combine(_startupPath, "TempBackuper.lnk"); //todo - remove "temp"

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
            _shortcut.Create();
        } else {
            _shortcut.Delete();
        }
    }

    /// <summary>
    /// Gets whether the application is currently set to be instanced automatically on windows' boot.
    /// </summary>
    /// <returns></returns>
    public bool Get() => _shortcut.Exists();

}