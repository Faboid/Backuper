using Backuper.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Backuper.UI.WPF.ViewModels;

public class BackuperResultViewModel : ViewModelBase {

    private readonly IBackuper _backuper;

    public string Name => _backuper.Name;
    public string Source => _backuper.SourcePath;
    public int MaxVersions => _backuper.MaxVersions;

    private BackupingStatus _status = BackupingStatus.Starting;
    public BackupingStatus Status { 
        get => _status; 
        set => SetAndRaise(nameof(Status), ref _status, value); 
    }

    public BackuperResultViewModel(IBackuper backuper) {
        _backuper = backuper;
    }

    internal async Task Backup(CancellationToken cancellationToken = default) {

        try {
            
            var task = _backuper.BackupAsync(cancellationToken);
            Status = BackupingStatus.Backuping;
            var result = await task;
            Status = result switch {
                BackupResponseCode.Success => BackupingStatus.Success,
                BackupResponseCode.AlreadyUpdated => BackupingStatus.AlreadyUpdated,
                _ => BackupingStatus.Failed,
            };

        } catch (Exception) {
            Status = BackupingStatus.Failed;
            //todo - log
        }
    }

    public enum BackupingStatus {
        Starting,
        Backuping,
        AlreadyUpdated,
        Success,
        Failed,
    }

}