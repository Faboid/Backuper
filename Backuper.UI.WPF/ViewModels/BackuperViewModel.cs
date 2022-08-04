using Backuper.Core;
using Backuper.Core.Models;
using Backuper.UI.WPF.Commands;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Backuper.UI.WPF.ViewModels; 

public class BackuperViewModel : ViewModelBase {

    private readonly IBackuper _backuper;
    private readonly BackuperStore _backuperStore;

    public bool Updated => _backuper.IsUpdated;
    public bool UpdateOnBoot => _backuper.Info.UpdateOnBoot;
    public string MaxVersions => _backuper.Info.MaxVersions.ToString();
    public string Name => _backuper.Info.Name;
    public string SourcePath => _backuper.Info.SourcePath;

    public ICommand? BackupCommand { get; }
    public ICommand? EditCommand { get; }
    public ICommand DeleteCommand { get; }

    public BackuperViewModel(BackuperStore backuperStore, IBackuper backuper, NavigationService<EditBackuperViewModel> navigatorToEditBackuperViewModel) {
        _backuper = backuper;
        _backuperStore = backuperStore;
        BackupCommand = new AsyncRelayCommand(Backup);
        EditCommand = new NavigateCommand<EditBackuperViewModel>(navigatorToEditBackuperViewModel);
        DeleteCommand = new AsyncRelayCommand(Delete);
    }

    private async Task Backup() {

        await _backuper.StartBackupAsync();
        OnPropertyChanged(nameof(Updated));

        MessageBox.Show($"{Name} has been backed up successfully.");

    }

    private async Task Delete() {

        var name = Name;
        var result = await _backuperStore.DeleteBackuperAsync(name);

        var message = result switch {
            Core.Saves.DeleteBackuperCode.Success => $"The backuper {name} has been deleted successfully.",
            Core.Saves.DeleteBackuperCode.BackuperDoesNotExist => $"The backuper {name} does not exist.",
            _ => "There has been an unknown error.",
        };

        MessageBox.Show(message);

    }

}