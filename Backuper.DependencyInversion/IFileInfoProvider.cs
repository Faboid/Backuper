namespace Backuper.DependencyInversion;

public interface IFileInfoProvider {
    IFileInfo FromFilePath(string path);
}
