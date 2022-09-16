using Backuper.Core;
using Backuper.UI.WPF.Commands;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace Backuper.UI.WPF.ViewModels;

public class BackuperListingViewModel : ViewModelBase {

    private readonly BackuperStore _backuperStore;
    private readonly ICollectionView _backupersCollectionView;
    private readonly Func<IBackuper, BackuperViewModel> _createBackuperViewModel;

    private readonly ObservableCollection<BackuperViewModel> _backupers;
    public IEnumerable<BackuperViewModel> Backupers => _backupers;

    private string _search = "";
    public string Search {
        get => _search;
        set {
            SetAndRaise(nameof(Search), ref _search, value);
            _backupersCollectionView.Refresh();
        }
    }

    public ICommand SettingsCommand { get; }
    public ICommand CreateBackuperCommand { get; }
    public ICommand BackupAllCommand { get; }

    private ICommand LoadBackupersCommand { get; }
    
    private BackuperListingViewModel(BackuperStore backuperStore, INotificationService notificationService,
                                    NavigationService<CreateBackuperViewModel> navigatorToCreateBackuperViewModel,
                                    NavigationService<BackupingResultsViewModel> navigatorToBackupingResultsViewModel,
                                    NavigationService<SettingsViewModel> navigatorToSettingsViewModel,
                                    Func<IBackuper, BackuperViewModel> createBackuperViewModel
                                    ) {
        _createBackuperViewModel = createBackuperViewModel;
        LoadBackupersCommand = new LoadBackupersCommand(backuperStore, notificationService, UpdateBackupers);
        CreateBackuperCommand = new NavigateCommand<CreateBackuperViewModel>(navigatorToCreateBackuperViewModel);
        BackupAllCommand = new NavigateCommand<BackupingResultsViewModel>(navigatorToBackupingResultsViewModel);
        SettingsCommand = new NavigateCommand<SettingsViewModel>(navigatorToSettingsViewModel);
        _backupers = new();
        _backupersCollectionView = CollectionViewSource.GetDefaultView(_backupers);
        _backupersCollectionView.Filter = BackupersFilter;

        _backuperStore = backuperStore;
        _backuperStore.BackupersChanged += RefreshBackupers;
    }

    public static BackuperListingViewModel LoadViewModel(BackuperStore backuperStore, INotificationService notificationService,
                                                        NavigationService<CreateBackuperViewModel> navigatorToCreateBackuperViewModel,
                                                        NavigationService<BackupingResultsViewModel> navigatorToBackupingResultsViewModel,
                                                        NavigationService<SettingsViewModel> navigatorToSettingsViewModel,
                                                        Func<IBackuper, BackuperViewModel> createBackuperViewModel) {

        BackuperListingViewModel vm = new(backuperStore, notificationService, 
                navigatorToCreateBackuperViewModel, 
                navigatorToBackupingResultsViewModel, 
                navigatorToSettingsViewModel, 
                createBackuperViewModel);
        vm.LoadBackupersCommand.Execute(null);
        return vm;
    }

    private void RefreshBackupers() {
        LoadBackupersCommand?.Execute(null);
    }

    private bool BackupersFilter(object obj) {

        if(obj is BackuperViewModel vm) {
            return vm.Name.Contains(Search, StringComparison.InvariantCultureIgnoreCase);
        }

        return false;
    }

    private void UpdateBackupers(IEnumerable<IBackuper> backupers) {
        _backupers.Clear();
        foreach(var backuper in backupers) {
            var backuperViewModel = _createBackuperViewModel.Invoke(backuper);
            _backupers.Add(backuperViewModel);
        }
    }
}
