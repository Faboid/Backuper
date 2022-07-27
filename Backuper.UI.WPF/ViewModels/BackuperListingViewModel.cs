using Backuper.UI.WPF.Commands;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Backuper.UI.WPF.ViewModels {
    public class BackuperListingViewModel : ViewModelBase {

        private readonly ObservableCollection<BackuperViewModel> _backupers;

        public IEnumerable<BackuperViewModel> Backupers => _backupers;

        public ICommand? ChangeBackupPathCommand { get; }
        public ICommand? ToggleAutomaticBackupsCommand { get; }
        public ICommand? CreateBackuperCommand { get; }
        public ICommand? BackupAllCommand { get; }
        private readonly BackuperStore _backuperStore;

        public BackuperListingViewModel(BackuperStore backuperStore, NavigationService<CreateBackuperViewModel> navigatorToCreateBackuperViewModel) {
            _backuperStore = backuperStore;
            CreateBackuperCommand = new NavigateCommand<CreateBackuperViewModel>(navigatorToCreateBackuperViewModel);
            _backupers = new();

            UpdateBackupers();
        }

        private void UpdateBackupers() {
            _backupers.Clear();
            foreach(var backuper in _backuperStore.Backupers) {
                var backuperViewModel = new BackuperViewModel(backuper);
                _backupers.Add(backuperViewModel);
            }
        }
    }
}
