namespace Backuper.DependencyInversion;

public class FileInfoProvider : IFileInfoProvider {
    public IFileInfo FromFilePath(string path) => new FileInfoWrapper(path);
}
