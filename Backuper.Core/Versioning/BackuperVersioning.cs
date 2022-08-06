using Backuper.Core.Services;
using Backuper.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Backuper.Core.Versioning; 

internal class BackuperVersioning : IBackuperVersioning {

    private readonly IPathsBuilderService _pathsBuilderService;
    private readonly string _mainBackupersDirectory;
    private DirectoryInfo _backupsDirectory;
    private DirectoryInfo _binDirectory;
    private string _backuperName;

    public BackuperVersioning(string mainDirectory, string backuperName, IPathsBuilderService pathsBuilderService) {
        _pathsBuilderService = pathsBuilderService;
        _mainBackupersDirectory = mainDirectory;
        _backuperName = backuperName;
        SetDirectories();
    }

    [MemberNotNull(nameof(_backupsDirectory), nameof(_binDirectory))]
    private void SetDirectories() {
        _backupsDirectory = new(GetBackuperDirectory(_backuperName));
        _binDirectory = new(GetBinDirectory(_backuperName));
    }

    //todo - extract these two methods to a class/interface
    private string GetBackuperDirectory(string name) => Path.Combine(_mainBackupersDirectory, "Backups", name);
    private string GetBinDirectory(string name) => Path.Combine(_mainBackupersDirectory, "Bin", name);

    public string GenerateNewBackupVersionDirectory() {
        return _pathsBuilderService.GenerateNewBackupVersionDirectory(_backupsDirectory.FullName);
    }

    public async Task MigrateTo(string newName) {
        DirectoryInfo newDir = new(GetBackuperDirectory(newName));

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
