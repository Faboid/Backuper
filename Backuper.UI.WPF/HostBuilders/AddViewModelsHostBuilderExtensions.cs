using Backuper.Core;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using Backuper.UI.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;

namespace Backuper.UI.WPF.HostBuilders;

public static class AddViewModelsHostBuilderExtensions {

    public static IHostBuilder AddMainWindow(this IHostBuilder hostBuilder) {
        return hostBuilder.ConfigureServices(services => {

            services.AddTransient<Func<Window, MainViewModel>>((s) => (win) => new MainViewModel(s.GetRequiredService<NavigationStore>(), win, s.GetRequiredService<INotificationService>()));

            services.AddSingleton((s) => {
                var window = new MainWindow();
                window.DataContext = s.GetRequiredService<Func<Window, MainViewModel>>().Invoke(window);
                return window;
            });

        });
    }

    public static IHostBuilder AddViewModels(this IHostBuilder hostBuilder) {
        return hostBuilder.ConfigureServices(services => {

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

            services.AddTransient<SettingsViewModel>();
            services.AddTransient<BackuperResultViewModel>();
            services.AddTransient<CreateBackuperViewModel>();
            services.AddTransient<BackuperListingViewModel>();
            services.AddTransient<BackupingResultsViewModel>();

        });
    }


    private static Func<IBackuper, EditBackuperViewModel> CreateEditBackuperViewModel(IServiceProvider s) {
        return (backuper) => new EditBackuperViewModel(
            backuper,
            s.GetRequiredService<BackuperStore>(),
            s.GetRequiredService<INotificationService>(),
            s.GetRequiredService<NavigationService<BackuperListingViewModel>>()
        );
    }

    private static Func<IBackuper, BackuperViewModel> CreateBackuperViewModel(IServiceProvider s) {
        return (backuper) => new BackuperViewModel(
            s.GetRequiredService<BackuperStore>(),
            backuper,
            s.GetRequiredService<INotificationService>(),
            new(s.GetRequiredService<NavigationStore>(), () => s.GetRequiredService<Func<IBackuper, EditBackuperViewModel>>().Invoke(backuper))
        );
    }


}