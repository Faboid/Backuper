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

    private readonly BusyService _busyService;
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

    public bool IsNotBackuping => !_busyService.IsBusy;

    public ICommand? SettingsCommand { get; }
    public ICommand? ChangeBackupPathCommand { get; }
    public ICommand? ToggleAutomaticBackupsCommand { get; }
    public ICommand CreateBackuperCommand { get; }
    public ICommand BackupAllCommand { get; }

    private ICommand LoadBackupersCommand { get; }

    private BackuperListingViewModel(BackuperStore backuperStore,
                                    NavigationService<CreateBackuperViewModel> navigatorToCreateBackuperViewModel,
                                    Func<IBackuper, BackuperViewModel> createBackuperViewModel
                                    ) {
        _busyService = new BusyService();
        _busyService.BusyChanged += () => OnPropertyChanged(nameof(IsNotBackuping));
        _createBackuperViewModel = createBackuperViewModel;
        LoadBackupersCommand = new LoadBackupersCommand(backuperStore, UpdateBackupers);
        CreateBackuperCommand = new NavigateCommand<CreateBackuperViewModel>(navigatorToCreateBackuperViewModel);
        BackupAllCommand = new BackupAllCommand(backuperStore, _busyService);
        _backupers = new();
        _backupersCollectionView = CollectionViewSource.GetDefaultView(_backupers);
        _backupersCollectionView.Filter = BackupersFilter;

        _backuperStore = backuperStore;
        _backuperStore.BackupersChanged += RefreshBackupers;
    }

    public static BackuperListingViewModel LoadViewModel(BackuperStore backuperStore,
                                                        NavigationService<CreateBackuperViewModel> navigatorToCreateBackuperViewModel,
                                                        Func<IBackuper, BackuperViewModel> createBackuperViewModel) {

        BackuperListingViewModel vm = new(backuperStore, navigatorToCreateBackuperViewModel, createBackuperViewModel);
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
