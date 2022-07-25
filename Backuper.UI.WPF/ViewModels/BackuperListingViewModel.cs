using Backuper.Core.Models;
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

        public BackuperListingViewModel() {
            _backupers = new();

            //temporary values to make sure the UI functions
            _backupers.Add(new(new BackuperMock(new BackuperInfo("someName", "path", 3, false))));
            _backupers.Add(new(new BackuperMock(new BackuperInfo("secondName", "pathHere", 1, true))));
            _backupers.Add(new(new BackuperMock(new BackuperInfo("anotherName", "newPath", 99, false))));
        }

    }
}
