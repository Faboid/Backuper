namespace Backuper.Core.Versioning;

public interface IBackuperVersioningFactory {
    IBackuperVersioning CreateVersioning(string backuperName);
}