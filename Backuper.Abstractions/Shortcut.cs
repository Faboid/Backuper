using IWshRuntimeLibrary;

namespace Backuper.Abstractions;

/// <summary>
/// A wrapper of <see cref="IWshShortcut"/> to facilitate creating shortcuts.
/// </summary>
public class Shortcut : IShortcut {

    public Shortcut(string shortcutPath, string targetPath) {
        if(!shortcutPath.EndsWith(".lnk", StringComparison.InvariantCultureIgnoreCase)) {
            throw new ArgumentException("Shortcuts must end with \".lnk\" as extension.");
        }

        _shortcut = (IWshShortcut)new WshShell().CreateShortcut(shortcutPath);
        _shortcut.TargetPath = targetPath;
    }

    private readonly IWshShortcut _shortcut;

    public Shortcut SetArguments(string arguments) {
        _shortcut.Arguments = arguments;
        return this;
    }

    public Shortcut SetDescription(string description) {
        _shortcut.Description = description;
        return this;
    }

    public void Create() => _shortcut.Save();
    public void Delete() => System.IO.File.Delete(_shortcut.FullName);
    public bool Exists() => System.IO.File.Exists(_shortcut.FullName);

}