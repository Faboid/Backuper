using Backuper.Core;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backuper.UI.WPF.Commands;
public class LoadBackupersCommand : AsyncCommandBase {

    private readonly INotificationService _notificationService;
    private readonly BackuperStore _backuperStore;
    private readonly Action<IEnumerable<IBackuper>> loadBackupers;

    public LoadBackupersCommand(BackuperStore backuperStore, INotificationService notificationService, Action<IEnumerable<IBackuper>> loadBackupers) {
        _backuperStore = backuperStore;
        this.loadBackupers = loadBackupers;
        _notificationService = notificationService;
    }

    protected override async Task ExecuteAsync(object? parameter) {

        try {

            await _backuperStore.Load();
            loadBackupers?.Invoke(_backuperStore.Backupers);

        } catch(Exception) {

            _notificationService.Send("Failed to load backupers.", "Error");
            throw;
        }

    }
}
