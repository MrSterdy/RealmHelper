using Microsoft.Extensions.DependencyInjection;

using RealmHelper.Realm.Common.Application.Mapping;

namespace RealmHelper.Realm.Common.Application;

public static class ConfigureServices
{
    public static void AddCommonApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(CommonResponseProfile));
    }
}