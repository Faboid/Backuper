using Backuper.Abstractions;
using Backuper.Core;
using Backuper.Core.Services;
using Backuper.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backuper.UI.WPF.HostBuilders;

public static class AddConfigObjectsHostBuilderExtensions {

    public static IHostBuilder AddConfigObjects(this IHostBuilder hostBuilder) {
        return hostBuilder.ConfigureServices(services => {

            services.AddSingleton<AutoBootService>();
            services.AddSingleton<PathsHandler>();

            services.AddSingleton((s) => new Settings(s.GetRequiredService<IFileInfoProvider>().FromFilePath(s.GetRequiredService<PathsHandler>().GetSettingsFile())));
            services.AddSingleton<SettingsService>();

        });
    }

}
