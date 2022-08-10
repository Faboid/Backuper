using Backuper.Core.Services;
using Backuper.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Backuper.Core.Versioning; 

internal class BackuperVersioning : IBackuperVersioning {

    private readonly IPathsBuilderService _pathsBuilderService;
    private DirectoryInfo _backupsDirectory;
    private DirectoryInfo _binDirectory;
    private string _backuperName;

    public BackuperVersioning(string backuperName, IPathsBuilderService pathsBuilderService) {
        _pathsBuilderService = pathsBuilderService;
        _backuperName = backuperName;
        SetDirectories();
    }

    [MemberNotNull(nameof(_backupsDirectory), nameof(_binDirectory))]
    private void SetDirectories() {
        _backupsDirectory = new(_pathsBuilderService.GetBackuperDirectory(_backuperName));
        _binDirectory = new(_pathsBuilderService.GetBinDirectory(_backuperName));
    }

    public string GenerateNewBackupVersionDirectory() {
        return _pathsBuilderService.GenerateNewBackupVersionDirectory(_backupsDirectory.FullName);
    }

    public async Task MigrateTo(string newName) {
        DirectoryInfo newDir = new(_pathsBuilderService.GetBackuperDirectory(newName));

        await _backupsDirectory.CopyToAsync(newDir.FullName);
        _backupsDirectory.Delete(true);

        _backuperName = newName;
        SetDirectories();
    }

    public async Task Bin() {
        await _backupsDirectory.CopyToAsync(_binDirectory.FullName).ConfigureAwait(false);
        _backupsDirectory.Delete(true);
    }

    public DateTime GetLastBackupTimeUTC() {
        return _backupsDirectory
            .EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
            .Select(x => x.CreationTimeUtc)
            .DefaultIfEmpty()
            .Max();
    }

    public void DeleteExtraVersions(int maxVersions) {
        _backupsDirectory.EnumerateDirectories()
            .OrderByDescending(x => x.CreationTimeUtc)
            .Skip(maxVersions)
            .ForEach(x => x.Delete(true));
    }
}
