using Backuper.Core;
using Backuper.Core.Saves;
using Backuper.Core.Services;
using Backuper.Core.Validation;
using Backuper.Core.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backuper.UI.WPF.HostBuilders;

public static class AddBackuperHostBuilderExtensions {

    public static IHostBuilder AddBackuper(this IHostBuilder hostBuilder) {
        return hostBuilder.ConfigureServices(services => {

            services.AddSingleton<IPathsBuilderService, PathsBuilderService>();
            services.AddSingleton<IBackuperVersioningFactory, BackuperVersioningFactory>();
            services.AddSingleton<IBackuperServiceFactory, BackuperServiceFactory>();
            services.AddSingleton<IBackuperConnection, BackuperConnection>();
            services.AddSingleton<IBackuperValidator, BackuperValidator>();

            services.AddSingleton<IBackuperFactory, BackuperFactory>();

        });
    }

}
