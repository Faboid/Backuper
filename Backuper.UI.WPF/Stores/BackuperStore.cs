using Backuper.Core;
using Backuper.Core.Models;
using Backuper.Core.Saves;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backuper.UI.WPF.Stores; 

public class BackuperStore {
    
    private readonly List<IBackuper> _backupers;
    private readonly Lazy<Task> _initializationTask;
    private readonly IBackuperConnection _connection;
    private readonly BackuperFactory _factory;

    public Action<IBackuper>? OnBackuperCreated;
    public IEnumerable<IBackuper> Backupers => _backupers;

    public BackuperStore(BackuperFactory factory, IBackuperConnection backuperConnection) {
        _backupers = new List<IBackuper>();
        _initializationTask = new(Initialize);
        _connection = backuperConnection;
        _factory = factory;
    }

    public async Task Load() {
        await _initializationTask.Value;
    }

    private async Task Initialize() {
        var infos = _connection.GetAllBackupersAsync();

        _backupers.Clear();
        await foreach(var info in infos) {
            var backuper = _factory.CreateBackuper(info);
            _backupers.Add(backuper);
        }
    }

    public async Task<CreateBackuperCode> CreateBackuperAsync(BackuperInfo info) {

        var result = await _connection.CreateBackuperAsync(info);

        if(result == CreateBackuperCode.Success) {

            var backuper = _factory.CreateBackuper(info);
            _backupers.Add(backuper);
            OnBackuperCreated?.Invoke(backuper);
        }

        return result;

    }

}
