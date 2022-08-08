using Backuper.Core.Models;
using Backuper.Core.Rewrite;
using Backuper.Core.Services;
using Backuper.Core.Versioning;
using Backuper.Utils;

namespace Backuper.Core; 

public class Backuper {

    private readonly IBackuperService _backuperService;
    private readonly IBackuperConnection _connection;
    private readonly IBackuperVersioning _versioning;
    private readonly Locker _locker = new();

    private BackuperInfo _info;
    public string Name => _info.Name;
    public string SourcePath => _info.SourcePath;
    public int MaxVersions => _info.MaxVersions;
    public bool UpdateOnBoot => _info.UpdateOnBoot;

    internal Backuper(
                BackuperInfo info,
                IBackuperService backuperService,
                IBackuperConnection connection,
                IBackuperVersioning versioning
            ) {

        _info = info;
        _backuperService = backuperService;
        _connection = connection;
        _versioning = versioning;
    }

    public async Task<BackupResponseCode> BackupAsync(CancellationToken token = default) {
        using var lockd = await _locker.GetLockAsync(CancellationToken.None);
        if(IsUpdated()) {
            return BackupResponseCode.AlreadyUpdated;
        }

        if(token.IsCancellationRequested) {
            return BackupResponseCode.Cancelled;
        }

        var newVersionPath = _versioning.GenerateNewBackupVersionDirectory();
        await _backuperService.BackupAsync(newVersionPath, token);
        _versioning.DeleteExtraVersions(MaxVersions);

        return BackupResponseCode.Success;
    }

    public async Task<EditBackuperResponseCode> EditAsync(BackuperInfo newInfo) {
        using var lockd = await _locker.GetLockAsync(CancellationToken.None);

        var isValid = newInfo.IsValid();
        if(isValid != BackuperInfo.InfoValid.Valid) {

            return isValid switch {
                BackuperInfo.InfoValid.NameEmptyOrSpaces => EditBackuperResponseCode.NewNameIsEmptyOrWhiteSpaces,
                BackuperInfo.InfoValid.SourceDoesNotExist => EditBackuperResponseCode.NewSourceDoesNotExist,
                BackuperInfo.InfoValid.NegativeMaxVersions => EditBackuperResponseCode.NewMaxVersionsAreNegative,
                _ => EditBackuperResponseCode.Unknown
            };
        }

        if(_connection.Exists(newInfo.Name)) {
            return EditBackuperResponseCode.NewNameIsOccupied;
        }

        //not supporting changing source
        if(newInfo.SourcePath != SourcePath) {
            return EditBackuperResponseCode.SourceCannotBeChanged;
        }

        if(newInfo.Name != Name) {
            await _versioning.MigrateTo(newInfo.Name);
        }

        await _connection.OverwriteAsync(Name, newInfo);
        _info = newInfo;
        return EditBackuperResponseCode.Success;
    }

    public async Task BinAsync() {
        using var lockd = await _locker.GetLockAsync(CancellationToken.None);
        await _versioning.Bin();
        _connection.Delete(Name);
    }

    public bool IsUpdated() {
        return _versioning.GetLastBackupTimeUTC() >= _backuperService.GetSourceLastWriteTimeUTC();
    }

}

public enum BackupResponseCode {
    Unknown,
    Success,
    AlreadyUpdated,
    Cancelled,
    Failure
}

public enum EditBackuperResponseCode {
    Unknown,
    Success,
    NewNameIsEmptyOrWhiteSpaces,
    NewSourceDoesNotExist,
    NewMaxVersionsAreNegative,
    NewNameIsOccupied,
    SourceCannotBeChanged,
}