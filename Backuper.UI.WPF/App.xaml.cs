using Backuper.Abstractions;
using Backuper.Core;
using Backuper.Core.Saves;
using Backuper.Core.Services;
using Backuper.Core.Validation;
using Backuper.Core.Versioning;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using Backuper.UI.WPF.ViewModels;
using Backuper.Utils;
using System.Windows;

namespace Backuper.UI.WPF;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {

    private readonly INotificationService _notificationService;
    private readonly NavigationStore _navigationStore;
    private readonly BackuperStore _backuperStore;
    private readonly SettingsService _settingsService;

    public App() {
        _navigationStore = new();
        //todo - use a DI container
        _notificationService = new MessageBoxNotificationService();
        var fileInfoProvider = new FileInfoProvider();
        var directoryInfoProvider = new DirectoryInfoProvider();
        var dateTimeProvider = new DateTimeProvider();
        var pathsHandler = new PathsHandler(directoryInfoProvider, fileInfoProvider);
        var pathsBuilderService = new PathsBuilderService(pathsHandler, dateTimeProvider, directoryInfoProvider);
        var versioningFactory = new BackuperVersioningFactory(pathsBuilderService, directoryInfoProvider);
        var serviceFactory = new BackuperServiceFactory(directoryInfoProvider, fileInfoProvider);
        var backuperConnection = new BackuperConnection(pathsHandler);
        var backuperValidator = new BackuperValidator(directoryInfoProvider, fileInfoProvider);
        var backuperFactory = new BackuperFactory(versioningFactory, serviceFactory, backuperConnection, backuperValidator);
        _backuperStore = new(backuperFactory);
        var autoBootService = new AutoBootService(new ShortcutProvider());
        var settings = new Settings(fileInfoProvider.FromFilePath(pathsHandler.GetSettingsFile()));
        _settingsService = new(pathsHandler, autoBootService, settings);
    }

    protected override void OnStartup(StartupEventArgs e) {

        ViewModelBase startingVM;

        if(e.Args.Length == 1 && e.Args[0] == SettingsService.StartupArguments) {
            startingVM = CreateBackupingResultsViewModel();
        } else {
            startingVM = CreateBackuperListingViewModel();
        }

        _navigationStore.CurrentViewModel = startingVM;

        MainWindow = new MainWindow();
        MainWindow.DataContext = new MainViewModel(_navigationStore, MainWindow);
        MainWindow.Show();

        base.OnStartup(e);
    }

    private SettingsViewModel CreateSettingsViewModel() {
        return new SettingsViewModel(_settingsService, _notificationService, _navigationStore, new(_navigationStore, CreateBackuperListingViewModel));
    }

    private BackuperViewModel CreateBackuperViewModel(IBackuper backuper) {
        return new(_backuperStore, backuper, _notificationService, new(_navigationStore, () => CreateEditBackuperViewModel(backuper)));
    }

    private BackupingResultsViewModel CreateBackupingResultsViewModel() {
        return BackupingResultsViewModel.LoadViewModel(_backuperStore, new(_navigationStore, CreateBackuperListingViewModel));
    }

    private BackuperListingViewModel CreateBackuperListingViewModel() {
        return BackuperListingViewModel.LoadViewModel(_backuperStore, _notificationService, 
            new(_navigationStore, CreateCreateBackuperViewModel), 
            new(_navigationStore, CreateBackupingResultsViewModel), 
            new(_navigationStore, CreateSettingsViewModel), 
            CreateBackuperViewModel);
    }

    private CreateBackuperViewModel CreateCreateBackuperViewModel() {
        return new(_backuperStore, _navigationStore, _notificationService, new(_navigationStore, CreateBackuperListingViewModel));
    }

    private EditBackuperViewModel CreateEditBackuperViewModel(IBackuper backuper) {
        return new(backuper, _backuperStore, _notificationService, new(_navigationStore, CreateBackuperListingViewModel));
    }

}
