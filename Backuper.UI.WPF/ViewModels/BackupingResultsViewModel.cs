using Backuper.Core;
using Backuper.Extensions;
using Backuper.UI.WPF.Commands;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
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
    private readonly ObservableCollection<BackuperResultViewModel> _backuperResults;
    
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

    private ICommand LoadBackupersCommand { get; }
    private ICommand ExecuteBackupsCommand { get; }

    private BackupingResultsViewModel(BackuperStore backuperStore, INotificationService notificationService, 
                                      NavigationService<BackuperListingViewModel> navigationServiceToListingViewModel, 
                                      CancellationToken cancellationToken = default) {
        _backuperResults = new();
        HomeCommand = new NavigateCommand<BackuperListingViewModel>(navigationServiceToListingViewModel);
        LoadBackupersCommand = new LoadBackupersCommand(backuperStore, notificationService, Load);
        ExecuteBackupsCommand = new AsyncRelayCommand(() => ExecuteBackups(cancellationToken));
        _collectionView = CollectionViewSource.GetDefaultView(_backuperResults);
        _collectionView.Filter = SearchFilter;
    }

    public static BackupingResultsViewModel LoadViewModel(BackuperStore backuperStore, INotificationService notificationService, 
                                                          NavigationService<BackuperListingViewModel> navigationServiceToListingViewModel, 
                                                          CancellationToken cancellationToken = default) {
        var vm = new BackupingResultsViewModel(backuperStore, notificationService, navigationServiceToListingViewModel);
        vm.LoadBackupersCommand.Execute(null);
        vm.ExecuteBackupsCommand.Execute(null);
        return vm;
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

    private void Load(IEnumerable<IBackuper> backupers) {
        _backuperResults.Clear();
        foreach(var backuper in backupers) {
            _backuperResults.Add(new BackuperResultViewModel(backuper));
        }
    }

}