using Microsoft.Extensions.DependencyInjection;

using RealmHelper.Realm.Common.Application;
using RealmHelper.Realm.Java.Application.Mapping;

namespace RealmHelper.Realm.Java.Application;

public static class ConfigureServices
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddCommonApplicationServices();

        services.AddAutoMapper(typeof(JavaResponseProfile));
    }
}