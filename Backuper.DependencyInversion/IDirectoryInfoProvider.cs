namespace Backuper.DependencyInversion;

public interface IDirectoryInfoProvider {
    IDirectoryInfoWrapper Create(string path);
}
