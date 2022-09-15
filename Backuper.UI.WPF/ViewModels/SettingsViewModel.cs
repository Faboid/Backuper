using Backuper.Core;
using Backuper.UI.WPF.Commands;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using Backuper.Utils;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Backuper.UI.WPF.ViewModels;

public class SettingsViewModel : ViewModelBase {

    private readonly PathsHandler _pathsHandler;
    private readonly Settings _settings;
    private readonly INotificationService _notificationService;

    private const string autoBootKey = "AutoBoot";

    public string CurrentBackupsFolder { get; private set; }

    private string _backupsFolder = "";
    public string BackupsFolder {
        get => _backupsFolder;
        set {
            SetAndRaise(nameof(BackupsFolder), ref _backupsFolder, value);
        }
    }

    private bool _autoBoot;
    public bool AutoBoot {
        get => _autoBoot;
        set {
            SetAndRaise(nameof(_autoBoot), ref _autoBoot, value);
            _settings.Set(autoBootKey, value.ToString());
        }
    }

    public ICommand ChangeBackupsFolder { get; init; }
    public ICommand OpenPathDialogCommand { get; }
    public ICommand HomeButton { get; }

    public SettingsViewModel(Settings settings, PathsHandler pathsHandler, INotificationService notificationService, NavigationStore navigationStore, NavigationService<BackuperListingViewModel> navigateToBackuperListingViewModel) {
        _settings = settings;
        _pathsHandler = pathsHandler;
        _notificationService = notificationService;
        CurrentBackupsFolder = _pathsHandler.GetBackupersDirectory();
        _autoBoot = bool.Parse(settings.Get(autoBootKey).Or("True")!);
        ChangeBackupsFolder = new AsyncRelayCommand(ChangeFolder);
        var navigateToSelf = new NavigationService<ViewModelBase>(navigationStore, () => this);
        OpenPathDialogCommand = new NavigateCommand<OpenPathDialogViewModel>(new(navigationStore, () => new(navigateToSelf, (s) => BackupsFolder = s)));
        HomeButton = new NavigateCommand<BackuperListingViewModel>(navigateToBackuperListingViewModel);
    }

    private async Task ChangeFolder() {

        var result = await _pathsHandler.SetBackupersDirectoryAsync(BackupsFolder);
        var message = result switch {
            PathsHandler.BackupersMigrationResult.Failure => "There has been an error.",
            PathsHandler.BackupersMigrationResult.Success => "The backups have been transferred successfully.",
            PathsHandler.BackupersMigrationResult.InvalidPath => "The given path is invalid.",
            PathsHandler.BackupersMigrationResult.TargetDirectoryIsNotEmpty => "The given path must point to an empty folder.",
            _ => "There has been an unknown error.",
        };

        _notificationService.Send(message);

    }

}