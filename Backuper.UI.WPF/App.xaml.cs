using Backuper.Core.Services;
using Backuper.UI.WPF.HostBuilders;
using Backuper.UI.WPF.Stores;
using Backuper.UI.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using Serilog;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Backuper.UI.WPF;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application, IDisposable {

    private readonly IHost _host;

    public App() {
        _host = Host.CreateDefaultBuilder()
            .UseSerilog((host, loggerConfiguration) => {
                loggerConfiguration
                    .WriteTo.Debug()
                    .WriteTo.File(Path.Combine("Logs", "Log.txt"), rollingInterval: RollingInterval.Day);

                loggerConfiguration.MinimumLevel.Debug();
            })
            .AddIOAbstractions()
            .AddConfigObjects()
            .AddUIComponents()
            .AddViewModels()
            .AddMainWindow()
            .AddBackuper()
            .AddStores()
            .Build();

        Log.Logger = _host.Services.GetRequiredService<Serilog.ILogger>();
        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionLogger;
    }

    private void UnhandledExceptionLogger(object sender, UnhandledExceptionEventArgs e) {
        Exception exception = (Exception)e.ExceptionObject;
        var logger = _host.Services.GetRequiredService<ILogger<App>>();
        logger.LogCritical(exception, "There has been an unhandled exception.");
    }

    protected override void OnStartup(StartupEventArgs e) {
        
        _host.Start();
        var logger = _host.Services.GetRequiredService<ILogger<App>>();
        ViewModelBase startingVM;

        if(e.Args.Length == 1 && e.Args[0] == SettingsService.StartupArguments) {
            logger.LogInformation("Application started from boot.");
            startingVM = _host.Services.GetRequiredService<BackupingResultsViewModel>();
        } else {
            logger.LogInformation("Application started manually.");
            startingVM = _host.Services.GetRequiredService<BackuperListingViewModel>();
        }

        _host.Services.GetRequiredService<NavigationStore>().CurrentViewModel = startingVM;

        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();

        base.OnStartup(e);
    }

    private bool _isDisposed = false;
    public void Dispose() {
        if(!_isDisposed) {
            var logger = _host.Services.GetRequiredService<ILogger<App>>();
            logger.LogInformation("Closing the application.");

            _host.Dispose();
            GC.SuppressFinalize(this);
        }
        _isDisposed = true;
    }
}
