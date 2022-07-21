using Backuper.Core.Models;
using Backuper.Extensions;
using Backuper.Utils;
namespace Backuper.Core.BackuperTypes;

public class DirectoryBackuper : IBackuper {

    public DirectoryBackuper(BackuperInfo info, PathsBuilder pathsBuilder) {
        Info = info;
        paths = pathsBuilder.Build(info.Name);
        Directory.CreateDirectory(paths.BackupsDirectory);
        Source = new(info.SourcePath);
        IsUpdated = IsUpToDate();
    }

    public DirectoryInfo Source { get; private set; }
    public BackuperInfo Info { get; init; }
    public bool IsUpdated { get; private set; }

    private readonly Paths paths;
    private readonly Locker locker = new();

    //todo - these methods delete the main directory of this backuper.
    //either set up creation of it in the StartBackupsAsync method,
    //or add some flag that throws if someone tries to backup after erasing.
    public async Task BinBackupsAsync(CancellationToken token = default) {
        using var lockd = await locker.GetLockAsync().ConfigureAwait(false);
        if(token.IsCancellationRequested) return;
        using var threadHandler = ThreadsHandler.SetScopedForeground();
        await new DirectoryInfo(paths.BackupsDirectory).CopyToAsync(paths.BinDirectory).ConfigureAwait(false);
        Directory.Delete(paths.BackupsDirectory, true);
    }
    public async Task EraseBackupsAsync(CancellationToken token = default) {
        using var lockd = await locker.GetLockAsync().ConfigureAwait(false);
        if(token.IsCancellationRequested) return;
        using var threadHandler = ThreadsHandler.SetScopedForeground();
        Directory.Delete(paths.BackupsDirectory, true);
    }

    public async Task StartBackupAsync(CancellationToken token = default) {
        using var lockd = await locker.GetLockAsync().ConfigureAwait(false); //todo - return special code when the lock doesn't get obtained quickly
        using var threadHandler = ThreadsHandler.SetScopedForeground();

        if(IsUpToDate() || token.IsCancellationRequested) {
            return;
        }

        var path = paths.GenerateNewBackupVersionDirectory();
        await Source.CopyToAsync(path).ConfigureAwait(false);

        //delete extra versions
        Directory.EnumerateDirectories(paths.BackupsDirectory)
            .OrderByDescending(x => Directory.GetCreationTime(x))
            .Skip(Info.MaxVersions)
            .ForEach(x => Directory.Delete(x, true));

        IsUpdated = true;
    }

    private bool IsUpToDate() {

        //backups are done and then never touched,
        //so it's best to get the creation time
        var latestBackup = Directory
            .EnumerateDirectories(paths.BackupsDirectory, "*", SearchOption.TopDirectoryOnly)
            .Select(x => Directory.GetCreationTimeUtc(x))
            .DefaultIfEmpty()
            .Max();

        if(latestBackup == default || Source.LastWriteTimeUtc > latestBackup) {
            return false;
        }

        //windows doesn't update the parents' folders' last write time
        //therefore, it's needed to go through all children directories
        var latestChange = Directory
            .EnumerateDirectories(Source.FullName, "*", SearchOption.AllDirectories)
            .Select(x => Directory.GetLastWriteTimeUtc(x))
            .DefaultIfEmpty()
            .Max();

        return latestBackup > latestChange;

    }

}
