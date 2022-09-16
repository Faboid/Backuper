using Backuper.Core;
using Backuper.Core.Services;
using Backuper.UI.WPF.Commands;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using Backuper.Utils;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Backuper.UI.WPF.ViewModels;

public class SettingsViewModel : ViewModelBase {

    private readonly PathsHandler _pathsHandler;
    private readonly AutoBootService _autoBootService;
    private readonly Settings _settings;
    private readonly INotificationService _notificationService;

    private const string autoBootKey = "AutoBoot";

    private string _currentBackupsFolder;
    public string CurrentBackupsFolder { get => _currentBackupsFolder; private set => SetAndRaise(nameof(CurrentBackupsFolder), ref _currentBackupsFolder, value); }

    private string _backupsFolder = "";
    public string BackupsFolder { get => _backupsFolder; set => SetAndRaise(nameof(BackupsFolder), ref _backupsFolder, value); }

    private bool _autoBoot;
    public bool AutoBoot {
        get => _autoBoot;
        set {
            SetAndRaise(nameof(_autoBoot), ref _autoBoot, value);

            _autoBootService.Set(AutoBoot);
            _settings.Set(autoBootKey, AutoBoot.ToString());
            var message = value switch {
                true => "The autoboot has been turned on.",
                false => "The autoboot has been deactivated. Warning: keeping it off means the app must be manually opened to execute backups."
            };
            _notificationService.Send(message);
        }
    }

    public ICommand ChangeBackupersPathCommand { get; }
    public ICommand ResetToDefaultCommand { get; }
    public ICommand OpenPathDialogCommand { get; }
    public ICommand HomeButtonCommand { get; }

    public SettingsViewModel(Settings settings, PathsHandler pathsHandler, AutoBootService autoBootService, INotificationService notificationService, NavigationStore navigationStore, NavigationService<BackuperListingViewModel> navigateToBackuperListingViewModel) {
        _settings = settings;
        _pathsHandler = pathsHandler;
        _autoBootService = autoBootService;
        _notificationService = notificationService;
        _currentBackupsFolder = _pathsHandler.GetBackupersDirectory();
        _autoBoot = autoBootService.Get();
        ChangeBackupersPathCommand = new AsyncRelayCommand(ChangePath);
        ResetToDefaultCommand = new AsyncRelayCommand(ResetToDefault);
        var navigateToSelf = new NavigationService<ViewModelBase>(navigationStore, () => this);
        OpenPathDialogCommand = new NavigateCommand<OpenPathDialogViewModel>(new(navigationStore, () => new(navigateToSelf, (s) => BackupsFolder = s)));
        HomeButtonCommand = new NavigateCommand<BackuperListingViewModel>(navigateToBackuperListingViewModel);
    }

    private async Task ResetToDefault() {

        var result = await _pathsHandler.ResetBackupersDirectory();
        if(result is not PathsHandler.BackupersMigrationResult.AlreadyThere and not PathsHandler.BackupersMigrationResult.Success) {
            var message = ConvertResultToMessage(result);
            _notificationService.Send(message);
        }
        BackupsFolder = "";
        CurrentBackupsFolder = _pathsHandler.GetBackupersDirectory();

        _autoBootService.Set(true);
        _settings.Set(autoBootKey, true.ToString());
        _autoBoot = true;
        OnPropertyChanged(nameof(AutoBoot));

    }

    private async Task ChangePath() {

        var result = await _pathsHandler.SetBackupersDirectoryAsync(BackupsFolder);
        var message = ConvertResultToMessage(result);
        _notificationService.Send(message);

    }

    private static string ConvertResultToMessage(PathsHandler.BackupersMigrationResult result) => result switch {
        PathsHandler.BackupersMigrationResult.Failure => "There has been an error.",
        PathsHandler.BackupersMigrationResult.Success => "The backups have been transferred successfully.",
        PathsHandler.BackupersMigrationResult.InvalidPath => "The given path is invalid.",
        PathsHandler.BackupersMigrationResult.TargetDirectoryIsNotEmpty => "The given path must point to an empty folder.",
        PathsHandler.BackupersMigrationResult.AlreadyThere => "The given path is already set.",
        _ => "There has been an unknown error.",
    };

}