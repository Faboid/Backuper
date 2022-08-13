namespace Backuper.DependencyInversion;

public class DirectoryInfoProvider : IDirectoryInfoProvider {
    public IDirectoryInfoWrapper FromDirectoryPath(string path) {
        return new DirectoryInfoWrapper(path);
    }
}
