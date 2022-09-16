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
    private readonly AutoBootService _autoBootService;
    private readonly PathsHandler _pathsHandler;
    private readonly Settings _settings;

    public App() {
        _navigationStore = new();
        _autoBootService = new(new ShortcutProvider());
        //todo - use a DI container
        _notificationService = new MessageBoxNotificationService();
        var fileInfoProvider = new FileInfoProvider();
        var directoryInfoProvider = new DirectoryInfoProvider();
        _pathsHandler = new(directoryInfoProvider, fileInfoProvider);
        var dateTimeProvider = new DateTimeProvider();
        var pathsBuilderService = new PathsBuilderService(_pathsHandler, dateTimeProvider, directoryInfoProvider);
        var versioningFactory = new BackuperVersioningFactory(pathsBuilderService, directoryInfoProvider);
        var serviceFactory = new BackuperServiceFactory(directoryInfoProvider, fileInfoProvider);
        var backuperConnection = new BackuperConnection(_pathsHandler);
        var backuperValidator = new BackuperValidator(directoryInfoProvider, fileInfoProvider);
        var backuperFactory = new BackuperFactory(versioningFactory, serviceFactory, backuperConnection, backuperValidator);
        _backuperStore = new(backuperFactory);
        _settings = new(fileInfoProvider.FromFilePath(_pathsHandler.GetSettingsFile()));
    }

    protected override void OnStartup(StartupEventArgs e) {

        _navigationStore.CurrentViewModel = CreateBackuperListingViewModel();

        MainWindow = new MainWindow();
        MainWindow.DataContext = new MainViewModel(_navigationStore, MainWindow);
        MainWindow.Show();

        base.OnStartup(e);
    }

    private SettingsViewModel CreateSettingsViewModel() {
        return new SettingsViewModel(_settings, _pathsHandler, _autoBootService, _notificationService, _navigationStore, new(_navigationStore, CreateBackuperListingViewModel));
    }

    private BackuperViewModel CreateBackuperViewModel(IBackuper backuper) {
        return new(_backuperStore, backuper, _notificationService, new(_navigationStore, () => CreateEditBackuperViewModel(backuper)));
    }

    private BackupingResultsViewModel CreateBackupingResultsViewModel() {
        return BackupingResultsViewModel.LoadViewModel(_backuperStore, _notificationService, new(_navigationStore, CreateBackuperListingViewModel));
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
