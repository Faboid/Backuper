using Backuper.Abstractions;
using Backuper.Core;
using Backuper.Core.Saves;
using Backuper.Core.Services;
using Backuper.Core.Validation;
using Backuper.Core.Versioning;
using Backuper.UI.WPF.HostBuilders;
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
        _host = Host.CreateDefaultBuilder()
            .AddIOAbstractions()
            .AddConfigObjects()
            .AddUIComponents()
            .AddViewModels()
            .AddMainWindow()
            .AddBackuper()
            .AddStores()
            .Build();
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

    private bool _isDisposed = false;
    public void Dispose() {
        if(!_isDisposed) {
            _host.Dispose();
            GC.SuppressFinalize(this);
        }
        _isDisposed = true;
    }
}
