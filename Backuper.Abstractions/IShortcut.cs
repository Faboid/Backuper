namespace Backuper.Abstractions;

public interface IShortcut {
    bool Exists();
    void Create();
    void Delete();
    Shortcut SetArguments(string arguments);
    Shortcut SetDescription(string description);
}
