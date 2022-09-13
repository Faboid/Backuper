namespace Backuper.Abstractions;

public interface IShortcut {
    bool Exists();
    void Save();
    Shortcut SetArguments(string arguments);
    Shortcut SetDescription(string description);
}
