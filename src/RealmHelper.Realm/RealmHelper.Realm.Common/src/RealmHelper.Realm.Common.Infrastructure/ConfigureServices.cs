using Microsoft.Extensions.DependencyInjection;

using RealmHelper.Realm.Common.Infrastructure.Services;

namespace RealmHelper.Realm.Common.Infrastructure;

public static class ConfigureServices
{
    public static void AddCommonInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IXboxLiveService, XboxLiveService>();
    }
}