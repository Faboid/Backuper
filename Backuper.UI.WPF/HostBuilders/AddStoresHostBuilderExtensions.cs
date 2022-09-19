using Backuper.UI.WPF.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backuper.UI.WPF.HostBuilders;

public static class AddStoresHostBuilderExtensions {

    public static IHostBuilder AddStores(this IHostBuilder hostBuilder) {
        return hostBuilder.ConfigureServices(services => {
            
            services.AddSingleton<NavigationStore>();
            services.AddSingleton<BackuperStore>();

        });
    }

}
