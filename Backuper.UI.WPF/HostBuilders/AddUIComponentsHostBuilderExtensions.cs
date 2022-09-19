using Backuper.UI.WPF.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backuper.UI.WPF.HostBuilders;

public static class AddUIComponentsHostBuilderExtensions {

    public static IHostBuilder AddUIComponents(this IHostBuilder hostBuilder) {
        return hostBuilder.ConfigureServices(services => {
            services.AddSingleton<INotificationService, NotificationService>();
        });
    }

}
