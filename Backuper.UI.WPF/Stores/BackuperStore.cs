using Backuper.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backuper.Core;

namespace Backuper.UI.WPF.Stores;

public class BackuperStore {

    private readonly Dictionary<string, IBackuper> _backupers;
    private readonly IBackuperFactory _backuperFactory;

    private Lazy<Task> _initializationTask;

    public event Action? BackupersChanged;
    public event Action<IBackuper>? BackuperCreated;
    public event Action<IBackuper>? BackuperDeleted;

    public IEnumerable<IBackuper> Backupers => _backupers.Values;

    public BackuperStore(IBackuperFactory factory) {
        _backupers = new();
        _initializationTask = new(Initialize);
        _backuperFactory = factory;

        BackuperCreated += (a) => BackupersChanged?.Invoke();
        BackuperDeleted += (a) => BackupersChanged?.Invoke();
    }

    public async Task Load() {
        await _initializationTask.Value;
    }

    private async Task Initialize() {
        var backupers = _backuperFactory.LoadBackupers();

        _backupers.Clear();
        await foreach(var backuper in backupers) {
            _backupers.Add(backuper.Name, backuper);
        }
    }

    public bool BackuperExists(string name) {
        return _backupers.ContainsKey(name);
    }

    public async Task<CreateBackuperResponse> CreateBackuperAsync(BackuperInfo info) {

        var result = await _backuperFactory.CreateBackuper(info);
        return result.Match(
            some => {
                _backupers.Add(some.Name, some);
                BackuperCreated?.Invoke(some);
                return CreateBackuperResponse.Success;
            },
            error => error switch {
                BackuperFactory.CreateBackuperFailureCode.NameIsOccupied => CreateBackuperResponse.NameIsOccupied,
                BackuperFactory.CreateBackuperFailureCode.NameIsEmpty => CreateBackuperResponse.NameIsEmpty,
                BackuperFactory.CreateBackuperFailureCode.NameHasIllegalCharacters => CreateBackuperResponse.NameHasIllegalCharacters,
                BackuperFactory.CreateBackuperFailureCode.SourceDoesNotExist => CreateBackuperResponse.SourceDoesNotExist,
                BackuperFactory.CreateBackuperFailureCode.SourceIsEmpty => CreateBackuperResponse.SourceIsEmpty,
                BackuperFactory.CreateBackuperFailureCode.ZeroOrNegativeMaxVersions => CreateBackuperResponse.ZeroOrNegativeMaxVersions,
                _ => CreateBackuperResponse.UnknownError,
            },
            () => CreateBackuperResponse.UnknownError
        );

    }

    public async Task<DeleteBackuperResponse> DeleteBackuperAsync(string name) {

        if(string.IsNullOrWhiteSpace(name)) {
            return DeleteBackuperResponse.NameIsNullOrWhiteSpace;
        }

        if(!_backupers.TryGetValue(name, out var backuper)) {
            return DeleteBackuperResponse.BackuperNotFound;
        }

        await backuper.BinAsync();
        _backupers.Remove(name);
        BackuperDeleted?.Invoke(backuper);

        return DeleteBackuperResponse.Success;
    }

    public async Task<UpdateBackuperResponse> UpdateBackuperAsync(string name, string? newName = null, int? newMaxVersions = null, bool? newUpdateOnBoot = null) {

        if(string.IsNullOrWhiteSpace(name)) {
            return UpdateBackuperResponse.NameIsNullOrWhiteSpace;
        }

        if(!_backupers.TryGetValue(name, out var backuper)) {
            return UpdateBackuperResponse.BackuperNotFound;
        }

        var infoName = newName ?? backuper.Name;
        var infoSource = backuper.SourcePath;
        var infoVersions = newMaxVersions ?? backuper.MaxVersions;
        var infoUpdateOnBoot = newUpdateOnBoot ?? backuper.UpdateOnBoot;

        var info = new BackuperInfo(infoName, infoSource, infoVersions, infoUpdateOnBoot);
        return await UpdateBackuperAsync(backuper, info);

    }


    private static async Task<UpdateBackuperResponse> UpdateBackuperAsync(IBackuper backuper, BackuperInfo info) {

        var result = await backuper.EditAsync(info);
        return result switch {
            EditBackuperResponseCode.Success => UpdateBackuperResponse.Success,
            EditBackuperResponseCode.NewMaxVersionsIsZeroOrNegative => UpdateBackuperResponse.NewMaxVersionsIsZeroOrNegative,
            EditBackuperResponseCode.NewNameIsOccupied => UpdateBackuperResponse.NewNameIsOccupied,
            EditBackuperResponseCode.NameContainsIllegalCharacters => UpdateBackuperResponse.NewNameContainsIllegalCharacters,
            _ => UpdateBackuperResponse.UnknownError,
        };

    }

}

//UI Responses
public enum CreateBackuperResponse {
    UnknownError,
    Success,
    Failure,
    NameIsOccupied,
    NameHasIllegalCharacters,
    NameIsEmpty,
    SourceDoesNotExist,
    SourceIsEmpty,
    ZeroOrNegativeMaxVersions,
}

public enum DeleteBackuperResponse {
    UnknownError,
    Success,
    Failure,
    NameIsNullOrWhiteSpace,
    BackuperNotFound,
}

public enum UpdateBackuperResponse {
    UnknownError,
    Success,
    Failure,
    BackuperNotFound,
    NewMaxVersionsIsZeroOrNegative,
    NewNameIsOccupied,
    NewNameContainsIllegalCharacters,
    NameIsNullOrWhiteSpace,
}