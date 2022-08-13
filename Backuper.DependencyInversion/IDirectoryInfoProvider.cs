namespace Backuper.DependencyInversion;

public interface IDirectoryInfoProvider {
    IDirectoryInfoWrapper FromDirectoryPath(string path);
}
