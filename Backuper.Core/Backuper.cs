﻿using Backuper.Core.Models;
using Backuper.Core.Saves;
using Backuper.Core.Services;
using Backuper.Core.Validation;
using Backuper.Core.Versioning;
using Backuper.Utils;
using Microsoft.Extensions.Logging;

namespace Backuper.Core;

public class Backuper : IBackuper {

    private readonly ILogger<IBackuper>? _logger;
    private readonly IBackuperService _backuperService;
    private readonly IBackuperConnection _connection;
    private readonly IBackuperVersioning _versioning;
    private readonly IBackuperValidator _validator;
    private readonly Locker _locker = new();

    private BackuperInfo _info;
    public string Name => _info.Name;
    public string SourcePath => _info.SourcePath;
    public int MaxVersions => _info.MaxVersions;

    internal Backuper(
                BackuperInfo info,
                IBackuperService backuperService,
                IBackuperConnection connection,
                IBackuperVersioning versioning,
                IBackuperValidator validator,
                ILogger<IBackuper>? logger = null) {

        _info = new BackuperInfo(info.Name, info.SourcePath, info.MaxVersions);
        _backuperService = backuperService;
        _connection = connection;
        _versioning = versioning;
        _validator = validator;
        _logger = logger;
    }

    public async Task<BackupResponseCode> BackupAsync(CancellationToken token = default) {
        using var lockd = await _locker.GetLockAsync(CancellationToken.None);
        if(IsUpdated()) {
            return BackupResponseCode.AlreadyUpdated;
        }

        if(token.IsCancellationRequested) {
            return BackupResponseCode.Cancelled;
        }

        _logger?.LogInformation("Beginning backup of {Name}", Name);

        try {
            using var threadHandler = ThreadsHandler.SetScopedForeground();
            var newVersionPath = _versioning.GenerateNewBackupVersionDirectory();
            
            var result = await _backuperService.BackupAsync(newVersionPath, token);
            if(result != BackupResult.Success) {
                _logger?.LogInformation("Failed to backup {Name}. Reason: {Reason}", Name, result.ToString());
                return result switch {
                    BackupResult.Hibernating => BackupResponseCode.Hibernating,
                    BackupResult.Corrupted => BackupResponseCode.Corrupted,
                    _ => BackupResponseCode.Failure,
                };
            }
            
            _versioning.DeleteExtraVersions(MaxVersions);
        } catch(Exception ex) {

            _logger?.LogError(ex, "Failed to backup {Name}", Name);
            return BackupResponseCode.Failure;

        }

        _logger?.LogInformation("Backed up {Name} successfully.", Name);
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

        if(newInfo.Name != Name && _connection.Exists(newInfo.Name)) {
            return EditBackuperResponseCode.NewNameIsOccupied;
        }

        using var threadHandler = ThreadsHandler.SetScopedForeground();

        if(newInfo.Name != Name) {
            await _versioning.MigrateTo(newInfo.Name);
        }

        await _connection.OverwriteAsync(Name, newInfo);
        _logger?.LogInformation(
            "{Name} has been modified. From {Name}, {MaxVersions}, it has been changed to {NewName}, {NewMaxVersions}.",
            Name, Name, MaxVersions, newInfo.Name, newInfo.MaxVersions
        );
        _info = new BackuperInfo(newInfo.Name, newInfo.SourcePath, newInfo.MaxVersions);
        return EditBackuperResponseCode.Success;
    }

    public async Task BinAsync() {
        _logger?.LogInformation("Beginning binning {Name}.", Name);
        using var lockd = await _locker.GetLockAsync();
        using var threadHandler = ThreadsHandler.SetScopedForeground();
        await _versioning.Bin();
        _connection.Delete(Name);
        _logger?.LogInformation("{Name} has been binned.", Name);
    }

    public bool IsUpdated() {
        var sourceTime = _backuperService.GetSourceLastWriteTimeUTC().Or(default);
        if(sourceTime == default) {
            //this happens only if the source is missing.
            //to get attention from the user, it will be defaulted to false
            return false;
        }

        return _versioning.GetLastBackupTimeUTC() >= sourceTime;
    }

    private bool _isDisposed = false;
    public void Dispose() {
        if(!_isDisposed) {
            _locker.Dispose();
            GC.SuppressFinalize(this);
        }
        _isDisposed = true;
    }
}

public enum BackupResponseCode {
    Unknown,
    Success,
    AlreadyUpdated,
    Cancelled,
    Failure,
    Hibernating,
    Corrupted
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