using Microsoft.Extensions.DependencyInjection;

using RealmHelper.Realm.Common.WebApi.Middlewares;

namespace RealmHelper.Realm.Common.WebApi;

public static class ConfigureServices
{
    public static void AddCommonWebApiServices(this IServiceCollection services)
    {
        services.AddControllers();

        services.AddTransient<ExceptionMiddleware>();
    }
}