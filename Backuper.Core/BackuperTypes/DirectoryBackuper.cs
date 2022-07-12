using Backuper.Core.Models;
using Backuper.Extensions;
using Backuper.Utils;
namespace Backuper.Core.BackuperTypes;

public class DirectoryBackuper : IBackuper {

    public DirectoryBackuper(BackuperInfo info, PathsBuilder pathsBuilder) {
        Info = info;
        this.pathsBuilder = pathsBuilder.Build(info.Name);
        IsUpdated = IsUpToDate();
        Source = new(info.SourcePath);
    }

    public DirectoryInfo Source { get; private set; }
    public BackuperInfo Info { get; init; }
    public bool IsUpdated { get; private set; }

    private readonly Paths pathsBuilder;

    //todo - test the methods below
    public Task BinBackupsAsync() {
        Directory.Move(pathsBuilder.BackupsDirectory, pathsBuilder.BinDirectory);
        return Task.CompletedTask;
    }

    public void EraseBackups() {
        Directory.Delete(pathsBuilder.BackupsDirectory, true);
    }

    public Task StartBackupAsync() {
        if(IsUpToDate()) {
            return Task.CompletedTask;
        }

        var path = pathsBuilder.GenerateNewBackupVersionDirectory();
        Directory.CreateDirectory(path);

        Dictionary<string, string> yo = new();

        //create all directories
        Source
            .EnumerateDirectories("*", SearchOption.AllDirectories)
            .Select(x => x.Name.Replace(Source.Name, path))
            .ForEach(x => Directory.CreateDirectory(x));

        //todo - actually make this async
        //copy all files
        Source
            .EnumerateFiles("*", SearchOption.AllDirectories)
            .Select(x => (File: x, NewPath: x.Name.Replace(Source.Name, path)))
            .ForEach(x => x.File.CopyTo(x.NewPath));

        //delete extra versions
        Directory.EnumerateDirectories(pathsBuilder.BackupsDirectory)
            .OrderByDescending(x => Directory.GetCreationTime(x))
            .Skip(Info.MaxVersions)
            .ForEach(x => Directory.Delete(x, true));

        IsUpdated = true;
        return Task.CompletedTask;
    }

    private bool IsUpToDate() {

        //backups are done and then never touched,
        //so it's best to get the creation time
        var latestBackup = Directory
            .GetDirectories(pathsBuilder.BackupsDirectory, "*", SearchOption.TopDirectoryOnly)
            .Select(x => Directory.GetCreationTimeUtc(x))
            .DefaultIfEmpty()
            .Max();

        if(latestBackup == default || Source.LastWriteTimeUtc > latestBackup) {
            return false;
        }

        //windows doesn't update the parents' folders' last write time
        //therefore, it's needed to go through all children directories
        var latestChange = Directory
            .GetDirectories(Source.FullName, "*", SearchOption.AllDirectories)
            .Select(x => Directory.GetLastWriteTimeUtc(x))
            .DefaultIfEmpty()
            .Max();

        return latestBackup > latestChange;

    }

}
