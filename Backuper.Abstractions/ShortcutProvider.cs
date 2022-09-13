namespace Backuper.Abstractions;

public class ShortcutProvider : IShortcutProvider {
    public IShortcut FromShortcutPaths(string shortcutPath, string targetPath) => new Shortcut(shortcutPath, targetPath);
}
