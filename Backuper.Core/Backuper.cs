using Backuper.Core.Models;
using Backuper.Core.Rewrite;
using Backuper.Core.Services;
using Backuper.Core.Validation;
using Backuper.Core.Versioning;
using Backuper.Utils;

namespace Backuper.Core; 

public class Backuper : Rewrite.IBackuper {

    private readonly IBackuperService _backuperService;
    private readonly IBackuperConnection _connection;
    private readonly IBackuperVersioning _versioning;
    private readonly IBackuperValidator _validator;
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
                IBackuperVersioning versioning,
                IBackuperValidator validator
        ) {

        _info = new BackuperInfo(info.Name, info.SourcePath, info.MaxVersions, info.UpdateOnBoot);
        _backuperService = backuperService;
        _connection = connection;
        _versioning = versioning;
        _validator = validator;

        var valid = _validator.IsValid(info);
        if(valid != BackuperValid.Valid) {
            throw new ArgumentException($"The given backuper info contains a non-valid value. Error code: {valid}.", nameof(info));
        }
    }

    public async Task<BackupResponseCode> BackupAsync(CancellationToken token = default) {
        using var lockd = await _locker.GetLockAsync(CancellationToken.None);
        if(IsUpdated()) {
            return BackupResponseCode.AlreadyUpdated;
        }

        if(token.IsCancellationRequested) {
            return BackupResponseCode.Cancelled;
        }

        using var threadHandler = ThreadsHandler.SetScopedForeground();
        var newVersionPath = _versioning.GenerateNewBackupVersionDirectory();
        await _backuperService.BackupAsync(newVersionPath, token);
        _versioning.DeleteExtraVersions(MaxVersions);

        return BackupResponseCode.Success;
    }

    public async Task<EditBackuperResponseCode> EditAsync(BackuperInfo newInfo) {
        using var lockd = await _locker.GetLockAsync(CancellationToken.None);

        if(newInfo == null) {
            return EditBackuperResponseCode.GivenInfoIsNull;
        }

        //not supporting changing source
        if(newInfo.SourcePath != SourcePath) {
            return EditBackuperResponseCode.SourceCannotBeChanged;
        }

        var isValid = _validator.IsValid(newInfo);
        if(isValid != BackuperValid.Valid) {

            return isValid switch {
                BackuperValid.NameIsEmpty => EditBackuperResponseCode.NewNameIsEmptyOrWhiteSpaces,
                BackuperValid.NameHasIllegalCharacters => EditBackuperResponseCode.NameContainsIllegalCharacters,
                //BackuperValid.SourceIsEmpty => EditBackuperResponseCode.SourceIsEmptyOrWhiteSpaces,
                //BackuperValid.SourceDoesNotExist => EditBackuperResponseCode.NewSourceDoesNotExist,
                BackuperValid.ZeroOrNegativeMaxVersions => EditBackuperResponseCode.NewMaxVersionsIsZeroOrNegative,
                _ => EditBackuperResponseCode.Unknown
            };
        }

        if(_connection.Exists(newInfo.Name)) {
            return EditBackuperResponseCode.NewNameIsOccupied;
        }

        using var threadHandler = ThreadsHandler.SetScopedForeground();

        if(newInfo.Name != Name) {
            await _versioning.MigrateTo(newInfo.Name);
        }

        await _connection.OverwriteAsync(Name, newInfo);
        _info = new BackuperInfo(newInfo.Name, newInfo.SourcePath, newInfo.MaxVersions, newInfo.UpdateOnBoot);
        return EditBackuperResponseCode.Success;
    }

    public async Task BinAsync() {
        using var lockd = await _locker.GetLockAsync();
        using var threadHandler = ThreadsHandler.SetScopedForeground();
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
    NewMaxVersionsIsZeroOrNegative,
    NewNameIsOccupied,
    SourceCannotBeChanged,
    NameContainsIllegalCharacters,
    SourceIsEmptyOrWhiteSpaces,
    GivenInfoIsNull,
}