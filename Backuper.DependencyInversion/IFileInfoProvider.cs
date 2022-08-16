namespace Backuper.Abstractions;

public interface IFileInfoProvider {
    IFileInfo FromFilePath(string path);
}
