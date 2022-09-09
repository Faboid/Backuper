using Backuper.Core;
using Backuper.Extensions;
using Backuper.UI.WPF.Commands;
using Backuper.UI.WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Backuper.UI.WPF.ViewModels;

public class BackupingResultsViewModel : ViewModelBase {

    private readonly ICollectionView _collectionView;
    private readonly ObservableCollection<BackuperResultViewModel> _backuperResults = new();
    
    public IEnumerable<BackuperResultViewModel> Backupers => _backuperResults;

    private string _search = "";
    public string Search {
        get => _search;
        set {
            SetAndRaise(nameof(Search), ref _search, value);
            _collectionView.Refresh();
        }
    }

    public ICommand HomeCommand { get; }

    public BackupingResultsViewModel(ICollection<IBackuper> backupers, NavigationService<BackuperListingViewModel> navigationServiceToListingViewModel) {
        HomeCommand = new NavigateCommand<BackuperListingViewModel>(navigationServiceToListingViewModel);
        
        foreach(var backuper in backupers) {
            _backuperResults.Add(new BackuperResultViewModel(backuper));
        }

        _collectionView = CollectionViewSource.GetDefaultView(_backuperResults);
        _collectionView.Filter = SearchFilter;
    }

    private async Task ExecuteBackups(CancellationToken cancellationToken = default) {
        await _backuperResults
            .AsParallel()
            .ForEachAsync(x => x.Backup(cancellationToken));
    }

    private bool SearchFilter(object obj) {

        if(obj is BackuperResultViewModel vm) {
            return vm.Name.Contains(Search, StringComparison.InvariantCultureIgnoreCase);
        }

        return false;
    }

}

public class BackuperResultViewModel : ViewModelBase {

    private readonly IBackuper _backuper;

    public string Name => _backuper.Name;
    public string Source => _backuper.SourcePath;
    public int MaxVersions => _backuper.MaxVersions;

    private BackupingStatus _status = BackupingStatus.Starting;
    public BackupingStatus Status { 
        get => Status; 
        set => SetAndRaise(nameof(Status), ref _status, value); 
    }

    public BackuperResultViewModel(IBackuper backuper) {
        _backuper = backuper;
    }

    internal async Task Backup(CancellationToken cancellationToken = default) {

        var task = _backuper.BackupAsync(cancellationToken);
        Status = BackupingStatus.Backuping;
        var result = await task;
        Status = result switch {
            BackupResponseCode.Success => BackupingStatus.Success,
            BackupResponseCode.AlreadyUpdated => BackupingStatus.AlreadyUpdated,
            _ => BackupingStatus.Failed,
        };

    }

    public enum BackupingStatus {
        Starting,
        Backuping,
        AlreadyUpdated,
        Success,
        Failed,
    }

}