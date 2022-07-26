using Backuper.Core;
using Backuper.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Backuper.UI.WPF.ViewModels; 

public class BackuperViewModel : ViewModelBase {

    private readonly IBackuper _backuper;

    public bool Updated => _backuper.IsUpdated;
    public bool UpdateOnBoot => _backuper.Info.UpdateOnBoot;
    public string MaxVersions => _backuper.Info.MaxVersions.ToString();
    public string Name => _backuper.Info.Name;
    public string SourcePath => _backuper.Info.SourcePath;

    public ICommand? BackupCommand { get; }
    public ICommand? EditCommand { get; }
    public ICommand? DeleteCommand { get; }

    public BackuperViewModel(IBackuper backuper) {
        _backuper = backuper;
    }

    public BackuperViewModel(string name, string source, int maxVersions, bool updateOnBoot) {
        var info = new BackuperInfo(name, source, maxVersions, updateOnBoot);
        BackuperFactory factory = new();
        _backuper = factory.CreateBackuper(info);
    }

}

/// <summary>
/// Temporary class that will be used to test the UI's functionality.
/// </summary>
internal class BackuperMock : IBackuper {

    public BackuperMock(BackuperInfo info) {
        Info = info;
    }

    public BackuperInfo Info { get; }
    public bool IsUpdated { get; } = true;

    public Task BinBackupsAsync(CancellationToken token) {
        throw new NotImplementedException();
    }

    public void Dispose() {
        throw new NotImplementedException();
    }

    public Task EraseBackupsAsync(CancellationToken token) {
        throw new NotImplementedException();
    }

    public Task StartBackupAsync(CancellationToken token) {
        throw new NotImplementedException();
    }
}