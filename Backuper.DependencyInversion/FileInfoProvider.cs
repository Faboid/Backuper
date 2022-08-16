namespace Backuper.Abstractions;

public class FileInfoProvider : IFileInfoProvider {
    public IFileInfo FromFilePath(string path) => new FileInfoWrapper(path);
}
