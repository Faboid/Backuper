namespace Backuper.Abstractions;

public interface IShortcutProvider {
    IShortcut FromShortcutPaths(string shortcutPath, string targetPath);
}
