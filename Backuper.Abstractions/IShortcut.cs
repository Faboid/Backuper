namespace Backuper.Abstractions;

public interface IShortcut {
    bool Exists();
    void Create();
    void Delete();
    IShortcut SetArguments(string arguments);
    IShortcut SetDescription(string description);
}
