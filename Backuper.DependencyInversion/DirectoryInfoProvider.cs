namespace Backuper.Abstractions;

public class DirectoryInfoProvider : IDirectoryInfoProvider {
    public IDirectoryInfo FromDirectoryPath(string path) {
        return new DirectoryInfoWrapper(path);
    }
}
