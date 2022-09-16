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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;

namespace Backuper.UI.WPF;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application, IDisposable {

    private readonly IHost _host;

    public App() {

        _host = Host.CreateDefaultBuilder().ConfigureServices(services => {
            services.AddSingleton<NavigationStore>();
            services.AddSingleton<INotificationService, NotificationService>();
            
            services.AddSingleton<IFileInfoProvider, FileInfoProvider>();
            services.AddSingleton<IDirectoryInfoProvider, DirectoryInfoProvider>();
            services.AddSingleton<IShortcutProvider, ShortcutProvider>();

            services.AddSingleton<AutoBootService>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<PathsHandler>();

            services.AddSingleton<IPathsBuilderService, PathsBuilderService>();
            services.AddSingleton<IBackuperVersioningFactory, BackuperVersioningFactory>();
            services.AddSingleton<IBackuperServiceFactory, BackuperServiceFactory>();
            services.AddSingleton<IBackuperConnection, BackuperConnection>();
            services.AddSingleton<IBackuperValidator, BackuperValidator>();

            services.AddSingleton<IBackuperFactory, BackuperFactory>();
            services.AddSingleton<BackuperStore>();

            services.AddSingleton((s) => new Settings(s.GetRequiredService<IFileInfoProvider>().FromFilePath(s.GetRequiredService<PathsHandler>().GetSettingsFile())));
            services.AddSingleton<SettingsService>();

            services.AddSingleton<Func<SettingsViewModel>>((s) => () => s.GetRequiredService<SettingsViewModel>());
            services.AddSingleton<Func<BackupingResultsViewModel>>((s) => () => s.GetRequiredService<BackupingResultsViewModel>());
            services.AddSingleton<Func<CreateBackuperViewModel>>((s) => () => s.GetRequiredService<CreateBackuperViewModel>());
            services.AddSingleton<Func<BackuperListingViewModel>>((s) => () => s.GetRequiredService<BackuperListingViewModel>());
            services.AddSingleton(CreateEditBackuperViewModel);
            services.AddSingleton(CreateBackuperViewModel);

            services.AddSingleton<NavigationService<SettingsViewModel>>();
            services.AddSingleton<NavigationService<BackupingResultsViewModel>>();
            services.AddSingleton<NavigationService<CreateBackuperViewModel>>();
            services.AddSingleton<NavigationService<BackuperListingViewModel>>();
            services.AddSingleton<NavigationService<EditBackuperViewModel>>();

            services.AddTransient<SettingsViewModel>();
            services.AddTransient<BackuperResultViewModel>();
            services.AddTransient<CreateBackuperViewModel>();
            services.AddTransient<BackuperListingViewModel>();
            services.AddTransient<BackupingResultsViewModel>();
            
            services.AddTransient<Func<Window, MainViewModel>>((s) => (win) => new MainViewModel(s.GetRequiredService<NavigationStore>(), win, s.GetRequiredService<INotificationService>()));

            services.AddSingleton((s) => {
                var window = new MainWindow();
                window.DataContext = s.GetRequiredService<Func<Window, MainViewModel>>().Invoke(window);
                return window;
            });

        }).Build();

    }

    protected override void OnStartup(StartupEventArgs e) {

        _host.Start();
        ViewModelBase startingVM;

        if(e.Args.Length == 1 && e.Args[0] == SettingsService.StartupArguments) {
            startingVM = _host.Services.GetRequiredService<BackupingResultsViewModel>();
        } else {
            startingVM = _host.Services.GetRequiredService<BackuperListingViewModel>();
        }

        _host.Services.GetRequiredService<NavigationStore>().CurrentViewModel = startingVM;

        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();

        base.OnStartup(e);
    }

    private BackuperViewModel CreateBackuperViewModel(IBackuper backuper) {
        var s = _host.Services;
        return new(
            s.GetRequiredService<BackuperStore>(), 
            backuper, 
            s.GetRequiredService<INotificationService>(), 
            new(s.GetRequiredService<NavigationStore>(), () => CreateEditBackuperViewModel(backuper)));
    }

    private EditBackuperViewModel CreateEditBackuperViewModel(IBackuper backuper) {
        var s = _host.Services;
        return new (
            backuper, 
            s.GetRequiredService<BackuperStore>(), 
            s.GetRequiredService<INotificationService>(), 
            s.GetRequiredService<NavigationService<BackuperListingViewModel>>()
        );
    }

    private bool _isDisposed = false;
    public void Dispose() {
        if(!_isDisposed) {
            _host.Dispose();
            GC.SuppressFinalize(this);
        }
        _isDisposed = true;
    }
}
