using Backuper.UI.WPF.Commands;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using Backuper.Utils;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Backuper.UI.WPF.ViewModels;

public class SettingsViewModel : ViewModelBase {

    private readonly Settings _settings;
    private readonly INotificationService _notificationService;

    private const string backupsFolderKey = "MainBackupsFolder";
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

    public SettingsViewModel(Settings settings, INotificationService notificationService, NavigationStore navigationStore, NavigationService<BackuperListingViewModel> navigateToBackuperListingViewModel) {
        _settings = settings;
        _notificationService = notificationService;
        CurrentBackupsFolder = settings.Get(backupsFolderKey).Or("Unknown")!;
        _autoBoot = bool.Parse(settings.Get(autoBootKey).Or("True")!);
        ChangeBackupsFolder = new AsyncRelayCommand(ChangeFolder);
        var navigateToSelf = new NavigationService<ViewModelBase>(navigationStore, () => this);
        OpenPathDialogCommand = new NavigateCommand<OpenPathDialogViewModel>(new(navigationStore, () => new(navigateToSelf, (s) => BackupsFolder = s)));
        HomeButton = new NavigateCommand<BackuperListingViewModel>(navigateToBackuperListingViewModel);
    }

    private Task ChangeFolder() {

        if(!Directory.Exists(BackupsFolder)) {
            _notificationService.Send("The new backups folder cannot be inexistent.", "Error");
            return Task.CompletedTask;
        }

        if(Directory.EnumerateFileSystemEntries(BackupsFolder).Any()) {
            _notificationService.Send("The new backups folder must be an existing, empty folder.");
            return Task.CompletedTask;
        }

        _settings.Set(backupsFolderKey, BackupsFolder);
        _notificationService.Send("The backups have been migrated successfully.");
        //todo - migrate the backups
        return Task.CompletedTask;
    }

}