using Backuper.Core;
using Backuper.Core.Models;
using Backuper.Core.Saves;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backuper.UI.WPF.Stores; 

public class BackuperStore {
    
    private readonly Dictionary<string, IBackuper> _backupers;
    private readonly Lazy<Task> _initializationTask;
    private readonly IBackuperConnection _connection;
    private readonly BackuperFactory _factory;

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

}
