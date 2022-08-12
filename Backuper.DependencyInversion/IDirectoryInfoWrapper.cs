namespace Backuper.DependencyInversion;

public interface IDirectoryInfoWrapper {

    IEnumerable<IDirectoryInfoWrapper> EnumerateDirectories(string searchPattern, SearchOption searchOption);
    IEnumerable<IDirectoryInfoWrapper> EnumerateDirectories();
    Task CopyToAsync(string path);
    void Delete(bool recursively);

}