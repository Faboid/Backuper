using Backuper.Core.Rewrite;
using Backuper.UI.WPF.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Backuper.UI.WPF.Commands; 
public class LoadBackupersCommand : AsyncCommandBase {

    private readonly BackuperStore _backuperStore;
    private readonly Action<IEnumerable<IBackuper>> loadBackupers;

    public LoadBackupersCommand(BackuperStore backuperStore, Action<IEnumerable<IBackuper>> loadBackupers) {
        _backuperStore = backuperStore;
        this.loadBackupers = loadBackupers;
    }

    protected override async Task ExecuteAsync(object? parameter) {

        try {

            await _backuperStore.Load();
            loadBackupers?.Invoke(_backuperStore.Backupers);

        } catch(Exception) {

            MessageBox.Show("Failed to load backupers.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            throw;
        }

    }
}
