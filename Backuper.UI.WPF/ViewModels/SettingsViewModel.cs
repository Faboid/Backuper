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

    private string _currentBackupsFolder;
    public string CurrentBackupsFolder { get => _currentBackupsFolder; private set => SetAndRaise(nameof(CurrentBackupsFolder), ref _currentBackupsFolder, value); }

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
        }
    }

    public ICommand ResetBackupersDirectoryCommand { get; }
    public ICommand ApplyChangesCommand { get; }
    public ICommand OpenPathDialogCommand { get; }
    public ICommand HomeButtonCommand { get; }

    public SettingsViewModel(Settings settings, PathsHandler pathsHandler, INotificationService notificationService, NavigationStore navigationStore, NavigationService<BackuperListingViewModel> navigateToBackuperListingViewModel) {
        _settings = settings;
        _pathsHandler = pathsHandler;
        _notificationService = notificationService;
        _currentBackupsFolder = _pathsHandler.GetBackupersDirectory();
        _autoBoot = bool.Parse(settings.Get(autoBootKey).Or("True")!);
        ResetBackupersDirectoryCommand = new AsyncRelayCommand(ResetBackupersDirectory);
        ApplyChangesCommand = new AsyncRelayCommand(ApplyChanges);
        var navigateToSelf = new NavigationService<ViewModelBase>(navigationStore, () => this);
        OpenPathDialogCommand = new NavigateCommand<OpenPathDialogViewModel>(new(navigationStore, () => new(navigateToSelf, (s) => BackupsFolder = s)));
        HomeButtonCommand = new NavigateCommand<BackuperListingViewModel>(navigateToBackuperListingViewModel);
    }

    private async Task ResetBackupersDirectory() {

        var result = await _pathsHandler.ResetBackupersDirectory();
        var message = ConvertResultToMessage(result);
        _notificationService.Send(message);
        CurrentBackupsFolder = _pathsHandler.GetBackupersDirectory();

    }

    private async Task ApplyChanges() {

        var result = await _pathsHandler.SetBackupersDirectoryAsync(BackupsFolder);
        var message = ConvertResultToMessage(result);
        _notificationService.Send(message);

        //todo - use the AutoBoot class
        _settings.Set(autoBootKey, AutoBoot.ToString());

    }

    private static string ConvertResultToMessage(PathsHandler.BackupersMigrationResult result) => result switch {
            PathsHandler.BackupersMigrationResult.Failure => "There has been an error.",
            PathsHandler.BackupersMigrationResult.Success => "The backups have been transferred successfully.",
            PathsHandler.BackupersMigrationResult.InvalidPath => "The given path is invalid.",
            PathsHandler.BackupersMigrationResult.TargetDirectoryIsNotEmpty => "The given path must point to an empty folder.",
            _ => "There has been an unknown error.",
        };

}