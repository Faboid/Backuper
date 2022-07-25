using Backuper.Core.Models;
using Backuper.Extensions;
using Backuper.Utils;
namespace Backuper.Core.BackuperTypes;

public class FileBackuper : IBackuper {

    public FileBackuper(BackuperInfo info, PathsBuilder pathsBuilder) {
        Info = info;
        paths = pathsBuilder.Build(info.Name);
        Directory.CreateDirectory(paths.BackupsDirectory);
        Source = new(info.SourcePath);
        IsUpdated = IsUpToDate();
    }

    public FileInfo Source { get; private set; }
    public BackuperInfo Info { get; private set; }
    public bool IsUpdated { get; private set; }

    private readonly Paths paths;
    private readonly Locker locker = new();

    public async Task BinBackupsAsync(CancellationToken token = default) {
        using var lockd = await locker.GetLockAsync(CancellationToken.None).ConfigureAwait(false);
        if(token.IsCancellationRequested)
            return;
        using var threadHandler = ThreadsHandler.SetScopedForeground();
        await new DirectoryInfo(paths.BackupsDirectory).CopyToAsync(paths.BinDirectory).ConfigureAwait(false);
        Directory.Delete(paths.BackupsDirectory, true);
    }
    public async Task EraseBackupsAsync(CancellationToken token = default) {
        using var lockd = await locker.GetLockAsync(CancellationToken.None).ConfigureAwait(false);
        if(token.IsCancellationRequested)
            return;
        using var threadHandler = ThreadsHandler.SetScopedForeground();
        Directory.Delete(paths.BackupsDirectory, true);
    }

    public async Task StartBackupAsync(CancellationToken token = default) {
        using var lockd = await locker.GetLockAsync(CancellationToken.None).ConfigureAwait(false); //todo - return special code when the lock doesn't get obtained quickly
        using var threadHandler = ThreadsHandler.SetScopedForeground();

        if(IsUpdated || token.IsCancellationRequested) {
            return;
        }

        var path = paths.GenerateNewBackupVersionDirectory();
        Directory.CreateDirectory(path);
        var filePath = Path.Combine(path, Source.Name);
        await Source.CopyToAsync(filePath);

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
            .GetDirectories(paths.BackupsDirectory, "*", SearchOption.TopDirectoryOnly)
            .Select(x => Directory.GetCreationTimeUtc(x))
            .DefaultIfEmpty()
            .Max();

        if(latestBackup == default) {
            return false;
        }

        var latestChange = Source.LastWriteTimeUtc;
        return latestBackup > latestChange;

    }


    private bool isDisposed = false;
    public void Dispose() {
        if(isDisposed) {
            return;
        }
        isDisposed = true;

        locker.Dispose();
        GC.SuppressFinalize(this);
    }

}
