using Backuper.Core;
using Backuper.UI.WPF.Commands;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Backuper.UI.WPF.ViewModels {
    public class BackuperListingViewModel : ViewModelBase {

        private readonly BackuperStore _backuperStore;

        private readonly ObservableCollection<BackuperViewModel> _backupers;
        public IEnumerable<BackuperViewModel> Backupers => _backupers;

        public ICommand? ChangeBackupPathCommand { get; }
        public ICommand? ToggleAutomaticBackupsCommand { get; }
        public ICommand? CreateBackuperCommand { get; }
        public ICommand? BackupAllCommand { get; }

        private ICommand LoadBackupersCommand { get; }

        private BackuperListingViewModel(BackuperStore backuperStore, NavigationService<CreateBackuperViewModel> navigatorToCreateBackuperViewModel) {
            LoadBackupersCommand = new LoadReservationsCommand(backuperStore, UpdateBackupers);
            CreateBackuperCommand = new NavigateCommand<CreateBackuperViewModel>(navigatorToCreateBackuperViewModel);
            _backupers = new();
            _backuperStore = backuperStore;
            _backuperStore.BackupersChanged += RefreshBackupers;
        }

        public static BackuperListingViewModel LoadViewModel(BackuperStore backuperStore, NavigationService<CreateBackuperViewModel> navigatorToCreateBackuperViewModel) {
            BackuperListingViewModel vm = new(backuperStore, navigatorToCreateBackuperViewModel);
            vm.LoadBackupersCommand.Execute(null);
            return vm;
        }

        private void RefreshBackupers() {
            LoadBackupersCommand?.Execute(null);
        }

        private void UpdateBackupers(IEnumerable<IBackuper> backupers) {
            _backupers.Clear();
            foreach(var backuper in backupers) {
                var backuperViewModel = new BackuperViewModel(_backuperStore, backuper);
                _backupers.Add(backuperViewModel);
            }
        }
    }
}
