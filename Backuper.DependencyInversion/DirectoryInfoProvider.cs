namespace Backuper.DependencyInversion;

public class DirectoryInfoProvider : IDirectoryInfoProvider {
    public IDirectoryInfoWrapper Create(string path) {
        return new DirectoryInfoWrapper(path);
    }
}
