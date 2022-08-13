namespace Backuper.DependencyInversion;

public interface IDirectoryInfoProvider {
    IDirectoryInfo FromDirectoryPath(string path);
}
