using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using RealmHelper.Realm.Common.Infrastructure;
using RealmHelper.Realm.Java.Abstractions.Services;
using RealmHelper.Realm.Java.Application.Repositories;
using RealmHelper.Realm.Java.Infrastructure.Authentication;
using RealmHelper.Realm.Java.Infrastructure.Repositories;
using RealmHelper.Realm.Java.Infrastructure.Services;

namespace RealmHelper.Realm.Java.Infrastructure;

public static class ConfigureServices
{
    public static void AddJavaInfrastructureServices(this IServiceCollection services)
    {
        services.AddCommonInfrastructureServices();

        services.AddHttpContextAccessor();
        
        services.AddScoped<ClaimsPrincipal>(ctx => ctx.GetRequiredService<IHttpContextAccessor>().HttpContext!.User);

        services.AddMemoryCache();

        services.AddAuthentication(MinecraftJavaDefaults.AuthenticationScheme)
            .AddScheme<MinecraftJavaOptions,
                MinecraftJavaHandler>(MinecraftJavaDefaults.AuthenticationScheme, _ => { });
        
        services.AddScoped<IJavaRealmRepository, JavaRealmRepository>();
        services.AddScoped<IJavaRealmService, JavaRealmService>();
    }
}