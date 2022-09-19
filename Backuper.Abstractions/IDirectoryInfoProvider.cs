namespace Backuper.Abstractions;

public interface IDirectoryInfoProvider {
    IDirectoryInfo FromDirectoryPath(string path);
}
