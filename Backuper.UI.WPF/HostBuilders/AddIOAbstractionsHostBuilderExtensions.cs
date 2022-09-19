using Backuper.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backuper.UI.WPF.HostBuilders;

public static class AddIOAbstractionsHostBuilderExtensions {

    public static IHostBuilder AddIOAbstractions(this IHostBuilder hostBuilder) {
        return hostBuilder.ConfigureServices(services => {

            services.AddSingleton<IFileInfoProvider, FileInfoProvider>();
            services.AddSingleton<IDirectoryInfoProvider, DirectoryInfoProvider>();
            services.AddSingleton<IShortcutProvider, ShortcutProvider>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        });
    }

}
