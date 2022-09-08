using Backuper.Abstractions;
using Backuper.Core;
using Backuper.Core.Saves;
using Backuper.Core.Services;
using Backuper.Core.Validation;
using Backuper.Core.Versioning;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using Backuper.UI.WPF.ViewModels;
using System.IO;
using System.Windows;

namespace Backuper.UI.WPF;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {

    //temporary path
    private readonly string _mainBackuperDirectory = Path.Combine(Directory.GetCurrentDirectory(), "BackupersData");

    private readonly INotificationService _notificationService;
    private readonly NavigationStore _navigationStore;
    private readonly BackuperStore _backuperStore;

    public App() {
        _navigationStore = new();

        //todo - use a DI container
        _notificationService = new MessageBoxNotificationService();
        var fileInfoProvider = new FileInfoProvider();
        var directoryInfoProvider = new DirectoryInfoProvider();
        var dateTimeProvider = new DateTimeProvider();
        var pathsBuilderService = new PathsBuilderService(_mainBackuperDirectory, dateTimeProvider, directoryInfoProvider);
        var versioningFactory = new BackuperVersioningFactory(pathsBuilderService, directoryInfoProvider);
        var serviceFactory = new BackuperServiceFactory(directoryInfoProvider, fileInfoProvider);
        var backuperConnection = new BackuperConnection();
        var backuperValidator = new BackuperValidator(directoryInfoProvider, fileInfoProvider);
        var backuperFactory = new BackuperFactory(versioningFactory, serviceFactory, backuperConnection, backuperValidator);
        _backuperStore = new(backuperFactory);
    }

    protected override void OnStartup(StartupEventArgs e) {

        _navigationStore.CurrentViewModel = CreateBackuperListingViewModel();

        MainWindow = new MainWindow();
        MainWindow.DataContext = new MainViewModel(_navigationStore, MainWindow);
        MainWindow.Show();

        base.OnStartup(e);
    }

    private BackuperViewModel CreateBackuperViewModel(IBackuper backuper) {
        return new(_backuperStore, backuper, _notificationService, new(_navigationStore, () => CreateEditBackuperViewModel(backuper)));
    }

    private BackuperListingViewModel CreateBackuperListingViewModel() {
        return BackuperListingViewModel.LoadViewModel(_backuperStore, _notificationService, new(_navigationStore, CreateCreateBackuperViewModel), CreateBackuperViewModel);
    }

    private CreateBackuperViewModel CreateCreateBackuperViewModel() {
        return new(_backuperStore, _navigationStore, _notificationService, new(_navigationStore, CreateBackuperListingViewModel));
    }

    private EditBackuperViewModel CreateEditBackuperViewModel(IBackuper backuper) {
        return new(backuper, _backuperStore, _notificationService, new(_navigationStore, CreateBackuperListingViewModel));
    }

}
