using Microsoft.Extensions.DependencyInjection;

using RealmHelper.Realm.Common.WebApi.Middlewares;

namespace RealmHelper.Realm.Common.WebApi;

public static class ConfigureServices
{
    public static void AddCommonWebApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        
        services.AddCors(opts => opts.AddDefaultPolicy(b => b
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true)));

        services.AddTransient<ExceptionMiddleware>();
    }
}