using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using RealmHelper.Realm.Bedrock.Abstractions.Services;
using RealmHelper.Realm.Bedrock.Application.Repositories;
using RealmHelper.Realm.Bedrock.Infrastructure.Authentication;
using RealmHelper.Realm.Bedrock.Infrastructure.Repositories;
using RealmHelper.Realm.Bedrock.Infrastructure.Services;
using RealmHelper.Realm.Common.Infrastructure;

namespace RealmHelper.Realm.Bedrock.Infrastructure;

public static class ConfigureServices
{
    public static void AddBedrockInfrastructureServices(this IServiceCollection services)
    {
        services.AddCommonInfrastructureServices();

        services.AddMemoryCache();

        services.AddHttpContextAccessor();

        services.AddScoped<ClaimsPrincipal>(ctx => ctx.GetRequiredService<IHttpContextAccessor>().HttpContext!.User);

        services.AddScoped<IBedrockRealmRepository, BedrockRealmRepository>();
        services.AddScoped<IBedrockRealmService, BedrockRealmService>();

        services.AddAuthentication(XboxLiveDefaults.AuthenticationScheme)
            .AddScheme<XboxLiveOptions, XboxLiveHandler>(XboxLiveDefaults.AuthenticationScheme, _ => { });
    }
}