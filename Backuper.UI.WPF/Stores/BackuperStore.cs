using Backuper.Core;
using Backuper.Core.Models;
using Backuper.Core.Saves;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backuper.UI.WPF.Stores; 

public class BackuperStore {
    
    private readonly Dictionary<string, IBackuper> _backupers;
    private readonly IBackuperConnection _connection;
    private readonly BackuperFactory _factory;
    
    private Lazy<Task> _initializationTask;

    public event Action? BackupersChanged;
    public event Action<IBackuper>? BackuperCreated;
    public event Action<IBackuper>? BackuperDeleted;

    public IEnumerable<IBackuper> Backupers => _backupers.Values;

    public BackuperStore(BackuperFactory factory, IBackuperConnection backuperConnection) {
        _backupers = new();
        _initializationTask = new(Initialize);
        _connection = backuperConnection;
        _factory = factory;

        BackuperCreated += (a) => BackupersChanged?.Invoke();
        BackuperDeleted += (a) => BackupersChanged?.Invoke();
    }

    public async Task Load() {
        await _initializationTask.Value;
    }

    private async Task Initialize() {
        var infos = _connection.GetAllBackupersAsync();

        _backupers.Clear();
        await foreach(var info in infos) {
            var backuper = _factory.CreateBackuper(info);
            _backupers.Add(backuper.Info.Name, backuper);
        }
    }

    public bool BackuperExists(string name) {
        return _backupers.ContainsKey(name);
    }

    public async Task<CreateBackuperCode> CreateBackuperAsync(BackuperInfo info) {

        var result = await _connection.CreateBackuperAsync(info);

        if(result == CreateBackuperCode.Success) {

            var backuper = _factory.CreateBackuper(info);
            _backupers.Add(backuper.Info.Name, backuper);
            BackuperCreated?.Invoke(backuper);
        }

        return result;

    }

    public async Task<DeleteBackuperCode> DeleteBackuperAsync(string name) {

        if(string.IsNullOrWhiteSpace(name)) {
            return DeleteBackuperCode.NameNotValid;
        }

        if(!_backupers.TryGetValue(name, out var backuper)) {
            return DeleteBackuperCode.BackuperDoesNotExist;
        }

        await backuper.BinBackupsAsync();
        var result = _connection.DeleteBackuper(name);

        if(result == DeleteBackuperCode.Success) {

            _backupers.Remove(name);
            BackuperDeleted?.Invoke(backuper);
        }

        return result;
    }

    public async Task<UpdateBackuperCode> UpdateBackuperAsync(string name, string? newName, int newMaxVersions = 0, bool? newUpdateOnBoot = null) {

        if(!BackuperExists(name)) {
            return UpdateBackuperCode.BackuperDoesNotExist;
        }

        var result = await _connection.UpdateBackuperAsync(name, newName, newMaxVersions, newUpdateOnBoot);

        if(result == UpdateBackuperCode.Success) {

            var updatedName = string.IsNullOrWhiteSpace(newName) ? name : newName;
            //to avoid duplicating the logic on value validation, it's simpler to refresh the backuper's data.
            var updatedBackuper = await _connection.GetBackuperAsync(updatedName);
            _backupers.Remove(name);

            var info = updatedBackuper.Or(null!);

            if(info == null) {

                //if it fails to get the updated value, refresh the whole list.
                _initializationTask = new(Initialize);

            } else {

                _backupers.Add(info.Name, _factory.CreateBackuper(info));

            }
        }

        return result;
    }

}
