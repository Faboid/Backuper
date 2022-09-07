using Backuper.Core;
using Backuper.Extensions;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Backuper.UI.WPF.Commands;

public class BackupAllCommand : AsyncCommandBase {

    private readonly BackuperStore _backuperStore;

    public BackupAllCommand(BackuperStore backuperStore, BusyService busyService) : base(busyService) {
        _backuperStore = backuperStore;
    }

    protected async override Task ExecuteAsync(object? parameter) {

        var result = await _backuperStore.Backupers
            .SelectAsync(x => x.BackupAsync())
            .ToListAsync();

        var successes = result.Where(x => x == BackupResponseCode.Success).Count();
        var failures = result.Where(x => x is BackupResponseCode.Failure or BackupResponseCode.Unknown).Count();
        var alreadyUpdated = result.Where(x => x == BackupResponseCode.AlreadyUpdated).Count();

        StringBuilder sb = new();
        sb.AppendLine($"Updated {successes} successfully.");
        sb.AppendLine($"{alreadyUpdated} were already updated.");
        sb.AppendLine($"{failures} have failed.");

        MessageBox.Show(sb.ToString(), "Backups Result");

    }

}