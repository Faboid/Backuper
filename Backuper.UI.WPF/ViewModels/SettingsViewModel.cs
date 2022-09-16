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

    private readonly SettingsService _settingsService;
    private readonly INotificationService _notificationService;

    private string _currentBackupsFolder;
    public string CurrentBackupsFolder { get => _currentBackupsFolder; private set => SetAndRaise(nameof(CurrentBackupsFolder), ref _currentBackupsFolder, value); }

    private string _backupsFolder = "";
    public string BackupsFolder { get => _backupsFolder; set => SetAndRaise(nameof(BackupsFolder), ref _backupsFolder, value); }

    private bool _autoBoot;
    public bool AutoBoot {
        get => _autoBoot;
        set {
            SetAndRaise(nameof(_autoBoot), ref _autoBoot, value);

            _settingsService.SetAutoBoot(AutoBoot);
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

    public SettingsViewModel(SettingsService settingsService, INotificationService notificationService, NavigationStore navigationStore, NavigationService<BackuperListingViewModel> navigateToBackuperListingViewModel) {
        _settingsService = settingsService;
        _notificationService = notificationService;
        _currentBackupsFolder = _settingsService.GetBackupersDirectory();
        _autoBoot = _settingsService.GetAutoBoot();
        ChangeBackupersPathCommand = new AsyncRelayCommand(ChangePath);
        ResetToDefaultCommand = new AsyncRelayCommand(ResetToDefault);
        var navigateToSelf = new NavigationService<ViewModelBase>(navigationStore, () => this);
        OpenPathDialogCommand = new NavigateCommand<OpenPathDialogViewModel>(new(navigationStore, () => new(navigateToSelf, (s) => BackupsFolder = s)));
        HomeButtonCommand = new NavigateCommand<BackuperListingViewModel>(navigateToBackuperListingViewModel);
    }

    private async Task ResetToDefault() {

        var result = await _settingsService.ResetBackupersDirectory();
        if(result is not PathsHandler.BackupersMigrationResult.AlreadyThere and not PathsHandler.BackupersMigrationResult.Success) {
            var message = ConvertResultToMessage(result);
            _notificationService.Send(message);
        }
        BackupsFolder = "";
        CurrentBackupsFolder = _settingsService.GetBackupersDirectory();

        _settingsService.SetAutoBoot(true);
        _autoBoot = true;
        OnPropertyChanged(nameof(AutoBoot));

    }

    private async Task ChangePath() {

        var result = await _settingsService.SetBackupersDirectoryAsync(BackupsFolder);
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