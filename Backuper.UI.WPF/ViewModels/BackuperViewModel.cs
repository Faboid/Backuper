using Backuper.Core;
using Backuper.UI.WPF.Commands;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Backuper.UI.WPF.ViewModels;

public class BackuperViewModel : ViewModelBase {

    private readonly INotificationService _notificationService;
    private readonly IBackuper _backuper;
    private readonly BackuperStore _backuperStore;

    public bool Updated => _backuper.IsUpdated();
    public bool UpdateOnBoot => _backuper.UpdateOnBoot;
    public string MaxVersions => _backuper.MaxVersions.ToString();
    public string Name => _backuper.Name;
    public string SourcePath => _backuper.SourcePath;

    public ICommand BackupCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    public BackuperViewModel(BackuperStore backuperStore, IBackuper backuper, INotificationService notificationService, NavigationService<EditBackuperViewModel> navigatorToEditBackuperViewModel) {
        _backuper = backuper;
        _backuperStore = backuperStore;
        _notificationService = notificationService;
        var busyService = new BusyService();
        BackupCommand = new AsyncRelayCommand(Backup, busyService);
        EditCommand = new NavigateCommand<EditBackuperViewModel>(navigatorToEditBackuperViewModel, busyService);
        DeleteCommand = new AsyncRelayCommand(Delete, busyService);
    }

    private async Task Backup() {

        var response = await _backuper.BackupAsync();
        OnPropertyChanged(nameof(Updated));

        var message = response switch {
            BackupResponseCode.Success => $"{Name} has been backed up successfully.",
            BackupResponseCode.AlreadyUpdated => $"{Name} is already up to date.",
            BackupResponseCode.Cancelled => $"{Name}'s backup has been stopped.",
            _ => $"{Name}'s backup has failed for an unknown reason.",
        };

        _notificationService.Send(message);

    }

    private async Task Delete() {

        var name = Name;
        var result = await _backuperStore.DeleteBackuperAsync(name);

        var message = result switch {
            DeleteBackuperResponse.Success => $"The backuper {name} has been deleted successfully.",
            DeleteBackuperResponse.BackuperNotFound => $"The backuper {name} does not exist.",
            DeleteBackuperResponse.NameIsNullOrWhiteSpace => "The given name cannot be empty.",
            _ => "There has been an unknown error.",
        };

        _notificationService.Send(message);

    }

}